# Database Restore Script for Patient Management System
# Usage: .\restore-database.ps1 -BackupFile "path\to\backup.bak" [-Server localhost] [-Database PatientManagementDb] [-Force]

param(
    [Parameter(Mandatory=$true)]
    [string]$BackupFile,
    [string]$Server = "localhost",
    [string]$Database = "PatientManagementDb",
    [string]$DataPath = "",
    [string]$LogPath = "",
    [switch]$Force = $false,
    [switch]$NoRecovery = $false
)

# Import SQL Server module
try {
    Import-Module SqlServer -ErrorAction Stop
} catch {
    Write-Error "SQL Server module not found. Install with: Install-Module -Name SqlServer"
    exit 1
}

# Verify backup file exists
if (-not (Test-Path $BackupFile)) {
    Write-Error "Backup file not found: $BackupFile"
    exit 1
}

Write-Host "=== Patient Management Database Restore ===" -ForegroundColor Cyan
Write-Host "Server: $Server" -ForegroundColor White
Write-Host "Database: $Database" -ForegroundColor White
Write-Host "Backup File: $BackupFile" -ForegroundColor White
Write-Host "Force: $Force" -ForegroundColor White
Write-Host ""

try {
    # Test connection
    Write-Host "Testing database connection..." -ForegroundColor Yellow
    $testQuery = "SELECT @@VERSION"
    Invoke-Sqlcmd -ServerInstance $Server -Database "master" -Query $testQuery -ErrorAction Stop | Out-Null
    Write-Host "✓ Connection successful" -ForegroundColor Green
    Write-Host ""

    # Check if database exists
    $dbCheckQuery = "SELECT name FROM sys.databases WHERE name = '$Database'"
    $dbExists = Invoke-Sqlcmd -ServerInstance $Server -Database "master" -Query $dbCheckQuery

    if ($dbExists -and -not $Force) {
        Write-Warning "Database '$Database' already exists!"
        $response = Read-Host "Do you want to overwrite it? This will delete all existing data! (yes/no)"
        if ($response -ne "yes") {
            Write-Host "Restore cancelled by user" -ForegroundColor Yellow
            exit 0
        }
    }

    # Get backup file list
    Write-Host "Reading backup file information..." -ForegroundColor Yellow
    $fileListQuery = "RESTORE FILELISTONLY FROM DISK = N'$BackupFile'"
    $fileList = Invoke-Sqlcmd -ServerInstance $Server -Database "master" -Query $fileListQuery -ErrorAction Stop
    
    Write-Host "✓ Backup file verified" -ForegroundColor Green
    Write-Host "  Files in backup:" -ForegroundColor Gray
    foreach ($file in $fileList) {
        Write-Host "    - $($file.LogicalName) ($($file.Type))" -ForegroundColor Gray
    }
    Write-Host ""

    # Get default data and log paths if not specified
    if ([string]::IsNullOrEmpty($DataPath)) {
        $pathQuery = @"
SELECT 
    SERVERPROPERTY('InstanceDefaultDataPath') as DataPath,
    SERVERPROPERTY('InstanceDefaultLogPath') as LogPath
"@
        $paths = Invoke-Sqlcmd -ServerInstance $Server -Database "master" -Query $pathQuery
        $DataPath = $paths.DataPath
        $LogPath = $paths.LogPath
    }

    Write-Host "Target Paths:" -ForegroundColor Gray
    Write-Host "  Data: $DataPath" -ForegroundColor Gray
    Write-Host "  Log: $LogPath" -ForegroundColor Gray
    Write-Host ""

    # Kill existing connections if database exists
    if ($dbExists) {
        Write-Host "Closing existing connections..." -ForegroundColor Yellow
        $killQuery = @"
ALTER DATABASE [$Database] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
"@
        Invoke-Sqlcmd -ServerInstance $Server -Database "master" -Query $killQuery -ErrorAction Stop
        Write-Host "✓ Connections closed" -ForegroundColor Green
        Write-Host ""
    }

    # Build restore command with file moves
    $moveStatements = @()
    foreach ($file in $fileList) {
        $targetPath = if ($file.Type -eq 'D') { $DataPath } else { $LogPath }
        $fileName = Split-Path $file.PhysicalName -Leaf
        $newPath = Join-Path $targetPath $fileName
        $moveStatements += "MOVE N'$($file.LogicalName)' TO N'$newPath'"
    }

    $restoreQuery = @"
RESTORE DATABASE [$Database] 
FROM DISK = N'$BackupFile' 
WITH FILE = 1, 
$($moveStatements -join ", `n"),
NOUNLOAD, 
REPLACE, 
STATS = 10
$(if ($NoRecovery) { ", NORECOVERY" })
"@

    # Perform restore
    Write-Host "Starting restore operation..." -ForegroundColor Yellow
    Write-Host "This may take several minutes depending on database size..." -ForegroundColor Gray
    Write-Host ""

    $startTime = Get-Date
    Invoke-Sqlcmd -ServerInstance $Server -Database "master" -Query $restoreQuery -QueryTimeout 7200 -ErrorAction Stop
    $endTime = Get-Date
    $duration = ($endTime - $startTime).TotalSeconds

    Write-Host "✓ Restore completed successfully" -ForegroundColor Green
    Write-Host "  Duration: $([math]::Round($duration, 2)) seconds" -ForegroundColor Gray
    Write-Host ""

    # Set database to multi-user mode
    if ($dbExists -or -not $NoRecovery) {
        Write-Host "Setting database to multi-user mode..." -ForegroundColor Yellow
        $multiUserQuery = "ALTER DATABASE [$Database] SET MULTI_USER;"
        Invoke-Sqlcmd -ServerInstance $Server -Database "master" -Query $multiUserQuery -ErrorAction Stop
        Write-Host "✓ Database ready for use" -ForegroundColor Green
        Write-Host ""
    }

    # Verify database integrity
    Write-Host "Verifying database integrity..." -ForegroundColor Yellow
    $checkDbQuery = "DBCC CHECKDB([$Database]) WITH NO_INFOMSGS, ALL_ERRORMSGS"
    Invoke-Sqlcmd -ServerInstance $Server -Database "master" -Query $checkDbQuery -QueryTimeout 1800 -ErrorAction Stop | Out-Null
    Write-Host "✓ Database integrity verified" -ForegroundColor Green
    Write-Host ""

    # Get database info
    $infoQuery = @"
SELECT 
    name as DatabaseName,
    state_desc as State,
    recovery_model_desc as RecoveryModel,
    (SELECT COUNT(*) FROM [$Database].sys.tables WHERE is_ms_shipped = 0) as TableCount
FROM sys.databases 
WHERE name = '$Database'
"@
    $dbInfo = Invoke-Sqlcmd -ServerInstance $Server -Database "master" -Query $infoQuery

    Write-Host "=== Restore Summary ===" -ForegroundColor Cyan
    Write-Host "✓ Database: $($dbInfo.DatabaseName)" -ForegroundColor Green
    Write-Host "✓ State: $($dbInfo.State)" -ForegroundColor Green
    Write-Host "✓ Recovery Model: $($dbInfo.RecoveryModel)" -ForegroundColor Green
    Write-Host "✓ Tables: $($dbInfo.TableCount)" -ForegroundColor Green
    Write-Host "✓ Status: Restore successful" -ForegroundColor Green
    Write-Host ""

    # Log restore operation
    $logEntry = [PSCustomObject]@{
        Timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
        Server = $Server
        Database = $Database
        BackupFile = $BackupFile
        Duration = [math]::Round($duration, 2)
        Status = "Success"
    }

    $logPath = ".\restore-log.csv"
    $logEntry | Export-Csv -Path $logPath -Append -NoTypeInformation

    exit 0

} catch {
    Write-Host "✗ Restore Failed!" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    
    # Try to recover database state
    try {
        $recoverQuery = "ALTER DATABASE [$Database] SET MULTI_USER;"
        Invoke-Sqlcmd -ServerInstance $Server -Database "master" -Query $recoverQuery -ErrorAction SilentlyContinue
    } catch {
        # Ignore recovery errors
    }

    # Log error
    $errorEntry = [PSCustomObject]@{
        Timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
        Server = $Server
        Database = $Database
        BackupFile = $BackupFile
        Duration = 0
        Status = "Failed: $($_.Exception.Message)"
    }

    $logPath = ".\restore-log.csv"
    $errorEntry | Export-Csv -Path $logPath -Append -NoTypeInformation

    exit 1
}

# Database Backup Script for Patient Management System
# Supports both local SQL Server and Docker deployments
# Usage: .\backup-database.ps1 [-Server localhost] [-Database PatientManagementDb] [-BackupPath ./backups] [-RetentionDays 30]

param(
    [string]$Server = "localhost",
    [string]$Database = "PatientManagementDb",
    [string]$BackupPath = ".\backups",
    [int]$RetentionDays = 30,
    [switch]$Compress = $true,
    [switch]$Verify = $true
)

# Import SQL Server module
try {
    Import-Module SqlServer -ErrorAction Stop
} catch {
    Write-Error "SQL Server module not found. Install with: Install-Module -Name SqlServer"
    exit 1
}

# Create backup directory if it doesn't exist
if (-not (Test-Path $BackupPath)) {
    New-Item -ItemType Directory -Path $BackupPath -Force | Out-Null
    Write-Host "Created backup directory: $BackupPath" -ForegroundColor Green
}

# Generate backup filename with timestamp
$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
$backupFileName = "${Database}_${timestamp}.bak"
$backupFilePath = Join-Path $BackupPath $backupFileName

Write-Host "=== Patient Management Database Backup ===" -ForegroundColor Cyan
Write-Host "Server: $Server" -ForegroundColor White
Write-Host "Database: $Database" -ForegroundColor White
Write-Host "Backup Path: $backupFilePath" -ForegroundColor White
Write-Host "Compression: $Compress" -ForegroundColor White
Write-Host "Verification: $Verify" -ForegroundColor White
Write-Host ""

try {
    # Test database connection
    Write-Host "Testing database connection..." -ForegroundColor Yellow
    $testQuery = "SELECT @@VERSION"
    $connectionResult = Invoke-Sqlcmd -ServerInstance $Server -Database "master" -Query $testQuery -ErrorAction Stop
    Write-Host "✓ Connection successful" -ForegroundColor Green
    Write-Host "  SQL Version: $($connectionResult.Column1.Split("`n")[0])" -ForegroundColor Gray
    Write-Host ""

    # Check database exists
    Write-Host "Verifying database exists..." -ForegroundColor Yellow
    $dbCheckQuery = "SELECT name FROM sys.databases WHERE name = '$Database'"
    $dbExists = Invoke-Sqlcmd -ServerInstance $Server -Database "master" -Query $dbCheckQuery -ErrorAction Stop
    
    if (-not $dbExists) {
        throw "Database '$Database' not found on server '$Server'"
    }
    Write-Host "✓ Database verified" -ForegroundColor Green
    Write-Host ""

    # Get database size
    $sizeQuery = @"
SELECT 
    DB_NAME() as DatabaseName,
    SUM(size * 8.0 / 1024) as SizeMB
FROM sys.master_files
WHERE database_id = DB_ID('$Database')
GROUP BY database_id
"@
    $dbSize = Invoke-Sqlcmd -ServerInstance $Server -Database $Database -Query $sizeQuery
    Write-Host "Database Size: $([math]::Round($dbSize.SizeMB, 2)) MB" -ForegroundColor Gray
    Write-Host ""

    # Perform backup
    Write-Host "Starting backup operation..." -ForegroundColor Yellow
    $backupQuery = @"
BACKUP DATABASE [$Database] 
TO DISK = N'$backupFilePath' 
WITH NOFORMAT, NOINIT, 
NAME = N'$Database-Full Database Backup', 
SKIP, NOREWIND, NOUNLOAD, STATS = 10
$(if ($Compress) { ", COMPRESSION" })
"@

    $startTime = Get-Date
    Invoke-Sqlcmd -ServerInstance $Server -Database "master" -Query $backupQuery -QueryTimeout 3600 -ErrorAction Stop
    $endTime = Get-Date
    $duration = ($endTime - $startTime).TotalSeconds

    Write-Host "✓ Backup completed successfully" -ForegroundColor Green
    Write-Host "  Duration: $([math]::Round($duration, 2)) seconds" -ForegroundColor Gray
    
    # Get backup file size
    $backupFileInfo = Get-Item $backupFilePath
    $backupSizeMB = [math]::Round($backupFileInfo.Length / 1MB, 2)
    $compressionRatio = if ($dbSize.SizeMB -gt 0) { 
        [math]::Round(($dbSize.SizeMB - $backupSizeMB) / $dbSize.SizeMB * 100, 2) 
    } else { 0 }
    
    Write-Host "  Backup Size: $backupSizeMB MB" -ForegroundColor Gray
    Write-Host "  Compression Ratio: $compressionRatio%" -ForegroundColor Gray
    Write-Host ""

    # Verify backup integrity
    if ($Verify) {
        Write-Host "Verifying backup integrity..." -ForegroundColor Yellow
        $verifyQuery = "RESTORE VERIFYONLY FROM DISK = N'$backupFilePath'"
        Invoke-Sqlcmd -ServerInstance $Server -Database "master" -Query $verifyQuery -QueryTimeout 600 -ErrorAction Stop
        Write-Host "✓ Backup verification successful" -ForegroundColor Green
        Write-Host ""
    }

    # Cleanup old backups
    Write-Host "Cleaning up old backups (retention: $RetentionDays days)..." -ForegroundColor Yellow
    $cutoffDate = (Get-Date).AddDays(-$RetentionDays)
    $oldBackups = Get-ChildItem -Path $BackupPath -Filter "${Database}_*.bak" | 
                  Where-Object { $_.LastWriteTime -lt $cutoffDate }
    
    if ($oldBackups) {
        foreach ($oldBackup in $oldBackups) {
            Remove-Item $oldBackup.FullName -Force
            Write-Host "  Deleted: $($oldBackup.Name)" -ForegroundColor Gray
        }
        Write-Host "✓ Cleanup completed: $($oldBackups.Count) old backup(s) removed" -ForegroundColor Green
    } else {
        Write-Host "  No old backups to clean up" -ForegroundColor Gray
    }
    Write-Host ""

    # Log backup metadata
    $logEntry = [PSCustomObject]@{
        Timestamp = $timestamp
        Server = $Server
        Database = $Database
        BackupFile = $backupFileName
        BackupSizeMB = $backupSizeMB
        DatabaseSizeMB = [math]::Round($dbSize.SizeMB, 2)
        CompressionRatio = $compressionRatio
        Duration = [math]::Round($duration, 2)
        Verified = $Verify
        Status = "Success"
    }

    $logPath = Join-Path $BackupPath "backup-log.csv"
    $logEntry | Export-Csv -Path $logPath -Append -NoTypeInformation

    Write-Host "=== Backup Summary ===" -ForegroundColor Cyan
    Write-Host "✓ Backup File: $backupFileName" -ForegroundColor Green
    Write-Host "✓ Location: $backupFilePath" -ForegroundColor Green
    Write-Host "✓ Size: $backupSizeMB MB" -ForegroundColor Green
    Write-Host "✓ Status: Success" -ForegroundColor Green
    Write-Host ""

    # Return success
    exit 0

} catch {
    Write-Host "✗ Backup Failed!" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    
    # Log error
    $errorEntry = [PSCustomObject]@{
        Timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
        Server = $Server
        Database = $Database
        BackupFile = $backupFileName
        BackupSizeMB = 0
        DatabaseSizeMB = 0
        CompressionRatio = 0
        Duration = 0
        Verified = $false
        Status = "Failed: $($_.Exception.Message)"
    }

    $logPath = Join-Path $BackupPath "backup-log.csv"
    $errorEntry | Export-Csv -Path $logPath -Append -NoTypeInformation

    exit 1
}

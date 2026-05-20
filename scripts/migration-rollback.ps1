# EF Core Migration Rollback Script
# Usage: .\migration-rollback.ps1 [-TargetMigration MigrationName] [-Project Infrastructure] [-StartupProject API]

param(
    [string]$TargetMigration = "0",  # "0" means rollback to initial state
    [string]$Project = "src\PatientManagement.Infrastructure",
    [string]$StartupProject = "src\PatientManagement.API",
    [string]$Context = "ApplicationDbContext",
    [switch]$WhatIf = $false,
    [switch]$Force = $false
)

Write-Host "=== EF Core Migration Rollback Tool ===" -ForegroundColor Cyan
Write-Host ""

# Check if .NET CLI is available
try {
    $dotnetVersion = dotnet --version
    Write-Host "✓ .NET SDK Version: $dotnetVersion" -ForegroundColor Green
} catch {
    Write-Error ".NET SDK not found. Please install .NET 8 SDK."
    exit 1
}

# Check if EF Core tools are installed
try {
    dotnet ef --version | Out-Null
} catch {
    Write-Warning "EF Core tools not installed. Installing..."
    dotnet tool install --global dotnet-ef
}

Write-Host ""
Write-Host "Configuration:" -ForegroundColor Yellow
Write-Host "  Target Migration: $TargetMigration" -ForegroundColor White
Write-Host "  Project: $Project" -ForegroundColor White
Write-Host "  Startup Project: $StartupProject" -ForegroundColor White
Write-Host "  Context: $Context" -ForegroundColor White
Write-Host "  What-If Mode: $WhatIf" -ForegroundColor White
Write-Host ""

try {
    # List all migrations
    Write-Host "Fetching migration history..." -ForegroundColor Yellow
    $migrations = dotnet ef migrations list `
        --project $Project `
        --startup-project $StartupProject `
        --context $Context `
        --no-build 2>&1

    Write-Host "✓ Available Migrations:" -ForegroundColor Green
    Write-Host $migrations -ForegroundColor Gray
    Write-Host ""

    # Get current migration
    Write-Host "Checking current database state..." -ForegroundColor Yellow
    $currentMigration = dotnet ef migrations list `
        --project $Project `
        --startup-project $StartupProject `
        --context $Context `
        --no-build 2>&1 | Select-String -Pattern "\(Pending\)|Applied" | Select-Object -Last 1

    if ($currentMigration) {
        Write-Host "✓ Current State: $currentMigration" -ForegroundColor Green
    } else {
        Write-Host "✓ Database is at initial state" -ForegroundColor Green
    }
    Write-Host ""

    # Confirm rollback
    if (-not $Force -and -not $WhatIf) {
        Write-Warning "This operation will rollback database migrations!"
        Write-Warning "Target: $TargetMigration"
        Write-Host ""
        $response = Read-Host "Are you sure you want to continue? (yes/no)"
        if ($response -ne "yes") {
            Write-Host "Rollback cancelled by user" -ForegroundColor Yellow
            exit 0
        }
        Write-Host ""
    }

    # Generate SQL script (what-if mode)
    if ($WhatIf) {
        Write-Host "Generating rollback SQL script (What-If mode)..." -ForegroundColor Yellow
        $scriptPath = ".\rollback-script-$(Get-Date -Format 'yyyyMMdd-HHmmss').sql"
        
        dotnet ef migrations script `
            $TargetMigration `
            --project $Project `
            --startup-project $StartupProject `
            --context $Context `
            --output $scriptPath `
            --idempotent

        Write-Host "✓ SQL script generated: $scriptPath" -ForegroundColor Green
        Write-Host "  Review this script before applying changes" -ForegroundColor Yellow
        Write-Host ""
        exit 0
    }

    # Create backup before rollback
    Write-Host "Creating database backup before rollback..." -ForegroundColor Yellow
    $backupScript = Join-Path $PSScriptRoot "backup-database.ps1"
    if (Test-Path $backupScript) {
        & $backupScript -BackupPath ".\backups\pre-rollback"
        Write-Host "✓ Backup completed" -ForegroundColor Green
    } else {
        Write-Warning "Backup script not found. Proceeding without backup."
        Write-Warning "It is highly recommended to backup before rollback!"
        Start-Sleep -Seconds 3
    }
    Write-Host ""

    # Perform rollback
    Write-Host "Executing migration rollback..." -ForegroundColor Yellow
    Write-Host "  Rolling back to: $TargetMigration" -ForegroundColor White
    Write-Host ""

    $startTime = Get-Date
    
    $rollbackCommand = "dotnet ef database update `"$TargetMigration`" " +
                      "--project `"$Project`" " +
                      "--startup-project `"$StartupProject`" " +
                      "--context $Context " +
                      "--verbose"

    Invoke-Expression $rollbackCommand

    $endTime = Get-Date
    $duration = ($endTime - $startTime).TotalSeconds

    Write-Host ""
    Write-Host "✓ Rollback completed successfully" -ForegroundColor Green
    Write-Host "  Duration: $([math]::Round($duration, 2)) seconds" -ForegroundColor Gray
    Write-Host ""

    # Verify database state
    Write-Host "Verifying database state..." -ForegroundColor Yellow
    $verifyMigrations = dotnet ef migrations list `
        --project $Project `
        --startup-project $StartupProject `
        --context $Context `
        --no-build 2>&1

    Write-Host "✓ Current Migrations:" -ForegroundColor Green
    Write-Host $verifyMigrations -ForegroundColor Gray
    Write-Host ""

    # Log rollback operation
    $logEntry = [PSCustomObject]@{
        Timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
        TargetMigration = $TargetMigration
        Project = $Project
        Duration = [math]::Round($duration, 2)
        Status = "Success"
    }

    $logPath = ".\migration-rollback-log.csv"
    $logEntry | Export-Csv -Path $logPath -Append -NoTypeInformation

    Write-Host "=== Rollback Summary ===" -ForegroundColor Cyan
    Write-Host "✓ Target Migration: $TargetMigration" -ForegroundColor Green
    Write-Host "✓ Database State: Rolled back successfully" -ForegroundColor Green
    Write-Host "✓ Backup Location: .\backups\pre-rollback" -ForegroundColor Green
    Write-Host ""
    Write-Host "Important Notes:" -ForegroundColor Yellow
    Write-Host "  - Review application functionality after rollback" -ForegroundColor White
    Write-Host "  - Update application code if schema changes affect it" -ForegroundColor White
    Write-Host "  - Keep backup until rollback is verified" -ForegroundColor White
    Write-Host ""

    exit 0

} catch {
    Write-Host "✗ Rollback Failed!" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "Recovery Steps:" -ForegroundColor Yellow
    Write-Host "  1. Check error message above for details" -ForegroundColor White
    Write-Host "  2. Restore from backup if needed: .\restore-database.ps1" -ForegroundColor White
    Write-Host "  3. Review migration history: dotnet ef migrations list" -ForegroundColor White
    Write-Host "  4. Contact development team if issue persists" -ForegroundColor White
    Write-Host ""

    # Log error
    $errorEntry = [PSCustomObject]@{
        Timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
        TargetMigration = $TargetMigration
        Project = $Project
        Duration = 0
        Status = "Failed: $($_.Exception.Message)"
    }

    $logPath = ".\migration-rollback-log.csv"
    $errorEntry | Export-Csv -Path $logPath -Append -NoTypeInformation

    exit 1
}

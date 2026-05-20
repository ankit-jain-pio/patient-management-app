# Monitoring and Logging Guide
## Patient Management Application

**Version:** 1.0  
**Last Updated:** May 20, 2026  
**Status:** Production

---

## Table of Contents

1. [Overview](#overview)
2. [Logging Architecture](#logging-architecture)
3. [Log Management](#log-management)
4. [Monitoring Metrics](#monitoring-metrics)
5. [Health Checks](#health-checks)
6. [Alert Configuration](#alert-configuration)
7. [Troubleshooting with Logs](#troubleshooting-with-logs)
8. [Best Practices](#best-practices)

---

## Overview

This guide covers the monitoring and logging infrastructure for the Patient Management Application, including log configuration, metrics collection, health monitoring, and alerting.

### Monitoring Goals

- **Proactive Issue Detection**: Identify problems before users report them
- **Performance Optimization**: Track and improve system performance
- **Security Monitoring**: Detect and respond to security incidents
- **Compliance**: Maintain audit trails for HIPAA compliance
- **Troubleshooting**: Quick problem diagnosis and resolution

### Key Components

```
┌─────────────────────────────────────────┐
│         Application Logs                │
│     (Serilog → File + Console)          │
└──────────────┬──────────────────────────┘
               │
               ├─► Docker Logs (stdout/stderr)
               ├─► File System (/app/logs)
               └─► Log Aggregation (optional: ELK, Splunk)
```

---

## Logging Architecture

### Logging Stack

| Component | Technology | Purpose |
|-----------|-----------|---------|
| **Logging Framework** | Serilog | Structured logging in .NET |
| **Console Sink** | Serilog.Sinks.Console | Development/Docker logs |
| **File Sink** | Serilog.Sinks.File | Persistent log storage |
| **Log Format** | JSON | Machine-readable structured logs |
| **Log Rotation** | Daily | Automatic log file management |
| **Retention** | 30 days | Configurable retention policy |

### Log Levels

| Level | Usage | Examples |
|-------|-------|----------|
| **Verbose** | Detailed trace | "Entering method X with parameter Y" |
| **Debug** | Development info | "Executing SQL query: SELECT..." |
| **Information** | Normal operations | "User logged in", "Patient created" |
| **Warning** | Potential issues | "High memory usage", "Slow query" |
| **Error** | Failures | "Failed to save consultation", "Database timeout" |
| **Fatal** | Critical failures | "Cannot start application", "Database unavailable" |

### Structured Logging Format

**Console Output** (Human-readable):
```
[2026-05-20 14:30:45.123 +05:30] [INF] User authenticated successfully
[2026-05-20 14:30:46.456 +05:30] [INF] Patient created with ID: P-12345
[2026-05-20 14:30:47.789 +05:30] [WRN] Database query took 1.5 seconds
[2026-05-20 14:30:48.012 +05:30] [ERR] Failed to send email notification
```

**File Output** (JSON structured):
```json
{
  "Timestamp": "2026-05-20T14:30:45.123+05:30",
  "Level": "Information",
  "MessageTemplate": "User authenticated successfully",
  "Properties": {
    "UserId": "user123",
    "SourceContext": "PatientManagement.API.Controllers.AuthController",
    "RequestPath": "/api/v1/auth/login",
    "RequestMethod": "POST",
    "StatusCode": 200,
    "Duration": 234
  }
}
```

---

## Log Management

### Log Location

**Docker Deployment**:
```
/app/logs/
├── patient-management-20260520.log
├── patient-management-20260519.log
├── patient-management-20260518.log
└── ...
```

**Host System**:
```
./logs/
├── patient-management-20260520.log
├── patient-management-20260519.log
└── ...
```

### Accessing Logs

#### Real-time Logs (Docker)

```bash
# All services
docker-compose logs -f

# API only
docker-compose logs -f api

# Last 100 lines
docker-compose logs --tail=100 api

# Since specific time
docker-compose logs --since="2026-05-20T14:00:00" api
```

#### File-based Logs

```bash
# View current log
tail -f ./logs/patient-management-$(date +%Y%m%d).log

# Search logs
grep "ERROR" ./logs/patient-management-20260520.log

# View with less
less ./logs/patient-management-20260520.log
```

#### PowerShell Log Analysis

```powershell
# View logs
Get-Content .\logs\patient-management-20260520.log -Tail 50 -Wait

# Search for errors
Select-String -Path ".\logs\patient-management-*.log" -Pattern "ERROR|FATAL"

# Parse JSON logs
Get-Content .\logs\patient-management-20260520.log | ConvertFrom-Json | 
    Where-Object { $_.Level -eq "Error" } | 
    Format-Table Timestamp, MessageTemplate

# Count by log level
Get-Content .\logs\patient-management-20260520.log | ConvertFrom-Json | 
    Group-Object Level | 
    Select-Object Name, Count
```

### Log Rotation

**Automated Rotation** (configured in appsettings.json):
```json
{
  "Serilog": {
    "WriteTo": [
      {
        "Name": "File",
        "Args": {
          "path": "/app/logs/patient-management-.log",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 30,
          "fileSizeLimitBytes": 104857600,
          "rollOnFileSizeLimit": true
        }
      }
    ]
  }
}
```

**Configuration Details**:
- **Rolling Interval**: Daily (new file each day)
- **Retention**: 30 days (older files deleted automatically)
- **File Size Limit**: 100 MB per file
- **Roll on Size**: Create new file when size limit reached

### Manual Log Cleanup

```bash
# Delete logs older than 30 days
find ./logs -name "*.log" -mtime +30 -delete

# Compress old logs
gzip ./logs/patient-management-2026051*.log

# Archive to backup
tar -czf logs-archive-$(date +%Y%m).tar.gz ./logs/*.log
```

---

## Monitoring Metrics

### Key Performance Indicators (KPIs)

#### Application Metrics

| Metric | Target | Warning | Critical | Monitoring Method |
|--------|--------|---------|----------|-------------------|
| **API Response Time** | <500ms | >1s | >2s | Logs + Health Check |
| **Database Query Time** | <200ms | >1s | >2s | Logs |
| **Request Throughput** | - | - | - | Logs |
| **Error Rate** | <0.1% | >1% | >5% | Logs |
| **CPU Usage** | <60% | >80% | >90% | Docker Stats |
| **Memory Usage** | <2GB | >4GB | >6GB | Docker Stats |
| **Disk Space** | >10GB | <5GB | <1GB | Health Check |

#### Business Metrics

| Metric | Target | Monitoring Method |
|--------|--------|-------------------|
| **Consultations per Day** | - | Database Query |
| **Active Patients** | - | Database Query |
| **Average Consultation Time** | <3min | Logs |
| **Prescription Generation Time** | <2s | Logs |
| **Failed Login Attempts** | <5/day | Logs |

### Collecting Metrics

#### Docker Stats

```bash
# Real-time stats
docker stats

# Sample output:
CONTAINER ID   NAME                     CPU %   MEM USAGE / LIMIT   MEM %
abc123         patient-management-api   12.5%   512MB / 8GB         6.4%
def456         patient-management-db    8.2%    1.2GB / 8GB         15%
```

#### Custom Metrics Script

```powershell
# daily-metrics.ps1
$date = Get-Date
$logFile = ".\logs\patient-management-$($date.ToString('yyyyMMdd')).log"

# Parse logs for metrics
$logs = Get-Content $logFile | ConvertFrom-Json

# Count by log level
$errorCount = ($logs | Where-Object { $_.Level -eq "Error" }).Count
$warningCount = ($logs | Where-Object { $_.Level -eq "Warning" }).Count
$infoCount = ($logs | Where-Object { $_.Level -eq "Information" }).Count

# Average response times
$apiRequests = $logs | Where-Object { $_.Properties.Duration -ne $null }
$avgResponseTime = ($apiRequests | Measure-Object -Property Properties.Duration -Average).Average

# Output report
Write-Host "=== Daily Metrics - $($date.ToShortDateString()) ==="
Write-Host "Total Errors: $errorCount"
Write-Host "Total Warnings: $warningCount"
Write-Host "Total Info: $infoCount"
Write-Host "Avg Response Time: $([math]::Round($avgResponseTime, 2))ms"
Write-Host "Total API Requests: $($apiRequests.Count)"
```

---

## Health Checks

### Health Check Endpoint

**URL**: `http://localhost:5000/health`

**Response** (Healthy):
```json
{
  "status": "Healthy",
  "checks": {
    "database": "Healthy",
    "diskSpace": "Healthy"
  },
  "duration": "00:00:00.123",
  "timestamp": "2026-05-20T14:30:45Z"
}
```

**Response** (Unhealthy):
```json
{
  "status": "Unhealthy",
  "checks": {
    "database": "Unhealthy",
    "diskSpace": "Healthy"
  },
  "duration": "00:00:05.000",
  "timestamp": "2026-05-20T14:30:45Z",
  "error": "Database connection timeout"
}
```

### Health Check Components

1. **Database Connectivity**
   - Verifies connection to SQL Server
   - Executes simple query: `SELECT 1`
   - Timeout: 10 seconds

2. **Disk Space**
   - Checks available disk space
   - Alert if <1GB free
   - Critical if <500MB free

3. **Application Status**
   - API responsiveness
   - Memory usage
   - Thread pool availability

### Automated Health Monitoring

**Docker Compose Health Check**:
```yaml
healthcheck:
  test: curl -f http://localhost:80/health || exit 1
  interval: 30s
  timeout: 10s
  start_period: 60s
  retries: 3
```

**Monitoring Script**:
```powershell
# health-monitor.ps1
$endpoint = "http://localhost:5000/health"
$interval = 60  # seconds

while ($true) {
    try {
        $response = Invoke-RestMethod -Uri $endpoint -TimeoutSec 10
        
        if ($response.status -eq "Healthy") {
            Write-Host "[$(Get-Date)] ✓ System Healthy" -ForegroundColor Green
        } else {
            Write-Host "[$(Get-Date)] ✗ System Unhealthy: $($response.error)" -ForegroundColor Red
            # Send alert email or notification
        }
    } catch {
        Write-Host "[$(Get-Date)] ✗ Health Check Failed: $($_.Exception.Message)" -ForegroundColor Red
    }
    
    Start-Sleep -Seconds $interval
}
```

---

## Alert Configuration

### Alert Scenarios

| Scenario | Trigger | Action | Priority |
|----------|---------|--------|----------|
| **Application Down** | Health check fails 3x | Email + SMS | Critical |
| **Database Unavailable** | DB check fails | Email | Critical |
| **High Error Rate** | >10 errors/hour | Email | High |
| **Disk Space Low** | <1GB free | Email | High |
| **High Memory** | >80% usage | Log | Medium |
| **Slow Response** | Avg >2s | Log | Medium |

### Email Alerts (PowerShell)

```powershell
# send-alert.ps1
param(
    [string]$Subject,
    [string]$Body,
    [string]$Priority = "High"
)

$smtpServer = "smtp.example.com"
$smtpPort = 587
$from = "alerts@clinic.com"
$to = "admin@clinic.com"
$username = $env:SMTP_USER
$password = ConvertTo-SecureString $env:SMTP_PASSWORD -AsPlainText -Force
$credential = New-Object System.Management.Automation.PSCredential($username, $password)

$message = @{
    From = $from
    To = $to
    Subject = $Subject
    Body = $Body
    SmtpServer = $smtpServer
    Port = $smtpPort
    UseSsl = $true
    Credential = $credential
    Priority = $Priority
}

Send-MailMessage @message

Write-Host "Alert sent: $Subject"
```

### Alert Workflow

```bash
# check-and-alert.sh
#!/bin/bash

HEALTH_URL="http://localhost:5000/health"
ALERT_EMAIL="admin@clinic.com"

# Check health
response=$(curl -s -o /dev/null -w "%{http_code}" "$HEALTH_URL")

if [ $response -ne 200 ]; then
    # Send alert
    echo "CRITICAL: Application health check failed (HTTP $response)" | \
        mail -s "Patient Management System Alert" "$ALERT_EMAIL"
    
    # Log to syslog
    logger -p user.crit "Patient Management System health check failed"
fi
```

**Cron Job** (check every 5 minutes):
```cron
*/5 * * * * /opt/patient-management/scripts/check-and-alert.sh
```

---

## Troubleshooting with Logs

### Common Issues and Log Patterns

#### Issue 1: Application Won't Start

**Log Pattern**:
```
[FATAL] An error occurred while starting the application
[FATAL] Unable to start Kestrel
```

**Diagnosis**:
```powershell
# Check recent fatal errors
Get-Content .\logs\patient-management-*.log | 
    ConvertFrom-Json | 
    Where-Object { $_.Level -eq "Fatal" } | 
    Select-Object -Last 10
```

**Common Causes**:
- Port already in use
- Database connection failed
- Configuration error

#### Issue 2: Slow Performance

**Log Pattern**:
```
[WRN] Database query took 2.5 seconds
[WRN] API request exceeded 2 second threshold
```

**Diagnosis**:
```powershell
# Find slow queries
Get-Content .\logs\patient-management-20260520.log | 
    ConvertFrom-Json | 
    Where-Object { $_.Properties.Duration -gt 1000 } | 
    Sort-Object { $_.Properties.Duration } -Descending
```

#### Issue 3: Authentication Failures

**Log Pattern**:
```
[WRN] Failed login attempt for user: admin
[ERR] JWT token validation failed
```

**Diagnosis**:
```powershell
# Count failed logins
Get-Content .\logs\patient-management-*.log | 
    ConvertFrom-Json | 
    Where-Object { $_.MessageTemplate -like "*Failed login*" } | 
    Group-Object { $_.Properties.Username } | 
    Sort-Object Count -Descending
```

#### Issue 4: Database Connection Issues

**Log Pattern**:
```
[ERR] SqlException: Cannot open database
[ERR] Connection timeout expired
```

**Diagnosis**:
```bash
# Check database logs
docker-compose logs db | grep -i error

# Test connection
sqlcmd -S localhost -U sa -Q "SELECT @@VERSION"
```

### Log Analysis Tools

#### PowerShell Log Parser

```powershell
# analyze-logs.ps1
param(
    [string]$LogPath = ".\logs\patient-management-*.log",
    [string]$Level = "Error",
    [int]$Last = 50
)

Get-Content $LogPath | 
    ConvertFrom-Json | 
    Where-Object { $_.Level -eq $Level } | 
    Select-Object -Last $Last | 
    Format-Table Timestamp, 
                 @{Name="Message"; Expression={$_.MessageTemplate}}, 
                 @{Name="Context"; Expression={$_.Properties.SourceContext}} `
                 -AutoSize
```

**Usage**:
```powershell
# Show last 50 errors
.\analyze-logs.ps1 -Level Error

# Show last 20 warnings
.\analyze-logs.ps1 -Level Warning -Last 20
```

---

## Best Practices

### Logging Best Practices

✅ **DO**:
- Log all significant events (user actions, errors, warnings)
- Use structured logging (key-value pairs)
- Include context (user ID, request ID, timestamp)
- Log at appropriate levels
- Sanitize sensitive data (passwords, tokens)
- Rotate logs regularly
- Monitor disk space

❌ **DON'T**:
- Log sensitive patient information in plain text
- Use excessive Debug/Verbose logging in production
- Log full stack traces for expected errors
- Ignore log errors (disk full, write failures)
- Store logs indefinitely without archiving

### Performance Considerations

**Asynchronous Logging**:
- Serilog writes asynchronously by default
- Minimal performance impact
- Buffer size configurable

**Log Level in Production**:
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    }
  }
}
```

### Security Considerations

**Sensitive Data Handling**:
```csharp
// DO NOT log sensitive data
Log.Information("User logged in: {UserId}", userId);  // ✓ Good

// NEVER log passwords, tokens, PHI
Log.Information("Login attempt: {Password}", password);  // ✗ Bad
Log.Information("Patient SSN: {SSN}", ssn);  // ✗ Bad
```

**Log Access Control**:
- Restrict log file permissions: `chmod 600 *.log`
- Limit user access to log directories
- Use encryption for log storage
- Implement log retention policies

### Compliance

**HIPAA Audit Requirements**:
- Log all access to patient records
- Include user ID, timestamp, action
- Retain logs for minimum 6 years
- Protect logs from tampering
- Regular audit log reviews

**Example Audit Log**:
```json
{
  "Timestamp": "2026-05-20T14:30:45Z",
  "Level": "Information",
  "MessageTemplate": "Patient record accessed",
  "Properties": {
    "UserId": "dr.smith",
    "PatientId": "P-12345",
    "Action": "View",
    "IpAddress": "192.168.1.100"
  }
}
```

---

## Monitoring Dashboard (Optional)

### Using Grafana + Prometheus

**Architecture**:
```
Application (Serilog)
    ↓
Prometheus Exporter
    ↓
Prometheus (Time-series DB)
    ↓
Grafana (Visualization)
```

**Key Metrics to Track**:
- Request rate (requests/second)
- Response time (p50, p95, p99)
- Error rate (%)
- Active connections
- Database query time
- Memory/CPU usage

### Using ELK Stack

**Architecture**:
```
Application (Serilog)
    ↓
Filebeat (Log Shipper)
    ↓
Logstash (Processing)
    ↓
Elasticsearch (Storage)
    ↓
Kibana (Visualization)
```

**Benefits**:
- Centralized log management
- Full-text search
- Real-time analysis
- Custom dashboards
- Alerting capabilities

---

## Maintenance Tasks

### Daily
- [ ] Review error logs
- [ ] Check health endpoint
- [ ] Verify backup logs
- [ ] Monitor disk space

### Weekly
- [ ] Analyze performance trends
- [ ] Review security logs
- [ ] Check alert configuration
- [ ] Test health checks

### Monthly
- [ ] Archive old logs
- [ ] Review retention policies
- [ ] Update monitoring thresholds
- [ ] Generate metrics reports

### Quarterly
- [ ] Audit log access
- [ ] Review compliance logs
- [ ] Update monitoring documentation
- [ ] Test disaster recovery logs

---

## References

- [Serilog Documentation](https://serilog.net/)
- [Docker Logging](https://docs.docker.com/config/containers/logging/)
- [Deployment Guide](./DEPLOYMENT_GUIDE.md)
- [Disaster Recovery Plan](./DISASTER_RECOVERY.md)

---

**Document Version:** 1.0  
**Last Updated:** May 20, 2026  
**Next Review:** August 20, 2026

# Deployment Guide
## Patient Management Application

**Version:** 1.0  
**Last Updated:** May 20, 2026  
**Environment:** Production  

---

## Table of Contents

1. [Overview](#overview)
2. [System Requirements](#system-requirements)
3. [Pre-Deployment Checklist](#pre-deployment-checklist)
4. [Deployment Methods](#deployment-methods)
5. [Configuration](#configuration)
6. [Health Checks](#health-checks)
7. [Rollback Procedures](#rollback-procedures)
8. [Post-Deployment](#post-deployment)
9. [Troubleshooting](#troubleshooting)

---

## Overview

This guide covers the deployment of the Patient Management Application using Docker containers. The application consists of three main components:

- **Backend API**: .NET 8 ASP.NET Core Web API
- **Frontend**: React 18+ Single Page Application
- **Database**: SQL Server 2022 Express

### Architecture

```
┌─────────────────┐
│   Nginx (80)    │ ← React Frontend
└────────┬────────┘
         │
         ├─────────► API (5000/5001) ← .NET Backend
         │
         └─────────► Database (1433) ← SQL Server
```

---

## System Requirements

### Hardware Requirements

| Component | Minimum | Recommended |
|-----------|---------|-------------|
| **CPU** | 2 cores | 4 cores |
| **RAM** | 4 GB | 8 GB |
| **Storage** | 20 GB SSD | 50 GB SSD |
| **Network** | 10 Mbps | 100 Mbps |

### Software Requirements

- **Operating System**: Windows Server 2019+, Ubuntu 20.04+, or Docker-compatible OS
- **Docker**: Version 20.10+ with Docker Compose v2.0+
- **.NET Runtime**: 8.0 (included in container)
- **SQL Server**: 2022 Express or higher
- **SSL Certificate**: For HTTPS (optional but recommended)

### Network Requirements

- **Inbound Ports**:
  - `80` (HTTP) - Frontend
  - `443` (HTTPS) - Frontend (SSL)
  - `5000` (HTTP) - API
  - `5001` (HTTPS) - API (SSL)
  - `1433` - Database (if external access needed)

- **Outbound Ports**:
  - `443` - For package downloads and updates
  - `80` - For health checks

---

## Pre-Deployment Checklist

### 1. Environment Preparation

- [ ] Server/VM provisioned with minimum requirements
- [ ] Docker and Docker Compose installed
- [ ] Firewall rules configured for required ports
- [ ] SSL certificates obtained (if using HTTPS)
- [ ] DNS records configured

### 2. Code Repository

- [ ] Latest code pulled from `master` branch
- [ ] All tests passing in CI/CD pipeline
- [ ] Version tag created (e.g., `v1.0.0`)

### 3. Configuration Files

- [ ] `.env.production` file created (copy from `.env.production.example`)
- [ ] Database connection string configured
- [ ] JWT secret generated (minimum 32 characters)
- [ ] CORS origins configured
- [ ] SMTP settings configured (for alerts)

### 4. Database

- [ ] Database server accessible
- [ ] SA password set and secured
- [ ] Backup storage configured
- [ ] Migration scripts reviewed

### 5. Monitoring & Logging

- [ ] Log directory created with appropriate permissions
- [ ] Backup directory created
- [ ] Monitoring tools configured (optional)

---

## Deployment Methods

### Method 1: Docker Compose (Recommended)

This method deploys all components using Docker Compose.

#### Step 1: Clone Repository

```bash
cd /opt
git clone https://github.com/your-org/patient-management-app.git
cd patient-management-app
git checkout tags/v1.0.0
```

#### Step 2: Configure Environment

```bash
# Copy and edit environment file
cp .env.production.example .env.production
nano .env.production
```

**Required Settings**:
```env
DB_SA_PASSWORD=YourStrongPassword123!
JWT_SECRET=YourJwtSecretKeyAtLeast32CharactersLong!
CLINIC_NAME=Your Clinic Name
CLINIC_ADDRESS=Your Clinic Address
CLINIC_PHONE=+1234567890
CORS_ORIGINS=https://yourdomain.com
```

#### Step 3: Build Frontend

```bash
cd client
npm ci
npm run build
cd ..
```

#### Step 4: Deploy Services

```bash
# Load environment variables
export $(cat .env.production | xargs)

# Start all services
docker-compose up -d

# Check service status
docker-compose ps
```

#### Step 5: Verify Deployment

```bash
# Check logs
docker-compose logs -f api

# Health check
curl http://localhost:5000/health

# Test frontend
curl http://localhost:3000
```

### Method 2: Manual Deployment

#### Backend API

```bash
# Publish application
cd src/PatientManagement.API
dotnet publish -c Release -o /opt/patient-management/api

# Configure service
sudo nano /etc/systemd/system/patient-management-api.service
```

**Service File** (`patient-management-api.service`):
```ini
[Unit]
Description=Patient Management API
After=network.target

[Service]
Type=notify
User=www-data
WorkingDirectory=/opt/patient-management/api
ExecStart=/usr/bin/dotnet /opt/patient-management/api/PatientManagement.API.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=patient-management-api
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```

```bash
# Start service
sudo systemctl daemon-reload
sudo systemctl enable patient-management-api
sudo systemctl start patient-management-api
sudo systemctl status patient-management-api
```

#### Frontend

```bash
# Build frontend
cd client
npm ci
npm run build

# Copy to web server
sudo cp -r dist/* /var/www/patient-management/

# Configure Nginx
sudo nano /etc/nginx/sites-available/patient-management
```

**Nginx Configuration**:
```nginx
server {
    listen 80;
    server_name yourdomain.com;
    
    root /var/www/patient-management;
    index index.html;
    
    location / {
        try_files $uri $uri/ /index.html;
    }
    
    location /api/ {
        proxy_pass http://localhost:5000/api/;
        proxy_http_version 1.1;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
    }
}
```

```bash
# Enable site
sudo ln -s /etc/nginx/sites-available/patient-management /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl reload nginx
```

---

## Configuration

### Database Configuration

#### Initial Setup

```sql
-- Connect to SQL Server
USE master;
GO

-- Create database
CREATE DATABASE PatientManagementDb;
GO

-- Create application user (recommended for production)
CREATE LOGIN PatientManagementUser WITH PASSWORD = 'SecurePassword123!';
CREATE USER PatientManagementUser FOR LOGIN PatientManagementUser;
ALTER ROLE db_owner ADD MEMBER PatientManagementUser;
GO
```

#### Apply Migrations

```bash
# Automated (Docker Compose)
docker-compose run --rm api dotnet ef database update \
    --project /src/src/PatientManagement.Infrastructure \
    --startup-project /src/src/PatientManagement.API

# Manual
cd src/PatientManagement.API
dotnet ef database update \
    --project ../PatientManagement.Infrastructure \
    --connection "Server=localhost;Database=PatientManagementDb;..."
```

### Application Configuration

#### JWT Settings

Generate secure JWT secret:
```bash
# Linux/Mac
openssl rand -base64 32

# PowerShell
-join ((48..57) + (65..90) + (97..122) | Get-Random -Count 32 | % {[char]$_})
```

#### CORS Configuration

Update `appsettings.Production.json`:
```json
{
  "CorsOrigins": [
    "https://yourdomain.com",
    "https://www.yourdomain.com"
  ]
}
```

#### SSL/HTTPS Configuration

For Docker Compose with SSL:
```yaml
# docker-compose.yml
api:
  ports:
    - "443:443"
  environment:
    - ASPNETCORE_URLS=https://+:443;http://+:80
    - ASPNETCORE_Kestrel__Certificates__Default__Path=/https/certificate.pfx
    - ASPNETCORE_Kestrel__Certificates__Default__Password=CertPassword
  volumes:
    - ./certs:/https:ro
```

---

## Health Checks

### API Health Endpoint

```bash
# Health check
curl http://localhost:5000/health

# Expected response
{
  "status": "Healthy",
  "checks": {
    "database": "Healthy",
    "diskSpace": "Healthy"
  },
  "duration": "00:00:00.123"
}
```

### Database Health Check

```bash
# Via Docker
docker-compose exec db /opt/mssql-tools/bin/sqlcmd \
    -S localhost -U sa -P "$DB_SA_PASSWORD" \
    -Q "SELECT @@VERSION"

# Direct connection
sqlcmd -S localhost -U sa -P "Password" -Q "SELECT DB_NAME()"
```

### Service Status

```bash
# Docker Compose
docker-compose ps

# Expected output:
# NAME                     STATUS
# patient-management-api   Up (healthy)
# patient-management-db    Up (healthy)
# patient-management-frontend   Up
# patient-management-backup     Up
```

---

## Rollback Procedures

### Scenario 1: Rollback to Previous Version

```bash
# Stop current deployment
docker-compose down

# Checkout previous version
git fetch --tags
git checkout tags/v0.9.0

# Rebuild and redeploy
docker-compose build
docker-compose up -d

# Rollback database migrations
.\scripts\migration-rollback.ps1 -TargetMigration PreviousMigration
```

### Scenario 2: Emergency Database Restore

```bash
# Stop application
docker-compose down

# Restore database
.\scripts\restore-database.ps1 \
    -BackupFile ".\backups\PatientManagementDb_20240520_020000.bak" \
    -Force

# Restart application
docker-compose up -d
```

### Scenario 3: Complete Environment Reset

```bash
# ⚠️ WARNING: This will delete all data!

# Stop and remove containers
docker-compose down -v

# Remove images
docker-compose down --rmi all

# Clean build and redeploy
git pull origin master
docker-compose build --no-cache
docker-compose up -d
```

---

## Post-Deployment

### 1. Smoke Tests

```bash
# Test API health
curl http://localhost:5000/health

# Test authentication
curl -X POST http://localhost:5000/api/v1/auth/login \
    -H "Content-Type: application/json" \
    -d '{"username":"admin","password":"yourpassword"}'

# Test patient search
curl http://localhost:5000/api/v1/patients \
    -H "Authorization: Bearer YOUR_TOKEN"
```

### 2. Monitor Logs

```bash
# API logs
docker-compose logs -f api

# Database logs
docker-compose logs -f db

# All services
docker-compose logs -f
```

### 3. Verify Backups

```bash
# Check backup directory
ls -lh ./backups

# Test backup script
.\scripts\backup-database.ps1 -Verify

# Verify latest backup
ls -lt ./backups | head -n 1
```

### 4. Performance Monitoring

- Monitor CPU and memory usage: `docker stats`
- Check disk space: `df -h`
- Review application logs for errors
- Test critical workflows (patient registration, appointments)

---

## Troubleshooting

### Issue 1: API Container Won't Start

**Symptoms**:
```
Error: Unable to start Kestrel
```

**Solution**:
```bash
# Check logs
docker-compose logs api

# Verify environment variables
docker-compose config

# Check port conflicts
netstat -tuln | grep 5000

# Restart services
docker-compose restart api
```

### Issue 2: Database Connection Failed

**Symptoms**:
```
SqlException: Cannot open database
```

**Solution**:
```bash
# Verify database container is running
docker-compose ps db

# Test database connection
docker-compose exec db /opt/mssql-tools/bin/sqlcmd \
    -S localhost -U sa -P "$DB_SA_PASSWORD" -Q "SELECT 1"

# Check connection string in appsettings.Production.json
docker-compose exec api cat /app/appsettings.Production.json

# Restart database
docker-compose restart db
```

### Issue 3: Frontend Returns 404

**Symptoms**:
- React app shows blank page
- Browser console shows 404 errors

**Solution**:
```bash
# Verify frontend build
ls -lh client/dist

# Rebuild frontend
cd client && npm run build

# Check Nginx configuration
docker-compose exec frontend nginx -t

# Restart frontend
docker-compose restart frontend
```

### Issue 4: Backup Fails

**Symptoms**:
```
Error: Cannot write to backup directory
```

**Solution**:
```bash
# Check directory permissions
ls -ld ./backups

# Create backup directory
mkdir -p ./backups
chmod 777 ./backups

# Test backup manually
.\scripts\backup-database.ps1
```

### Issue 5: High Memory Usage

**Symptoms**:
- API container using >2GB RAM
- Server performance degraded

**Solution**:
```bash
# Check container stats
docker stats

# Limit container memory in docker-compose.yml
services:
  api:
    deploy:
      resources:
        limits:
          memory: 1G

# Restart with limits
docker-compose up -d --force-recreate
```

---

## Security Considerations

### 1. Production Secrets

- **Never commit** `.env.production` to source control
- Use secure password manager for credentials
- Rotate JWT secret every 90 days
- Use strong database passwords (16+ characters)

### 2. Network Security

- Configure firewall to block direct database access
- Use HTTPS for all external connections
- Implement rate limiting on API endpoints
- Enable audit logging

### 3. Database Security

```sql
-- Enable transparent data encryption
ALTER DATABASE PatientManagementDb
SET ENCRYPTION ON;

-- Configure backup encryption
BACKUP DATABASE PatientManagementDb 
TO DISK = 'backup.bak'
WITH ENCRYPTION;
```

### 4. Application Security

- Keep Docker images updated
- Scan images for vulnerabilities: `docker scan patient-management-api`
- Enable Content Security Policy (CSP) headers
- Regular security audits

---

## Maintenance Schedule

| Task | Frequency | Owner |
|------|-----------|-------|
| Database Backup | Daily 2:00 AM | Automated |
| Log Rotation | Daily | Automated |
| Security Updates | Weekly | DevOps |
| Database Optimization | Monthly | DBA |
| Backup Testing | Monthly | DevOps |
| Disaster Recovery Drill | Quarterly | Team |
| Certificate Renewal | Yearly | DevOps |

---

## Support Contacts

- **DevOps Team**: devops@yourdomain.com
- **Database Admin**: dba@yourdomain.com
- **Development Team**: dev@yourdomain.com
- **24/7 Emergency**: +1-XXX-XXX-XXXX

---

## Appendix: Quick Reference Commands

```bash
# Start all services
docker-compose up -d

# Stop all services
docker-compose down

# View logs
docker-compose logs -f [service-name]

# Restart service
docker-compose restart [service-name]

# Update services
git pull origin master
docker-compose pull
docker-compose up -d

# Create backup
.\scripts\backup-database.ps1

# Restore backup
.\scripts\restore-database.ps1 -BackupFile path/to/backup.bak

# Apply migrations
docker-compose run --rm api dotnet ef database update

# Health check
curl http://localhost:5000/health

# Container stats
docker stats

# Clean up old images
docker image prune -a
```

---

**Document Version:** 1.0  
**Last Updated:** May 20, 2026  
**Next Review:** August 20, 2026

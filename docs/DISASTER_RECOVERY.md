# Disaster Recovery Plan
## Patient Management Application

**Version:** 1.0  
**Effective Date:** May 20, 2026  
**Review Schedule:** Quarterly  
**Classification:** CONFIDENTIAL

---

## Table of Contents

1. [Executive Summary](#executive-summary)
2. [Disaster Recovery Objectives](#disaster-recovery-objectives)
3. [Risk Assessment](#risk-assessment)
4. [Backup Strategy](#backup-strategy)
5. [Recovery Procedures](#recovery-procedures)
6. [Testing & Maintenance](#testing--maintenance)
7. [Roles & Responsibilities](#roles--responsibilities)
8. [Emergency Contacts](#emergency-contacts)

---

## Executive Summary

This Disaster Recovery Plan (DRP) outlines procedures for recovering the Patient Management Application in the event of system failure, data loss, or disaster. The plan ensures business continuity and protects patient data integrity.

### Critical Success Factors

- **Zero Data Loss**: All patient records must be recoverable
- **Minimal Downtime**: Maximum acceptable downtime is 4 hours
- **Data Integrity**: All recovered data must be accurate and complete
- **Compliance**: HIPAA compliance maintained during recovery

### Plan Scope

**In Scope**:
- Application server failure
- Database corruption or loss
- Hardware failure
- Natural disasters affecting data center
- Cyber security incidents (ransomware, data breach)
- Accidental data deletion

**Out of Scope**:
- Individual user account issues
- Minor application bugs
- Network connectivity problems (unless widespread)
- End-user device failures

---

## Disaster Recovery Objectives

### Recovery Time Objective (RTO)

**Maximum acceptable downtime**: 4 hours

| System Component | RTO Target | Priority |
|------------------|------------|----------|
| **Database** | 1 hour | Critical |
| **API Backend** | 2 hours | Critical |
| **Frontend** | 1 hour | High |
| **Backup System** | 4 hours | Medium |

### Recovery Point Objective (RPO)

**Maximum acceptable data loss**: 24 hours

| Data Type | RPO Target | Backup Frequency |
|-----------|------------|------------------|
| **Patient Records** | 0 hours | Continuous (transaction log) |
| **Consultations** | 1 hour | Hourly |
| **System Configuration** | 24 hours | Daily |
| **Application Code** | N/A | Git repository |

### Service Level Objectives

- **Availability**: 99.5% uptime (excluding planned maintenance)
- **Data Integrity**: 100% accuracy
- **Backup Success Rate**: 100% of scheduled backups
- **Recovery Success Rate**: >95% within RTO

---

## Risk Assessment

### Identified Risks

#### 1. Hardware Failure

**Probability**: Medium  
**Impact**: High  
**Mitigation**:
- Redundant server infrastructure
- RAID storage configuration
- Regular hardware monitoring
- Spare parts inventory

#### 2. Database Corruption

**Probability**: Low  
**Impact**: Critical  
**Mitigation**:
- Daily full backups
- Hourly differential backups
- Transaction log backups (continuous)
- Regular integrity checks (DBCC CHECKDB)

#### 3. Ransomware Attack

**Probability**: Medium  
**Impact**: Critical  
**Mitigation**:
- Offline backup copies
- Immutable backup storage
- Network segmentation
- Regular security updates
- Anti-malware software

#### 4. Natural Disaster

**Probability**: Low  
**Impact**: Critical  
**Mitigation**:
- Off-site backup storage
- Cloud backup replication
- Geographically distributed backups
- Alternative hosting site ready

#### 5. Human Error

**Probability**: Medium  
**Impact**: Medium  
**Mitigation**:
- User access controls
- Audit logging
- Change management process
- Backup verification

#### 6. Power Outage

**Probability**: Medium  
**Impact**: Medium  
**Mitigation**:
- UPS (Uninterruptible Power Supply)
- Generator backup
- Graceful shutdown procedures
- Auto-restart configurations

---

## Backup Strategy

### Backup Schedule

#### Database Backups

```
┌─────────────┬──────────────┬────────────┬──────────────┐
│ Backup Type │ Frequency    │ Retention  │ Storage      │
├─────────────┼──────────────┼────────────┼──────────────┤
│ Full        │ Daily 2:00AM │ 30 days    │ Local + Cloud│
│ Differential│ Every 6 hours│ 7 days     │ Local        │
│ Transaction │ Every 15 min │ 24 hours   │ Local        │
│ Log         │              │            │              │
└─────────────┴──────────────┴────────────┴──────────────┘
```

**Automated Script**: `scripts/backup-database.ps1`

**Backup Locations**:
1. **Primary**: `/app/backups` (local server)
2. **Secondary**: Network-attached storage (NAS)
3. **Tertiary**: Cloud storage (AWS S3 / Azure Blob)

#### Application Backups

```
┌──────────────────┬───────────┬────────────┬──────────────┐
│ Component        │ Frequency │ Retention  │ Method       │
├──────────────────┼───────────┼────────────┼──────────────┤
│ Application Code │ Per commit│ Permanent  │ Git repo     │
│ Configuration    │ Daily     │ 90 days    │ File backup  │
│ Log Files        │ Daily     │ 30 days    │ Log rotation │
│ Docker Images    │ Per build │ Last 10    │ Registry     │
└──────────────────┴───────────┴────────────┴──────────────┘
```

### Backup Verification

**Daily Verification**:
```powershell
# Automated backup verification script
.\scripts\backup-database.ps1 -Verify

# Checks performed:
# 1. Backup file exists
# 2. File size is reasonable
# 3. RESTORE VERIFYONLY (SQL Server)
# 4. Checksum validation
```

**Monthly Restoration Test**:
- Full database restore to test environment
- Application connectivity test
- Data integrity verification
- Performance validation
- Documentation of results

### Backup Retention Policy

| Backup Age | Action |
|------------|--------|
| **0-7 days** | Keep all backups (full + differential) |
| **8-30 days** | Keep daily full backups only |
| **31-90 days** | Keep weekly full backups |
| **90+ days** | Archive quarterly backups |

---

## Recovery Procedures

### Scenario 1: Database Corruption

**Symptoms**:
- Database errors in logs
- Application cannot connect
- DBCC CHECKDB reports corruption

**Recovery Steps**:

1. **Assess Damage**
   ```powershell
   # Check database integrity
   sqlcmd -S localhost -Q "DBCC CHECKDB(PatientManagementDb) WITH NO_INFOMSGS"
   ```

2. **Stop Application**
   ```powershell
   docker-compose down
   ```

3. **Attempt Repair** (if minor corruption)
   ```sql
   ALTER DATABASE PatientManagementDb SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
   DBCC CHECKDB(PatientManagementDb, REPAIR_ALLOW_DATA_LOSS);
   ALTER DATABASE PatientManagementDb SET MULTI_USER;
   ```

4. **Restore from Backup** (if repair fails)
   ```powershell
   # Find latest backup
   Get-ChildItem .\backups\*.bak | Sort-Object LastWriteTime -Descending | Select-Object -First 1
   
   # Restore database
   .\scripts\restore-database.ps1 `
       -BackupFile ".\backups\PatientManagementDb_YYYYMMDD_HHMMSS.bak" `
       -Force
   ```

5. **Apply Transaction Logs** (if available)
   ```sql
   RESTORE LOG PatientManagementDb 
   FROM DISK = 'transaction-log-backup.trn'
   WITH NORECOVERY;
   ```

6. **Verify Database**
   ```powershell
   # Check integrity
   sqlcmd -S localhost -Q "DBCC CHECKDB(PatientManagementDb)"
   
   # Verify data
   sqlcmd -S localhost -Q "SELECT COUNT(*) FROM Patients"
   ```

7. **Restart Application**
   ```powershell
   docker-compose up -d
   ```

8. **Validate Functionality**
   ```powershell
   # Health check
   curl http://localhost:5000/health
   
   # Test login
   curl -X POST http://localhost:5000/api/v1/auth/login
   ```

**Estimated Recovery Time**: 1-2 hours  
**Data Loss**: Minimal (last transaction log backup)

---

### Scenario 2: Complete Server Failure

**Symptoms**:
- Server unresponsive
- Hardware failure
- Operating system crash

**Recovery Steps**:

1. **Prepare New Server**
   - Provision replacement server
   - Install Docker and Docker Compose
   - Configure network settings

2. **Restore Application Code**
   ```bash
   cd /opt
   git clone https://github.com/your-org/patient-management-app.git
   cd patient-management-app
   git checkout tags/v1.0.0
   ```

3. **Copy Configuration**
   ```bash
   # Copy from backup or recreate
   cp /backup/location/.env.production .env.production
   cp /backup/location/nginx.conf nginx.conf
   ```

4. **Restore Database**
   
   **From Local Backup**:
   ```powershell
   # Copy backups to new server
   scp -r backups/ newserver:/opt/patient-management/
   
   # Restore on new server
   .\scripts\restore-database.ps1 -BackupFile "latest-backup.bak"
   ```
   
   **From Cloud Backup**:
   ```bash
   # Download from S3
   aws s3 cp s3://clinic-backups/latest/PatientManagementDb.bak ./backups/
   
   # Restore
   .\scripts\restore-database.ps1 -BackupFile "./backups/PatientManagementDb.bak"
   ```

5. **Deploy Application**
   ```bash
   # Build and start services
   docker-compose build
   docker-compose up -d
   ```

6. **Verify All Services**
   ```bash
   docker-compose ps
   docker-compose logs -f
   curl http://localhost:5000/health
   ```

7. **Update DNS** (if IP changed)
   ```bash
   # Point domain to new server IP
   # Update A record in DNS management console
   ```

8. **Full System Test**
   - Login functionality
   - Patient search
   - Create consultation
   - Generate prescription
   - Export data

**Estimated Recovery Time**: 2-4 hours  
**Data Loss**: Determined by last successful backup

---

### Scenario 3: Ransomware Attack

**Symptoms**:
- Files encrypted
- Ransom note displayed
- Unusual network activity
- Backup files deleted

**Immediate Actions** (within minutes):

1. **Isolate Systems**
   ```bash
   # Disconnect from network IMMEDIATELY
   sudo ip link set eth0 down
   
   # Stop all services
   docker-compose down
   
   # Power off if necessary
   sudo shutdown -h now
   ```

2. **Notify Stakeholders**
   - IT Security team
   - Management
   - Law enforcement (if required)
   - Cyber insurance provider

3. **Assess Damage**
   - Identify encrypted files
   - Check backup integrity
   - Determine entry point
   - Document everything (for forensics)

4. **DO NOT PAY RANSOM**
   - No guarantee of data recovery
   - Funds criminal activity
   - May mark you as easy target

**Recovery Steps** (after containment):

1. **Verify Offline Backups**
   ```powershell
   # Check backup storage (offline/immutable)
   ls -lh /backup/offline/
   
   # Verify backup integrity
   .\scripts\backup-database.ps1 -Verify
   ```

2. **Clean or Rebuild System**
   ```bash
   # Option 1: Clean system (not recommended)
   # Scan with anti-malware, remove infection
   
   # Option 2: Rebuild from scratch (recommended)
   # Wipe and reinstall OS
   # Reinstall applications
   ```

3. **Restore from Clean Backup**
   ```powershell
   # Use backup from BEFORE infection
   # Verify timestamp is before attack
   .\scripts\restore-database.ps1 `
       -BackupFile ".\backups\offline\pre-attack-backup.bak"
   ```

4. **Strengthen Security**
   - Change all passwords
   - Update all software
   - Review access controls
   - Implement additional security measures
   - Conduct security audit

5. **Monitor for Re-infection**
   - Enhanced logging for 30 days
   - Frequent integrity checks
   - Network traffic monitoring

**Estimated Recovery Time**: 1-3 days  
**Data Loss**: Depends on last clean backup  
**Post-Incident Review**: Mandatory within 7 days

---

### Scenario 4: Accidental Data Deletion

**Symptoms**:
- User reports missing records
- Recent data not found
- Audit logs show deletion

**Recovery Steps**:

1. **Verify Deletion**
   ```sql
   -- Check if data truly deleted
   SELECT * FROM Patients WHERE PatientId = 'XXX';
   
   -- Check audit logs
   SELECT * FROM AuditLogs 
   WHERE TableName = 'Patients' AND Action = 'DELETE'
   ORDER BY Timestamp DESC;
   ```

2. **Identify Affected Records**
   - Determine scope of deletion
   - Identify timestamp
   - Note affected tables

3. **Point-in-Time Restore**
   
   **Option A: Restore to Separate Database**
   ```powershell
   # Restore to temp database
   .\scripts\restore-database.ps1 `
       -BackupFile ".\backups\recent-backup.bak" `
       -Database "PatientManagementDb_Recovery"
   ```
   
   **Option B: Extract Specific Records**
   ```sql
   -- Copy deleted records from recovery database
   INSERT INTO PatientManagementDb.dbo.Patients
   SELECT * FROM PatientManagementDb_Recovery.dbo.Patients
   WHERE PatientId IN ('deleted-ids');
   ```

4. **Verify Restored Data**
   ```sql
   -- Confirm records restored
   SELECT * FROM Patients WHERE PatientId = 'XXX';
   
   -- Check data integrity
   SELECT COUNT(*) FROM Patients;
   ```

5. **Document Incident**
   - Who deleted data
   - What was deleted
   - When it occurred
   - Recovery actions taken

6. **Implement Preventive Measures**
   - Review user permissions
   - Add delete confirmations
   - Enhance audit logging

**Estimated Recovery Time**: 30 minutes - 2 hours  
**Data Loss**: None (if backup available)

---

### Scenario 5: Cloud Infrastructure Failure

**Symptoms**:
- Cloud provider outage
- Cannot access hosted services
- Regional data center down

**Recovery Steps**:

1. **Activate Failover Site**
   ```bash
   # Switch to secondary region
   # Update DNS to point to backup site
   
   # If using Azure:
   az network dns record-set a update \
       --resource-group clinic-rg \
       --zone-name yourdomain.com \
       --name www \
       --set aRecords[0].ipv4Address=BACKUP_IP
   ```

2. **Verify Backup Site Functionality**
   ```bash
   # Check all services
   curl https://backup-site.com/health
   
   # Test database connectivity
   sqlcmd -S backup-db-server -Q "SELECT @@VERSION"
   ```

3. **Sync Data** (if needed)
   ```bash
   # Restore latest backup to failover site
   # Or use database replication if configured
   ```

4. **Notify Users**
   - Send email notification
   - Update status page
   - Provide new URL if changed

5. **Monitor Primary Site**
   - Wait for cloud provider resolution
   - Test when primary site restored

6. **Failback to Primary**
   ```bash
   # Sync any new data back to primary
   # Switch DNS back to primary site
   # Deactivate failover site
   ```

**Estimated Recovery Time**: 1-4 hours  
**Data Loss**: Minimal (if replication configured)

---

## Testing & Maintenance

### Recovery Testing Schedule

| Test Type | Frequency | Scope | Owner |
|-----------|-----------|-------|-------|
| **Backup Verification** | Daily | Automated | System |
| **Test Restore** | Monthly | Single table | DBA |
| **Partial DR Drill** | Quarterly | One scenario | DR Team |
| **Full DR Drill** | Annually | Complete recovery | All staff |

### Monthly Test Procedure

1. **Select Test Scenario**
   - Rotate through different scenarios
   - Document planned test scope

2. **Execute Recovery**
   - Follow documented procedures
   - Time each step
   - Note any deviations

3. **Validate Results**
   - Verify data integrity
   - Test application functionality
   - Check recovery completeness

4. **Document Findings**
   - What worked well
   - What needs improvement
   - Update procedures if needed

5. **Update Plan**
   - Revise RTO/RPO if needed
   - Update contact information
   - Improve documentation

### Plan Maintenance

**Quarterly Review**:
- Review and update contact information
- Update backup locations
- Verify RTO/RPO targets
- Test emergency procedures
- Update risk assessment

**Annual Review**:
- Full DR drill
- Review and update all procedures
- Update technology stack changes
- Revise budget and resources
- Management approval

---

## Roles & Responsibilities

### Disaster Recovery Team

#### DR Coordinator
**Name**: [To be assigned]  
**Phone**: [Phone number]  
**Email**: [Email]  
**Responsibilities**:
- Overall DR plan ownership
- Coordinate recovery efforts
- Communication with stakeholders
- Post-incident review

#### Database Administrator (DBA)
**Name**: [To be assigned]  
**Phone**: [Phone number]  
**Email**: [Email]  
**Responsibilities**:
- Database backup management
- Database restoration
- Data integrity verification
- Performance optimization post-recovery

#### System Administrator
**Name**: [To be assigned]  
**Phone**: [Phone number]  
**Email**: [Email]  
**Responsibilities**:
- Server infrastructure
- Application deployment
- Network configuration
- Security implementation

#### Application Developer
**Name**: [To be assigned]  
**Phone**: [Phone number]  
**Email**: [Email]  
**Responsibilities**:
- Application troubleshooting
- Code deployment
- Configuration management
- Integration testing

#### IT Security Officer
**Name**: [To be assigned]  
**Phone**: [Phone number]  
**Email**: [Email]  
**Responsibilities**:
- Security incident response
- Forensics investigation
- Security hardening post-incident
- Compliance verification

#### Clinic Manager
**Name**: [To be assigned]  
**Phone**: [Phone number]  
**Email**: [Email]  
**Responsibilities**:
- Business continuity decisions
- Staff communication
- Patient communication (if needed)
- Resource allocation

---

## Emergency Contacts

### Internal Contacts

| Role | Name | Phone | Email | Availability |
|------|------|-------|-------|--------------|
| **DR Coordinator** | [Name] | [Phone] | [Email] | 24/7 |
| **DBA** | [Name] | [Phone] | [Email] | Business hours |
| **System Admin** | [Name] | [Phone] | [Email] | 24/7 |
| **Developer** | [Name] | [Phone] | [Email] | Business hours |
| **IT Security** | [Name] | [Phone] | [Email] | 24/7 |
| **Clinic Manager** | [Name] | [Phone] | [Email] | Business hours |

### External Contacts

| Service | Provider | Phone | Account # | Notes |
|---------|----------|-------|-----------|-------|
| **Cloud Hosting** | AWS/Azure | [Phone] | [Account] | Priority support |
| **Database Vendor** | Microsoft | [Phone] | [License] | Enterprise support |
| **Backup Service** | [Provider] | [Phone] | [Account] | 24/7 support |
| **Security Vendor** | [Provider] | [Phone] | [Account] | Incident response |
| **Cyber Insurance** | [Provider] | [Phone] | [Policy #] | Claims hotline |

### Escalation Path

```
Level 1: System Administrator
   ↓ (15 minutes if unresolved)
Level 2: DR Coordinator + DBA
   ↓ (30 minutes if unresolved)
Level 3: All DR Team + Management
   ↓ (1 hour if unresolved)
Level 4: External Vendors + Consultants
```

---

## Document Control

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | May 20, 2026 | DR Team | Initial version |

**Next Review Date**: August 20, 2026  
**Approval**: [Management Signature Required]  
**Classification**: CONFIDENTIAL - Authorized Personnel Only

---

## Appendix A: DR Checklist

### Immediate Response (0-15 minutes)
- [ ] Identify disaster type
- [ ] Activate DR team
- [ ] Assess impact
- [ ] Isolate affected systems
- [ ] Begin documentation

### Assessment Phase (15-30 minutes)
- [ ] Determine scope of damage
- [ ] Identify recovery requirements
- [ ] Check backup availability
- [ ] Estimate recovery time
- [ ] Notify stakeholders

### Recovery Phase (30 minutes - 4 hours)
- [ ] Follow scenario-specific procedures
- [ ] Restore from backups
- [ ] Verify data integrity
- [ ] Test application functionality
- [ ] Update stakeholders

### Validation Phase (Post-Recovery)
- [ ] Full system testing
- [ ] User acceptance testing
- [ ] Performance verification
- [ ] Security scan
- [ ] Documentation complete

### Post-Incident Phase (Within 7 days)
- [ ] Root cause analysis
- [ ] Lessons learned meeting
- [ ] Update DR plan
- [ ] Implement improvements
- [ ] Final report to management

---

**End of Disaster Recovery Plan**

*This plan is confidential and should be stored securely. All personnel with access to this plan should be trained on their specific roles and responsibilities.*

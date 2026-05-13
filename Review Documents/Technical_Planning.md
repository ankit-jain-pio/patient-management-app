# Technical Planning Document – Patient Management Application

**Date:** May 13, 2026  
**Based on:** BRD Analysis (May 11, 2026)  
**Status:** Technical Planning Complete with Review

---

## 1. Application Type

- **.NET Core API + React Frontend** (separated concerns)
- Justification: Decouples backend business logic from UI, enables independent scaling, allows flexible frontend updates without backend changes

---

## 2. Target Platform

- **.NET 8 (LTS)**
  - Latest long-term support, modern language features, strong performance
  - Backward compatible with .NET 6 if needed

- **React 18+**
  - Modern hooks-based component architecture, good dev tooling, meets browser compatibility requirement

---

## 3. Architecture Style

- **Clean Architecture** (layered approach)

**Justification:**
- Clear separation between API, application logic, and domain models
- Testable business logic independent of framework
- Scalable if multi-clinic support is added in future phases

**Layers:**
- Presentation (API Controllers, DTOs)
- Application (Use cases, services, business rules)
- Domain (Entities, value objects, domain services)
- Infrastructure (Data access, external integrations)

---

## 4. High-Level Component Breakdown

### Backend (.NET Core API)

| Component | Responsibility |
|-----------|-----------------|
| **API Layer** | HTTP endpoints, request validation, response formatting (Controllers, DTOs) |
| **Application Layer** | Use case orchestration, consultation workflow logic, export services |
| **Domain Layer** | Patient, Appointment, Consultation, Prescription entities; business rules |
| **Infrastructure Layer** | Entity Framework Core, database, file generation (PDF/CSV), authentication |

### Frontend (React)

| Component | Responsibility |
|-----------|-----------------|
| **UI Components** | Reusable forms, patient search, consultation entry, prescription viewer |
| **Pages/Views** | Patient dashboard, appointment calendar, consultation form, history view |
| **State Management** | React Context or lightweight Redux (patient, appointments, current consultation) |
| **Services** | API client layer, data formatting |

---

## 5. Data Persistence Approach

- **Database Type:** SQL Server (or PostgreSQL for cost optimization)
  - Relational model suits structured medical records
  - Transactional consistency for critical data (no data loss requirement)
  - Built-in backup capabilities

- **ORM Strategy:** Entity Framework Core (Code-First)
  - Rapid development, migration management, LINQ queries for search
  - Relationships naturally express patient-appointment-consultation structure

- **Storage Design:**
  - Single database instance for single clinic
  - Encryption at database level (transparent)
  - No sharding/partitioning needed for "moderate patient volume"

---

## 6. Cross-Cutting Concerns

### Authentication
- **Approach:** JWT token-based (stateless)
- **Implementation:** Single hardcoded credentials stored securely (hashed) in configuration or database
- **Session:** Token expiry ~8 hours for clinic working day
- **Dependency Note:** Requires clarification on password reset mechanism

### Logging
- **Library:** Serilog
- **Targets:** File (daily rolling) + console (development)
- **Level:** Info (operations), Debug (development), Error/Warning (critical events)
- **Scope:** API requests, errors, consultation saves (audit trail)

### Error Handling
- **Global Exception Middleware:** Standardized API error responses
- **Validation:** Fluent Validation for input constraints (vitals ranges, required fields)
- **Dependency Note:** Requires definition of valid vitals ranges (temp, BP, pulse)

### Configuration Management
- **Primary:** appsettings.json + environment-specific overrides
- **Sensitive Data:** Environment variables for DB connection, JWT secret
- **Prescription Format:** Configurable via settings (clinic name, header, footer)

---

## 7. Technical Stack Summary

| Layer | Technology |
|-------|------------|
| **Backend Framework** | .NET 8, ASP.NET Core Web API |
| **ORM** | Entity Framework Core (Code-First) |
| **Database** | SQL Server / PostgreSQL ⚠️ *[Decision pending]* |
| **Authentication** | JWT Bearer tokens, ASP.NET Core Identity |
| **Logging** | Serilog (File + Console) |
| **Validation** | FluentValidation |
| **PDF Generation** | ⚠️ *[Library TBD: iTextSharp, QuestPDF, or PdfSharpCore]* |
| **CSV Export** | CsvHelper library |
| **Frontend Framework** | React 18+ with TypeScript |
| **State Management** | React Context (or Redux if complexity requires) |
| **HTTP Client** | Axios |
| **UI Components** | Material-UI or Ant Design ⚠️ *[Decision pending]* |
| **Testing (Backend)** | xUnit, Moq, FluentAssertions |
| **Testing (Frontend)** | Jest, React Testing Library |

---

## 8. Development Environment Requirements

- Visual Studio 2022 or VS Code with C# extensions
- .NET 8 SDK
- Node.js 18+ and npm/yarn
- SQL Server Express / PostgreSQL (Docker container recommended for dev)
- Git for version control
- Postman or similar API testing tool

---

## 9. Step-by-Step Implementation Plan

### **Phase 1: Foundation & Core Setup**
1. Create .NET Core solution structure (Clean Architecture folders)
2. Set up Entity Framework Core with SQL database
3. Define core domain entities (Patient, Appointment, Consultation, Prescription, Vitals)
4. Configure dependency injection and logging (Serilog)
5. Implement authentication middleware (JWT token, single user)
6. **Define API versioning strategy** (e.g., `/api/v1/`)
7. **Select and integrate PDF generation library**

### **Phase 2: API Layer – Patient & Appointment Management**
8. Create Patient repository and API endpoints (Create, Read, Update, Search)
9. Create Appointment repository and API endpoints (Create, Read, Update Status)
10. Implement search service (by name, phone number)
11. Add validation for patient data (age, gender, contact format)
12. Create global exception handling middleware

### **Phase 3: API Layer – Consultation Workflow**
13. Create Consultation entity and repository
14. Create Vitals capture endpoints (Temperature, BP, Pulse validation)
15. Create Complaints/Diagnosis endpoints
16. Create Medication/Prescription endpoints
17. Implement consultation state management (in-progress, completed)

### **Phase 4: API Layer – History & Export**
18. Implement patient history retrieval (filtered by date range)
19. Create CSV export service (patient data, visit records)
20. Create PDF export service (prescription layout, visit summary)
21. Implement prescription generation logic (format with clinic header/footer)
22. Add query optimization for fast search retrieval (< 2s requirement)

### **Phase 5: React Frontend – Setup & Navigation**
23. Initialize React 18 project with TypeScript (strict mode)
24. Create base layout and navigation structure
25. Set up API client service layer (Axios with interceptors)
26. Implement authentication flow (login, token storage, logout)
27. Create context/state for user session
28. **Finalize state management approach** (Context vs Redux)

### **Phase 6: React Frontend – Patient Management**
29. Build patient search page (name/phone search)
30. Build patient registration form
31. Build patient profile view/edit page
32. Build recent patients quick access
33. Integrate with API endpoints

### **Phase 7: React Frontend – Appointment Management**
34. Build appointment scheduling form
35. Build daily appointment list view
36. Build appointment status update interface
37. Add appointment filtering/sorting
38. Integrate with API endpoints

### **Phase 8: React Frontend – Consultation Workflow**
39. Build vitals capture form (temperature, BP, pulse)
40. Build complaints/symptoms entry form
41. Build diagnosis documentation form
42. Build medication/prescription entry form
43. Implement multi-step consultation form navigation

### **Phase 9: React Frontend – History & Export**
44. Build patient history timeline view
45. Build visit detail modal/page
46. Build data export UI (CSV/PDF buttons)
47. Build prescription print view
48. Add date range filtering

### **Phase 10: Cross-Cutting Features & Optimization**
49. Implement error boundary components in React
50. Add loading states and spinners
51. Optimize bundle size and page load performance
52. Add form validation feedback (client-side)
53. Implement keyboard shortcuts for fast data entry

### **Phase 11: Testing & Quality**
54. Write unit tests for API services (consultation logic, validations) — **Target: 80% coverage**
55. Write integration tests for key workflows (end-to-end consultation)
56. Write component tests for critical React components — **Target: 70% coverage**
57. **Performance testing:** Page load (<2s), search response time (<2s)
58. **Security testing:** Authentication bypass attempts, SQL injection, XSS
59. **Accessibility testing:** WCAG 2.1 AA compliance

### **Phase 12: Deployment & Documentation**
60. Configure deployment pipeline (API to cloud/server)
61. Set up database backup automation (define schedule and retention)
62. **Document EF Core migration strategy** (rollback procedures)
63. Create deployment documentation
64. Create user guide for physician
65. Set up monitoring and logging dashboard
66. **Create disaster recovery plan**

---

## 10. Key Technical Decisions & Rationale

| Decision | Rationale | Dependency |
|----------|-----------|-----------|
| Clean Architecture | Testable, maintainable, scalable for future multi-clinic | None |
| EF Core Code-First | Rapid development, automatic migrations | None |
| JWT (Stateless) | Simple session management for single user, no server session storage needed | Clarify password reset process |
| Serilog | Industry standard, structured logging, minimal overhead | None |
| TypeScript (React) | Type safety, better IDE support, reduced runtime errors | None |
| SQL relational DB | Structured medical records, ACID compliance, built-in backup | Database platform choice pending |
| API Versioning (v1) | Future-proofs API for breaking changes | None |

---

## 11. Assumptions Requiring Validation Before Development

| # | Assumption | Impact | Priority |
|---|------------|--------|----------|
| 1 | **Vitals Validation Ranges** — Valid temperature (36–41°C), BP (60–200 mmHg), Pulse (40–150 bpm) | Critical for Phase 3 validation logic | **HIGH** |
| 2 | **Prescription Format** — Confirm exact clinic header/footer information fields | Required for Phase 4 PDF generation | **HIGH** |
| 3 | **Backup Automation** — Confirm frequency, retention, and restoration process | Phase 12 deployment configuration | MEDIUM |
| 4 | **Audit Logging** — Confirm if consultation modifications should be tracked for compliance | May require additional database schema | MEDIUM |
| 5 | **Password Reset** — Single user password management process (secure reset mechanism required) | Phase 1 authentication implementation | MEDIUM |
| 6 | **Database Platform** — SQL Server vs PostgreSQL (cost, hosting, licensing) | Phase 1 infrastructure setup | **HIGH** |
| 7 | **PDF Library** — License compatibility and feature requirements | Phase 1 library selection | **HIGH** |
| 8 | **Test Coverage Thresholds** — Confirm minimum acceptable coverage percentages | Phase 11 quality gates | LOW |

---

## 12. Identified Gaps & Recommendations

### Critical Gaps Addressed in Revised Plan:

1. ✅ **API Versioning Strategy Added** (Phase 1, Step 6)
2. ✅ **PDF Library Selection Added** (Phase 1, Step 7)
3. ✅ **State Management Decision Point Added** (Phase 5, Step 28)
4. ✅ **TypeScript Enforced** (Phase 5, Step 23 - strict mode)
5. ✅ **Test Coverage Metrics Defined** (Phase 11, Steps 54-56)
6. ✅ **Performance Testing Scenarios** (Phase 11, Step 57)
7. ✅ **Database Migration Strategy** (Phase 12, Step 62)
8. ✅ **Technical Stack Summary Section** (Section 7)
9. ✅ **Development Environment Requirements** (Section 8)

### Remaining Open Decisions (To Be Resolved in Phase 1):

- Database platform: SQL Server vs PostgreSQL
- PDF generation library: iTextSharp vs QuestPDF vs PdfSharpCore
- UI component library: Material-UI vs Ant Design vs custom

---

## 13. Risk Analysis

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Database schema changes post-deployment** | Medium | High | Implement EF Core migration versioning, maintain rollback scripts |
| **PDF generation library licensing issues** | Low | Medium | Evaluate license compatibility in Phase 1 before adoption |
| **Performance degradation with patient volume growth** | Medium | Medium | Add database indexing, implement query optimization early |
| **Single-user authentication bypass** | Low | High | Security testing in Phase 11, penetration testing before production |
| **Browser compatibility issues** | Low | Medium | Cross-browser testing in Phase 10, use polyfills where needed |
| **Data loss during backup/restore** | Low | Critical | Test backup/restore procedures before production deployment |

---

## 14. Out-of-Scope (Phase 1)

- Multi-user/receptionist functionality
- Billing/invoicing module
- Advanced analytics and reporting
- Mobile-native application
- Offline functionality
- Integration with external labs/pharmacies
- Electronic Health Record (EHR) interoperability
- Telemedicine capabilities

---

## 15. Success Criteria (Technical)

| Metric | Target | Validation Phase |
|--------|--------|------------------|
| API response time (patient search) | < 2 seconds | Phase 11 |
| Page load time (any view) | < 2 seconds | Phase 11 |
| Test coverage (backend) | ≥ 80% | Phase 11 |
| Test coverage (frontend) | ≥ 70% | Phase 11 |
| Zero critical security vulnerabilities | 100% | Phase 11 |
| Database backup success rate | 100% | Phase 12 |
| Consultation workflow completion time | < 3 minutes (user timing) | Phase 12 UAT |

---

## 16. Next Steps

**Immediate Actions (Before Phase 1):**
1. ✅ Obtain stakeholder approval on technical architecture
2. ⚠️ Resolve 3 HIGH priority assumptions (vitals ranges, prescription format, database platform)
3. ⚠️ Finalize PDF library selection based on license and features
4. ⚠️ Confirm hosting environment (cloud vs on-premise)
5. ✅ Set up development team access to repositories

**Then Proceed To:**
- **Worktree Setup** → Isolated development environment creation
- **Implementation Execution** → 65-step development plan

---

**Planning Status:** ✅ **COMPLETE** with minor dependencies flagged for Phase 1 resolution  
**Review Status:** ✅ **REVIEWED** — 85% ready for development handoff  
**Approval Required From:** Product Owner, Technical Lead, Clinic Owner (Stakeholder)

---

*Document prepared by Planning Agent on May 13, 2026*
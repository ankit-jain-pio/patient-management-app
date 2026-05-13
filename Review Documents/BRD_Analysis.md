# BRD Analysis – Patient Management Application

**Date:** May 11, 2026  
**Phase:** Phase 1 (Initial Analysis)

---

## 1. Business Goal

Build a simple, web-based Patient Management Application to help a general physician reduce manual paperwork, improve consultation efficiency, and maintain accurate, accessible patient history for daily clinical operations.

---

## 2. User Roles

**Primary User:**
- General Physician (single user) — performs all clinical operations

**Secondary Users:**
- None in Phase 1

**Stakeholders:**
- Clinic Owner (Doctor)
- Product Owner
- Development Team

---

## 3. High-Level Business Capabilities

- **Patient Management** — Register, view, edit patient profiles (name, age, gender, contact)
- **Appointment Management** — Schedule, track, and update appointment status (Scheduled, Completed, Cancelled, No-show)
- **Consultation Workflow** — Capture vitals, record complaints, document diagnosis, and prescribe medications
- **Prescription Generation** — Create printable prescriptions with clinic header, patient details, vitals, diagnosis, and medications
- **Patient History Access** — View previous visits with vitals, complaints, diagnosis, and prescriptions (filterable by date)
- **Search & Navigation** — Quick patient search (name, phone), view recent patients
- **Data Export** — Export patient/visit data as CSV or PDF

---

## 4. Key Business Workflows

**Primary Consultation Workflow:**
1. Locate/register patient → 
2. Review appointment → 
3. Capture mandatory vitals (temperature, BP, pulse) → 
4. Record patient complaints (symptoms) → 
5. Document diagnosis → 
6. Prescribe medications (name, dosage, frequency, duration, instructions) → 
7. Generate and print prescription → 
8. Access/review patient history as needed

**Supporting Workflows:**
- Appointment scheduling and status management
- Patient search and profile access
- Historical visit review and data export

---

## 5. Explicit Non-Functional Requirements

| Category | Requirement |
|----------|------------|
| **Usability** | Simple, minimal UI optimized for fast data entry during consultations |
| **Performance** | Page load < 2 seconds; fast patient search and retrieval |
| **Reliability** | No data loss; regular automated backups |
| **Security** | Secure login (single user); data encryption (at rest and in transit) |
| **Scalability** | Designed for single clinic with moderate patient volume |
| **Compatibility** | Modern web browsers (Chrome, Edge, Safari) |

---

## 6. Assumptions and Ambiguities Requiring Clarification

**Critical Gaps:**

1. **Authentication Model** — "Single user authentication" is mentioned, but unclear:
   - Is login hardcoded or configurable per clinic?
   - How is the single password managed (reset process)?
   - Are there any session management requirements?

2. **Backup Strategy** — "Regular automated backups" stated, but not specified:
   - Backup frequency (daily, hourly, on-demand)?
   - Retention period?
   - Restoration process defined?
   - Where are backups stored?

3. **Data Validation & Constraints** — Not explicitly defined:
   - What are valid ranges for vitals (temperature, BP, pulse)?
   - Is incomplete consultation data saveable/resumable?
   - Are mandatory fields enforced at form level?
   - What happens if vitals cannot be captured (patient refusal)?

4. **Prescription Format** — Vague specification:
   - What specific clinic header information (clinic name, address, doctor name, license)?
   - What exactly constitutes "basic footer" (signature line, notes, clinic stamp)?
   - Is prescription format customizable per clinic?

5. **Success Metric Ambiguity** — 
   - "High usability with minimal training" lacks measurable criteria (training time? error rate? user satisfaction score?).

6. **Browser Compatibility** — Incomplete specification:
   - Which versions of Chrome, Edge, Safari?
   - Mobile browser support (tablets for exam room use)?

7. **Data Retention & Privacy** — Not addressed:
   - How long should historical data be retained?
   - Are there any compliance requirements (medical privacy laws, HIPAA, local regulations)?
   - Is audit logging required for security/compliance?

8. **Concurrent Access** — Single user assumption, but unclear:
   - Will the doctor ever access from multiple devices/browsers simultaneously?
   - Session conflict handling?

9. **Incomplete Consultation Handling** — Not specified:
   - Can a doctor save a partial visit and resume later?
   - What triggers "completion" of a consultation?

10. **Data Export Performance** — Not defined:
    - Expected maximum volume per export?
    - Export time limits?

---

## Conclusion

**Status:** Business understanding is substantially complete for Phase 1 scope definition. The BRD provides clear functional boundaries and explicit scope exclusions.

**Recommendation:** Address the 10 ambiguities listed above before proceeding to detailed planning to prevent rework and ensure development alignment with intent.
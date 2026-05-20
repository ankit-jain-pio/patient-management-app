// Vitals data structure
export interface Vitals {
  temperature?: number; // Celsius
  bloodPressure?: string; // e.g., "120/80"
  pulseRate?: number; // beats per minute
  weight?: number; // kg
  height?: number; // cm
  oxygenSaturation?: number; // percentage
  respiratoryRate?: string;
  bmi?: number;
}

// Prescription structure
export interface Prescription {
  id: string;
  consultationId: string;
  medicationName: string;
  dosage: string;
  frequency: string;
  durationInDays: number;
  instructions?: string;
  prescribedDate: string;
}

// Main consultation interface
export interface Consultation {
  id: string;
  patientId: string;
  patient?: {
    id: string;
    fullName: string;
    age: number;
    phoneNumber: string;
  };
  appointmentId?: string;
  consultationDate: string;
  vitals?: Vitals;
  chiefComplaint?: string;
  symptoms?: string;
  diagnosis?: string;
  clinicalNotes?: string;
  treatmentPlan?: string;
  followUpInstructions?: string;
  nextVisitDate?: string;
  prescriptions: Prescription[];
  createdAt: string;
  updatedAt?: string;
}

// Create consultation request
export interface CreateConsultationRequest {
  patientId: string;
  appointmentId?: string;
  vitals?: Vitals;
  chiefComplaint?: string;
  symptoms?: string;
}

// Update consultation request
export interface UpdateConsultationRequest {
  vitals?: Vitals;
  chiefComplaint?: string;
  symptoms?: string;
  diagnosis?: string;
  clinicalNotes?: string;
  treatmentPlan?: string;
  followUpInstructions?: string;
  nextVisitDate?: string;
}

// Add prescription request
export interface AddPrescriptionRequest {
  medicationName: string;
  dosage: string;
  frequency: string;
  durationInDays: number;
  instructions?: string;
}

// Consultation form data (used in multi-step form)
export interface ConsultationFormData {
  // Step 1: Vitals
  vitals: Vitals;
  // Step 2: Complaints/Symptoms
  chiefComplaint: string;
  symptoms: string;
  // Step 3: Diagnosis
  diagnosis: string;
  clinicalNotes: string;
  treatmentPlan: string;
  followUpInstructions: string;
  nextVisitDate?: string;
  // Step 4: Prescriptions
  prescriptions: AddPrescriptionRequest[];
}

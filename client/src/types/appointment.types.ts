// Appointment status enum matching backend
export const AppointmentStatus = {
  Scheduled: 1,
  CheckedIn: 2,
  InProgress: 3,
  Completed: 4,
  Cancelled: 5,
  NoShow: 6,
} as const;

export type AppointmentStatus = (typeof AppointmentStatus)[keyof typeof AppointmentStatus];

// Patient summary for appointment display
export interface PatientSummary {
  id: string;
  fullName: string;
  age: number;
  phoneNumber: string;
}

// Main appointment interface
export interface Appointment {
  id: string;
  patientId: string;
  patient?: PatientSummary;
  scheduledDateTime: string;
  status: AppointmentStatus;
  reason?: string;
  notes?: string;
  checkInTime?: string;
  completedTime?: string;
  createdAt: string;
  updatedAt?: string;
}

// Create appointment request
export interface CreateAppointmentRequest {
  patientId: string;
  scheduledDateTime: string;
  reason?: string;
  notes?: string;
}

// Update appointment request
export interface UpdateAppointmentRequest {
  scheduledDateTime: string;
  reason?: string;
  notes?: string;
}

// Update appointment status request
export interface UpdateAppointmentStatusRequest {
  status: AppointmentStatus;
  checkInTime?: string;
  completedTime?: string;
}

// Appointment filters for query parameters
export interface AppointmentFilters {
  date?: string;
  patientId?: string;
  status?: AppointmentStatus;
  sortBy?: 'scheduledDateTime' | 'createdAt' | 'status';
  sortOrder?: 'asc' | 'desc';
}

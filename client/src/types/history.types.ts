import type { Consultation } from './consultation.types';
import type { Appointment } from './appointment.types';

// Patient history data
export interface PatientHistory {
  patientId: string;
  patientName: string;
  age: number;
  consultations: Consultation[];
  appointments: Appointment[];
  totalConsultations: number;
  totalAppointments: number;
}

// Date range filter
export interface DateRangeFilter {
  startDate?: string; // ISO date string
  endDate?: string; // ISO date string
}

// Export options
export interface ExportOptions {
  format: 'csv' | 'pdf';
  dateRange?: DateRangeFilter;
}

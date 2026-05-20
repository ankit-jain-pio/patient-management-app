import { apiService } from './api.service';
import type {
  Appointment,
  CreateAppointmentRequest,
  UpdateAppointmentRequest,
  UpdateAppointmentStatusRequest,
} from '../types/appointment.types';

class AppointmentService {
  private readonly basePath = '/appointments';

  public async getAppointmentsByDate(date: string): Promise<Appointment[]> {
    return apiService.get<Appointment[]>(`${this.basePath}/by-date?date=${encodeURIComponent(date)}`);
  }

  public async getPatientAppointments(patientId: string): Promise<Appointment[]> {
    return apiService.get<Appointment[]>(`${this.basePath}/patient/${patientId}`);
  }

  public async getAppointmentById(id: string): Promise<Appointment> {
    return apiService.get<Appointment>(`${this.basePath}/${id}`);
  }

  public async createAppointment(data: CreateAppointmentRequest): Promise<Appointment> {
    return apiService.post<Appointment>(this.basePath, data);
  }

  public async updateAppointment(id: string, data: UpdateAppointmentRequest): Promise<Appointment> {
    return apiService.put<Appointment>(`${this.basePath}/${id}`, data);
  }

  public async updateAppointmentStatus(
    id: string,
    data: UpdateAppointmentStatusRequest
  ): Promise<Appointment> {
    return apiService.patch<Appointment>(`${this.basePath}/${id}/status`, data);
  }
}

export const appointmentService = new AppointmentService();
export default appointmentService;

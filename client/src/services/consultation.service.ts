import { apiService } from './api.service';
import type {
  Consultation,
  CreateConsultationRequest,
  UpdateConsultationRequest,
  AddPrescriptionRequest,
  Prescription,
} from '../types/consultation.types';

class ConsultationService {
  private readonly basePath = '/consultations';

  public async getConsultationById(id: string): Promise<Consultation> {
    return apiService.get<Consultation>(`${this.basePath}/${id}`);
  }

  public async getPatientConsultations(patientId: string): Promise<Consultation[]> {
    return apiService.get<Consultation[]>(`${this.basePath}/patient/${patientId}`);
  }

  public async createConsultation(data: CreateConsultationRequest): Promise<Consultation> {
    return apiService.post<Consultation>(this.basePath, data);
  }

  public async updateConsultation(id: string, data: UpdateConsultationRequest): Promise<Consultation> {
    return apiService.put<Consultation>(`${this.basePath}/${id}`, data);
  }

  public async addPrescription(consultationId: string, data: AddPrescriptionRequest): Promise<Prescription> {
    return apiService.post<Prescription>(`${this.basePath}/${consultationId}/prescriptions`, data);
  }
}

export const consultationService = new ConsultationService();

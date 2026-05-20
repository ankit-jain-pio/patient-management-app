import { apiService } from './api.service';
import type {
  Patient,
  PatientSearchResult,
  CreatePatientRequest,
  UpdatePatientRequest,
  PatientSearchParams,
} from '../types/patient.types';

class PatientService {
  private readonly basePath = '/patients';

  public async getAllPatients(): Promise<Patient[]> {
    return apiService.get<Patient[]>(this.basePath);
  }

  public async searchPatients(params: PatientSearchParams): Promise<PatientSearchResult[]> {
    const queryParams = new URLSearchParams();
    if (params.searchTerm) queryParams.append('searchTerm', params.searchTerm);
    if (params.pageNumber) queryParams.append('pageNumber', params.pageNumber.toString());
    if (params.pageSize) queryParams.append('pageSize', params.pageSize.toString());

    const query = queryParams.toString();
    const url = `${this.basePath}/search${query ? `?${query}` : ''}`;
    
    return apiService.get<PatientSearchResult[]>(url);
  }

  public async getPatientById(id: string): Promise<Patient> {
    return apiService.get<Patient>(`${this.basePath}/${id}`);
  }

  public async createPatient(data: CreatePatientRequest): Promise<Patient> {
    return apiService.post<Patient>(this.basePath, data);
  }

  public async updatePatient(id: string, data: UpdatePatientRequest): Promise<Patient> {
    return apiService.put<Patient>(`${this.basePath}/${id}`, data);
  }
}

export const patientService = new PatientService();
export default patientService;

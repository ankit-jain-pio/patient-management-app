import { apiService } from './api.service';
import type { PatientHistory, DateRangeFilter } from '../types/history.types';

class ExportService {
  private readonly basePath = '/exports';

  // Get patient history with optional date filtering
  public async getPatientHistory(
    patientId: string,
    dateRange?: DateRangeFilter
  ): Promise<PatientHistory> {
    const params = new URLSearchParams();
    if (dateRange?.startDate) {
      params.append('startDate', dateRange.startDate);
    }
    if (dateRange?.endDate) {
      params.append('endDate', dateRange.endDate);
    }

    const queryString = params.toString();
    const url = `/patients/${patientId}/history${queryString ? `?${queryString}` : ''}`;
    
    return apiService.get<PatientHistory>(url);
  }

  // Export patients to CSV
  public exportPatientsCsv(): string {
    return `${apiService.getBaseUrl()}${this.basePath}/patients/csv`;
  }

  // Export appointments to CSV
  public exportAppointmentsCsv(dateRange?: DateRangeFilter): string {
    const params = new URLSearchParams();
    if (dateRange?.startDate) {
      params.append('startDate', dateRange.startDate);
    }
    if (dateRange?.endDate) {
      params.append('endDate', dateRange.endDate);
    }

    const queryString = params.toString();
    return `${apiService.getBaseUrl()}${this.basePath}/appointments/csv${queryString ? `?${queryString}` : ''}`;
  }

  // Export patient history to CSV
  public exportPatientHistoryCsv(patientId: string, dateRange?: DateRangeFilter): string {
    const params = new URLSearchParams();
    if (dateRange?.startDate) {
      params.append('startDate', dateRange.startDate);
    }
    if (dateRange?.endDate) {
      params.append('endDate', dateRange.endDate);
    }

    const queryString = params.toString();
    return `${apiService.getBaseUrl()}${this.basePath}/patients/${patientId}/history/csv${queryString ? `?${queryString}` : ''}`;
  }

  // Generate prescription PDF
  public getPrescriptionPdfUrl(consultationId: string): string {
    return `${apiService.getBaseUrl()}${this.basePath}/consultations/${consultationId}/prescription/pdf`;
  }

  // Generate consultation summary PDF
  public getConsultationSummaryPdfUrl(consultationId: string): string {
    return `${apiService.getBaseUrl()}${this.basePath}/consultations/${consultationId}/summary/pdf`;
  }

  // Download file helper (triggers browser download)
  public downloadFile(url: string, filename?: string): void {
    const link = document.createElement('a');
    link.href = url;
    link.download = filename || '';
    link.target = '_blank';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  }
}

export const exportService = new ExportService();

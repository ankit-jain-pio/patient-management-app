import { describe, it, expect, vi, beforeEach } from 'vitest';
import { exportService } from '../services/export.service';
import { apiService } from '../services/api.service';

vi.mock('../services/api.service');

describe('ExportService', () => {
  beforeEach(() => {
    vi.clearAllMocks();
    vi.mocked(apiService.getBaseUrl).mockReturnValue('http://localhost:5000/api/v1');
  });

  describe('getPatientHistory', () => {
    it('should fetch patient history without date range', async () => {
      const mockHistory = {
        patientId: 'patient-123',
        patientName: 'John Doe',
        age: 36,
        consultations: [],
        appointments: [],
        totalConsultations: 0,
        totalAppointments: 0,
      };

      vi.mocked(apiService.get).mockResolvedValue(mockHistory);

      const result = await exportService.getPatientHistory('patient-123');

      expect(apiService.get).toHaveBeenCalledWith('/patients/patient-123/history');
      expect(result).toEqual(mockHistory);
    });

    it('should fetch patient history with date range', async () => {
      const mockHistory = {
        patientId: 'patient-123',
        patientName: 'John Doe',
        age: 36,
        consultations: [],
        appointments: [],
        totalConsultations: 0,
        totalAppointments: 0,
      };

      vi.mocked(apiService.get).mockResolvedValue(mockHistory);

      await exportService.getPatientHistory('patient-123', {
        startDate: '2026-01-01',
        endDate: '2026-12-31',
      });

      expect(apiService.get).toHaveBeenCalledWith(
        '/patients/patient-123/history?startDate=2026-01-01&endDate=2026-12-31'
      );
    });
  });

  describe('Export URL Generators', () => {
    it('should generate patients CSV export URL', () => {
      const url = exportService.exportPatientsCsv();
      expect(url).toBe('http://localhost:5000/api/v1/exports/patients/csv');
    });

    it('should generate appointments CSV export URL without date range', () => {
      const url = exportService.exportAppointmentsCsv();
      expect(url).toBe('http://localhost:5000/api/v1/exports/appointments/csv');
    });

    it('should generate appointments CSV export URL with date range', () => {
      const url = exportService.exportAppointmentsCsv({
        startDate: '2026-01-01',
        endDate: '2026-12-31',
      });
      expect(url).toBe(
        'http://localhost:5000/api/v1/exports/appointments/csv?startDate=2026-01-01&endDate=2026-12-31'
      );
    });

    it('should generate patient history CSV export URL', () => {
      const url = exportService.exportPatientHistoryCsv('patient-123', {
        startDate: '2026-01-01',
        endDate: '2026-12-31',
      });
      expect(url).toBe(
        'http://localhost:5000/api/v1/exports/patients/patient-123/history/csv?startDate=2026-01-01&endDate=2026-12-31'
      );
    });

    it('should generate prescription PDF URL', () => {
      const url = exportService.getPrescriptionPdfUrl('consultation-123');
      expect(url).toBe(
        'http://localhost:5000/api/v1/exports/consultations/consultation-123/prescription/pdf'
      );
    });

    it('should generate consultation summary PDF URL', () => {
      const url = exportService.getConsultationSummaryPdfUrl('consultation-123');
      expect(url).toBe(
        'http://localhost:5000/api/v1/exports/consultations/consultation-123/summary/pdf'
      );
    });
  });

  describe('downloadFile', () => {
    it('should create and click download link', () => {
      const createElementSpy = vi.spyOn(document, 'createElement');
      const appendChildSpy = vi.spyOn(document.body, 'appendChild').mockImplementation(() => null as any);
      const removeChildSpy = vi.spyOn(document.body, 'removeChild').mockImplementation(() => null as any);

      const mockLink = {
        href: '',
        download: '',
        target: '',
        click: vi.fn(),
      } as any;

      createElementSpy.mockReturnValue(mockLink);

      exportService.downloadFile('http://example.com/file.pdf', 'test.pdf');

      expect(mockLink.href).toBe('http://example.com/file.pdf');
      expect(mockLink.download).toBe('test.pdf');
      expect(mockLink.target).toBe('_blank');
      expect(mockLink.click).toHaveBeenCalled();
      expect(appendChildSpy).toHaveBeenCalledWith(mockLink);
      expect(removeChildSpy).toHaveBeenCalledWith(mockLink);

      createElementSpy.mockRestore();
      appendChildSpy.mockRestore();
      removeChildSpy.mockRestore();
    });
  });
});

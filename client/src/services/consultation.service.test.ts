import { describe, it, expect, vi, beforeEach } from 'vitest';
import { consultationService } from '../services/consultation.service';
import { apiService } from '../services/api.service';

vi.mock('../services/api.service');

describe('ConsultationService', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe('getConsultationById', () => {
    it('should fetch consultation by ID', async () => {
      const mockConsultation = {
        id: 'consultation-123',
        patientId: 'patient-123',
        consultationDate: '2026-05-20T10:00:00Z',
        chiefComplaint: 'Headache',
        prescriptions: [],
        createdAt: '2026-05-20T10:00:00Z',
      };

      vi.mocked(apiService.get).mockResolvedValue(mockConsultation);

      const result = await consultationService.getConsultationById('consultation-123');

      expect(apiService.get).toHaveBeenCalledWith('/consultations/consultation-123');
      expect(result).toEqual(mockConsultation);
    });
  });

  describe('getPatientConsultations', () => {
    it('should fetch all consultations for a patient', async () => {
      const mockConsultations = [
        {
          id: 'consultation-1',
          patientId: 'patient-123',
          consultationDate: '2026-05-20T10:00:00Z',
          chiefComplaint: 'Headache',
          prescriptions: [],
          createdAt: '2026-05-20T10:00:00Z',
        },
      ];

      vi.mocked(apiService.get).mockResolvedValue(mockConsultations);

      const result = await consultationService.getPatientConsultations('patient-123');

      expect(apiService.get).toHaveBeenCalledWith('/consultations/patient/patient-123');
      expect(result).toEqual(mockConsultations);
    });
  });

  describe('createConsultation', () => {
    it('should create a new consultation', async () => {
      const createData = {
        patientId: 'patient-123',
        appointmentId: 'appointment-123',
        vitals: {
          temperature: 37.2,
          bloodPressure: '120/80',
          pulseRate: 72,
        },
        chiefComplaint: 'Fever and headache',
        symptoms: 'Started yesterday',
      };

      const mockResponse = {
        id: 'consultation-123',
        ...createData,
        consultationDate: '2026-05-20T10:00:00Z',
        prescriptions: [],
        createdAt: '2026-05-20T10:00:00Z',
      };

      vi.mocked(apiService.post).mockResolvedValue(mockResponse);

      const result = await consultationService.createConsultation(createData);

      expect(apiService.post).toHaveBeenCalledWith('/consultations', createData);
      expect(result).toEqual(mockResponse);
    });
  });

  describe('updateConsultation', () => {
    it('should update an existing consultation', async () => {
      const updateData = {
        diagnosis: 'Viral fever',
        treatmentPlan: 'Rest and fluids',
        clinicalNotes: 'Patient appears stable',
      };

      const mockResponse = {
        id: 'consultation-123',
        patientId: 'patient-123',
        consultationDate: '2026-05-20T10:00:00Z',
        ...updateData,
        prescriptions: [],
        createdAt: '2026-05-20T10:00:00Z',
        updatedAt: '2026-05-20T11:00:00Z',
      };

      vi.mocked(apiService.put).mockResolvedValue(mockResponse);

      const result = await consultationService.updateConsultation('consultation-123', updateData);

      expect(apiService.put).toHaveBeenCalledWith('/consultations/consultation-123', updateData);
      expect(result).toEqual(mockResponse);
    });
  });

  describe('addPrescription', () => {
    it('should add prescription to consultation', async () => {
      const prescriptionData = {
        medicationName: 'Paracetamol',
        dosage: '500mg',
        frequency: '3 times daily',
        durationInDays: 5,
        instructions: 'Take after meals',
      };

      const mockResponse = {
        id: 'prescription-123',
        consultationId: 'consultation-123',
        ...prescriptionData,
        prescribedDate: '2026-05-20T10:00:00Z',
      };

      vi.mocked(apiService.post).mockResolvedValue(mockResponse);

      const result = await consultationService.addPrescription('consultation-123', prescriptionData);

      expect(apiService.post).toHaveBeenCalledWith(
        '/consultations/consultation-123/prescriptions',
        prescriptionData
      );
      expect(result).toEqual(mockResponse);
    });
  });
});

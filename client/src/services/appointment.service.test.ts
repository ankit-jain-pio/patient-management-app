import { describe, it, expect, vi, beforeEach } from 'vitest';
import { appointmentService } from '../services/appointment.service';
import { apiService } from '../services/api.service';
import type {
  CreateAppointmentRequest,
  UpdateAppointmentRequest,
  UpdateAppointmentStatusRequest,
} from '../types/appointment.types';
import { AppointmentStatus } from '../types/appointment.types';

vi.mock('../services/api.service');

describe('AppointmentService', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe('getAppointmentsByDate', () => {
    it('should fetch appointments by date', async () => {
      const mockDate = '2026-05-20';
      const mockAppointments = [
        {
          id: '1',
          patientId: 'patient-1',
          scheduledDateTime: '2026-05-20T10:00:00Z',
          status: AppointmentStatus.Scheduled,
          reason: 'Checkup',
          createdAt: '2026-05-19T00:00:00Z',
        },
      ];

      vi.mocked(apiService.get).mockResolvedValue(mockAppointments);

      const result = await appointmentService.getAppointmentsByDate(mockDate);

      expect(apiService.get).toHaveBeenCalledWith('/appointments/by-date?date=2026-05-20');
      expect(result).toEqual(mockAppointments);
    });
  });

  describe('getPatientAppointments', () => {
    it('should fetch appointments for a specific patient', async () => {
      const patientId = 'patient-123';
      const mockAppointments = [
        {
          id: '1',
          patientId,
          scheduledDateTime: '2026-05-20T10:00:00Z',
          status: AppointmentStatus.Scheduled,
          createdAt: '2026-05-19T00:00:00Z',
        },
      ];

      vi.mocked(apiService.get).mockResolvedValue(mockAppointments);

      const result = await appointmentService.getPatientAppointments(patientId);

      expect(apiService.get).toHaveBeenCalledWith(`/appointments/patient/${patientId}`);
      expect(result).toEqual(mockAppointments);
    });
  });

  describe('getAppointmentById', () => {
    it('should fetch appointment by ID', async () => {
      const appointmentId = '123';
      const mockAppointment = {
        id: appointmentId,
        patientId: 'patient-1',
        scheduledDateTime: '2026-05-20T10:00:00Z',
        status: AppointmentStatus.Scheduled,
        reason: 'Checkup',
        createdAt: '2026-05-19T00:00:00Z',
      };

      vi.mocked(apiService.get).mockResolvedValue(mockAppointment);

      const result = await appointmentService.getAppointmentById(appointmentId);

      expect(apiService.get).toHaveBeenCalledWith(`/appointments/${appointmentId}`);
      expect(result).toEqual(mockAppointment);
    });
  });

  describe('createAppointment', () => {
    it('should create a new appointment', async () => {
      const request: CreateAppointmentRequest = {
        patientId: 'patient-123',
        scheduledDateTime: '2026-05-20T10:00:00',
        reason: 'Annual checkup',
        notes: 'Fasting required',
      };

      const mockResponse = {
        id: '456',
        ...request,
        status: AppointmentStatus.Scheduled,
        createdAt: '2026-05-19T00:00:00Z',
      };

      vi.mocked(apiService.post).mockResolvedValue(mockResponse);

      const result = await appointmentService.createAppointment(request);

      expect(apiService.post).toHaveBeenCalledWith('/appointments', request);
      expect(result).toEqual(mockResponse);
    });
  });

  describe('updateAppointment', () => {
    it('should update an existing appointment', async () => {
      const appointmentId = '123';
      const request: UpdateAppointmentRequest = {
        scheduledDateTime: '2026-05-20T14:00:00',
        reason: 'Follow-up consultation',
        notes: 'Bring previous reports',
      };

      const mockResponse = {
        id: appointmentId,
        patientId: 'patient-1',
        ...request,
        status: AppointmentStatus.Scheduled,
        createdAt: '2026-05-19T00:00:00Z',
        updatedAt: '2026-05-20T00:00:00Z',
      };

      vi.mocked(apiService.put).mockResolvedValue(mockResponse);

      const result = await appointmentService.updateAppointment(appointmentId, request);

      expect(apiService.put).toHaveBeenCalledWith(`/appointments/${appointmentId}`, request);
      expect(result).toEqual(mockResponse);
    });
  });

  describe('updateAppointmentStatus', () => {
    it('should update appointment status', async () => {
      const appointmentId = '123';
      const request: UpdateAppointmentStatusRequest = {
        status: AppointmentStatus.CheckedIn,
        checkInTime: '2026-05-20T10:05:00Z',
      };

      const mockResponse = {
        id: appointmentId,
        patientId: 'patient-1',
        scheduledDateTime: '2026-05-20T10:00:00Z',
        status: AppointmentStatus.CheckedIn,
        checkInTime: '2026-05-20T10:05:00Z',
        createdAt: '2026-05-19T00:00:00Z',
        updatedAt: '2026-05-20T10:05:00Z',
      };

      vi.mocked(apiService.patch).mockResolvedValue(mockResponse);

      const result = await appointmentService.updateAppointmentStatus(appointmentId, request);

      expect(apiService.patch).toHaveBeenCalledWith(
        `/appointments/${appointmentId}/status`,
        request
      );
      expect(result).toEqual(mockResponse);
    });
  });
});

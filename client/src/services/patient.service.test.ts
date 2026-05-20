import { describe, it, expect, vi, beforeEach } from 'vitest';
import { patientService } from '../services/patient.service';
import { apiService } from '../services/api.service';
import type { CreatePatientRequest, UpdatePatientRequest, PatientSearchParams } from '../types/patient.types';
import { Gender } from '../types/patient.types';

vi.mock('../services/api.service');

describe('PatientService', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe('getAllPatients', () => {
    it('should fetch all patients', async () => {
      const mockPatients = [
        {
          id: '1',
          firstName: 'John',
          lastName: 'Doe',
          fullName: 'John Doe',
          dateOfBirth: '1990-01-01T00:00:00Z',
          age: 34,
          gender: Gender.Male,
          phoneNumber: '1234567890',
          email: 'john@example.com',
          createdAt: '2025-01-01T00:00:00Z',
        },
      ];

      vi.mocked(apiService.get).mockResolvedValue(mockPatients);

      const result = await patientService.getAllPatients();

      expect(apiService.get).toHaveBeenCalledWith('/patients');
      expect(result).toEqual(mockPatients);
    });
  });

  describe('searchPatients', () => {
    it('should search patients with search parameters', async () => {
      const params: PatientSearchParams = {
        searchTerm: 'John',
        pageNumber: 1,
        pageSize: 10,
      };

      const mockResults = [
        {
          id: '1',
          fullName: 'John Doe',
          age: 34,
          gender: Gender.Male,
          phoneNumber: '1234567890',
          lastVisit: '2025-01-01T00:00:00Z',
          totalVisits: 5,
        },
      ];

      vi.mocked(apiService.get).mockResolvedValue(mockResults);

      const result = await patientService.searchPatients(params);

      expect(apiService.get).toHaveBeenCalledWith('/patients/search?searchTerm=John&pageNumber=1&pageSize=10');
      expect(result).toEqual(mockResults);
    });
  });

  describe('getPatientById', () => {
    it('should fetch patient by ID', async () => {
      const patientId = '123';
      const mockPatient = {
        id: patientId,
        firstName: 'John',
        lastName: 'Doe',
        fullName: 'John Doe',
        dateOfBirth: '1990-01-01T00:00:00Z',
        age: 34,
        gender: Gender.Male,
        phoneNumber: '1234567890',
        email: 'john@example.com',
        createdAt: '2025-01-01T00:00:00Z',
      };

      vi.mocked(apiService.get).mockResolvedValue(mockPatient);

      const result = await patientService.getPatientById(patientId);

      expect(apiService.get).toHaveBeenCalledWith(`/patients/${patientId}`);
      expect(result).toEqual(mockPatient);
    });
  });

  describe('createPatient', () => {
    it('should create a new patient', async () => {
      const request: CreatePatientRequest = {
        firstName: 'Jane',
        lastName: 'Smith',
        dateOfBirth: '1995-05-15',
        gender: Gender.Female,
        phoneNumber: '9876543210',
        email: 'jane@example.com',
        address: {
          street: '123 Main St',
          city: 'Mumbai',
          state: 'Maharashtra',
          postalCode: '400001',
          country: 'India',
        },
        emergencyContactName: 'John Smith',
        emergencyContactPhone: '1234567890',
        bloodGroup: 'A+',
        allergies: 'None',
        medicalHistory: 'None',
      };

      const mockResponse = {
        id: '456',
        ...request,
        fullName: 'Jane Smith',
        age: 29,
        createdAt: '2025-01-01T00:00:00Z',
      };

      vi.mocked(apiService.post).mockResolvedValue(mockResponse);

      const result = await patientService.createPatient(request);

      expect(apiService.post).toHaveBeenCalledWith('/patients', request);
      expect(result).toEqual(mockResponse);
    });
  });

  describe('updatePatient', () => {
    it('should update an existing patient', async () => {
      const patientId = '123';
      const request: UpdatePatientRequest = {
        id: patientId,
        firstName: 'John',
        lastName: 'Doe',
        dateOfBirth: '1990-01-01',
        gender: Gender.Male,
        phoneNumber: '1234567890',
        email: 'john.updated@example.com',
        address: {
          street: '456 New St',
          city: 'Delhi',
          state: 'Delhi',
          postalCode: '110001',
          country: 'India',
        },
        emergencyContactName: 'Jane Doe',
        emergencyContactPhone: '9876543210',
        bloodGroup: 'O+',
        allergies: 'Peanuts',
        medicalHistory: 'Diabetes',
      };

      const mockResponse = {
        ...request,
        fullName: 'John Doe',
        age: 34,
        createdAt: '2025-01-01T00:00:00Z',
      };

      vi.mocked(apiService.put).mockResolvedValue(mockResponse);

      const result = await patientService.updatePatient(patientId, request);

      expect(apiService.put).toHaveBeenCalledWith(`/patients/${patientId}`, request);
      expect(result).toEqual(mockResponse);
    });
  });
});

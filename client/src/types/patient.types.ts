export const Gender = {
  Male: 0,
  Female: 1,
  Other: 2,
} as const;

export type Gender = (typeof Gender)[keyof typeof Gender];

export interface Address {
  street: string;
  city: string;
  state: string;
  postalCode: string;
  country?: string;
}

export interface Patient {
  id: string;
  firstName: string;
  lastName: string;
  fullName: string;
  dateOfBirth: string;
  age: number;
  gender: Gender;
  phoneNumber: string;
  email?: string;
  address?: Address;
  emergencyContactName?: string;
  emergencyContactPhone?: string;
  bloodGroup?: string;
  allergies?: string;
  medicalHistory?: string;
  createdAt: string;
  updatedAt?: string;
}

export interface PatientSearchResult {
  id: string;
  firstName: string;
  lastName: string;
  fullName: string;
  age: number;
  gender: Gender;
  phoneNumber: string;
  lastVisit: string;
  totalVisits: number;
}

export interface CreatePatientRequest {
  firstName: string;
  lastName: string;
  dateOfBirth: string;
  gender: Gender;
  phoneNumber: string;
  email?: string;
  address?: {
    street: string;
    city: string;
    state: string;
    postalCode: string;
    country?: string;
  };
  emergencyContactName?: string;
  emergencyContactPhone?: string;
  bloodGroup?: string;
  allergies?: string;
  medicalHistory?: string;
}

export interface UpdatePatientRequest extends CreatePatientRequest {
  id: string;
}

export interface PatientSearchParams {
  searchTerm?: string;
  pageNumber?: number;
  pageSize?: number;
}

export const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000/api/v1';

export const AUTH_TOKEN_KEY = 'pm_auth_token';
export const AUTH_USER_KEY = 'pm_user';

export const ROUTES = {
  HOME: '/',
  LOGIN: '/login',
  PATIENTS: '/patients',
  PATIENT_NEW: '/patients/new',
  PATIENT_DETAIL: '/patients/:id',
  APPOINTMENTS: '/appointments',
  CONSULTATIONS: '/consultations',
  HISTORY: '/history',
} as const;

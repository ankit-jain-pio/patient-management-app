import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { BrowserRouter } from 'react-router-dom';
import { PatientDetail } from '../pages/PatientDetail';
import { patientService } from '../services/patient.service';
import { Gender } from '../types/patient.types';

vi.mock('../services/patient.service');

const mockNavigate = vi.fn();
const mockParams = { id: '123' };

vi.mock('react-router-dom', async () => {
  const actual = await vi.importActual('react-router-dom');
  return {
    ...actual,
    useNavigate: () => mockNavigate,
    useParams: () => mockParams,
  };
});

describe('PatientDetail', () => {
  const mockPatient = {
    id: '123',
    firstName: 'John',
    lastName: 'Doe',
    fullName: 'John Doe',
    dateOfBirth: '1990-01-01T00:00:00Z',
    age: 34,
    gender: Gender.Male,
    phoneNumber: '1234567890',
    email: 'john@example.com',
    address: {
      street: '123 Main St',
      city: 'Mumbai',
      state: 'Maharashtra',
      postalCode: '400001',
      country: 'India',
    },
    emergencyContactName: 'Jane Doe',
    emergencyContactPhone: '9876543210',
    bloodGroup: 'O+',
    allergies: 'None',
    medicalHistory: 'None',
    createdAt: '2025-01-01T00:00:00Z',
  };

  beforeEach(() => {
    vi.clearAllMocks();
  });

  const renderComponent = () => {
    return render(
      <BrowserRouter>
        <PatientDetail />
      </BrowserRouter>
    );
  };

  it('should display loading state initially', () => {
    vi.mocked(patientService.getPatientById).mockImplementation(
      () => new Promise(() => {})
    );

    renderComponent();
    expect(screen.getByRole('progressbar')).toBeInTheDocument();
  });

  it('should load and display patient details', async () => {
    vi.mocked(patientService.getPatientById).mockResolvedValue(mockPatient);

    renderComponent();

    await waitFor(() => {
      expect(screen.getByText('John Doe')).toBeInTheDocument();
    });

    expect(screen.getByText('John')).toBeInTheDocument();
    expect(screen.getByText('Doe')).toBeInTheDocument();
    expect(screen.getByText('1234567890')).toBeInTheDocument();
  });

  it('should enable edit mode when edit button is clicked', async () => {
    vi.mocked(patientService.getPatientById).mockResolvedValue(mockPatient);

    renderComponent();

    await waitFor(() => {
      expect(screen.getByText('John Doe')).toBeInTheDocument();
    });

    const editButton = screen.getByText('Edit');
    fireEvent.click(editButton);

    await waitFor(() => {
      expect(screen.getByText('Save')).toBeInTheDocument();
      expect(screen.getByText('Cancel')).toBeInTheDocument();
    });
  });

  it('should cancel edit mode when cancel is clicked', async () => {
    vi.mocked(patientService.getPatientById).mockResolvedValue(mockPatient);

    renderComponent();

    await waitFor(() => {
      expect(screen.getByText('John Doe')).toBeInTheDocument();
    });

    const editButton = screen.getByText('Edit');
    fireEvent.click(editButton);

    await waitFor(() => {
      expect(screen.getByText('Cancel')).toBeInTheDocument();
    });

    const cancelButton = screen.getByText('Cancel');
    fireEvent.click(cancelButton);

    await waitFor(() => {
      expect(screen.getByText('Edit')).toBeInTheDocument();
    });
  });

  it('should save changes when save button is clicked', async () => {
    vi.mocked(patientService.getPatientById).mockResolvedValue(mockPatient);
    vi.mocked(patientService.updatePatient).mockResolvedValue(mockPatient);

    renderComponent();

    await waitFor(() => {
      expect(screen.getByText('John Doe')).toBeInTheDocument();
    });

    const editButton = screen.getByText('Edit');
    fireEvent.click(editButton);

    await waitFor(() => {
      expect(screen.getByText('Save')).toBeInTheDocument();
    });

    const saveButton = screen.getByText('Save');
    fireEvent.click(saveButton);

    await waitFor(() => {
      expect(patientService.updatePatient).toHaveBeenCalledWith('123', expect.any(Object));
    });
  });

  it('should display error when patient load fails', async () => {
    vi.mocked(patientService.getPatientById).mockRejectedValue(new Error('API Error'));

    renderComponent();

    await waitFor(() => {
      expect(screen.getByText(/patient not found/i)).toBeInTheDocument();
    });
  });

  it('should navigate back when back button is clicked', async () => {
    vi.mocked(patientService.getPatientById).mockResolvedValue(mockPatient);

    renderComponent();

    await waitFor(() => {
      expect(screen.getByText('John Doe')).toBeInTheDocument();
    });

    const backButton = screen.getAllByRole('button')[0]; // First button is back
    fireEvent.click(backButton);

    expect(mockNavigate).toHaveBeenCalledWith('/patients');
  });
});

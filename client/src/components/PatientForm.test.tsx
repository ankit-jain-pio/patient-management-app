import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { PatientForm } from '../components/PatientForm';
import { patientService } from '../services/patient.service';
import { Gender } from '../types/patient.types';

vi.mock('../services/patient.service');

describe('PatientForm', () => {
  const mockOnClose = vi.fn();
  const mockOnSuccess = vi.fn();

  beforeEach(() => {
    vi.clearAllMocks();
  });

  const renderComponent = (open = true) => {
    return render(
      <PatientForm
        open={open}
        onClose={mockOnClose}
        onSuccess={mockOnSuccess}
      />
    );
  };

  it('should not render when open is false', () => {
    renderComponent(false);
    expect(screen.queryByText('Register New Patient')).not.toBeInTheDocument();
  });

  it('should render form when open is true', () => {
    renderComponent(true);
    expect(screen.getByText('Register New Patient')).toBeInTheDocument();
    expect(screen.getByLabelText(/first name/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/last name/i)).toBeInTheDocument();
  });

  it('should submit form with valid data', async () => {
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
      createdAt: '2025-01-01T00:00:00Z',
    };

    vi.mocked(patientService.createPatient).mockResolvedValue(mockPatient);

    renderComponent();

    fireEvent.change(screen.getByLabelText(/first name/i), {
      target: { value: 'John' },
    });
    fireEvent.change(screen.getByLabelText(/last name/i), {
      target: { value: 'Doe' },
    });
    fireEvent.change(screen.getByLabelText(/date of birth/i), {
      target: { value: '1990-01-01' },
    });
    fireEvent.change(screen.getByLabelText(/phone number/i), {
      target: { value: '1234567890' },
    });

    const submitButton = screen.getByText('Create Patient');
    fireEvent.click(submitButton);

    await waitFor(() => {
      expect(patientService.createPatient).toHaveBeenCalled();
    });

    await waitFor(() => {
      expect(mockOnSuccess).toHaveBeenCalled();
      expect(mockOnClose).toHaveBeenCalled();
    });
  });

  it('should display error message when creation fails', async () => {
    vi.mocked(patientService.createPatient).mockRejectedValue(new Error('API Error'));

    renderComponent();

    fireEvent.change(screen.getByLabelText(/first name/i), {
      target: { value: 'John' },
    });
    fireEvent.change(screen.getByLabelText(/last name/i), {
      target: { value: 'Doe' },
    });
    fireEvent.change(screen.getByLabelText(/date of birth/i), {
      target: { value: '1990-01-01' },
    });
    fireEvent.change(screen.getByLabelText(/phone number/i), {
      target: { value: '1234567890' },
    });

    const submitButton = screen.getByText('Create Patient');
    fireEvent.click(submitButton);

    await waitFor(() => {
      expect(screen.getByText(/failed to create patient/i)).toBeInTheDocument();
    });
  });

  it('should close dialog when cancel is clicked', () => {
    renderComponent();

    const cancelButton = screen.getByText('Cancel');
    fireEvent.click(cancelButton);

    expect(mockOnClose).toHaveBeenCalled();
  });

  it('should update form fields correctly', () => {
    renderComponent();

    const firstNameInput = screen.getByLabelText(/first name/i) as HTMLInputElement;
    fireEvent.change(firstNameInput, { target: { value: 'Jane' } });

    expect(firstNameInput.value).toBe('Jane');
  });
});

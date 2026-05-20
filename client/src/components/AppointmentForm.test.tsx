import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { AppointmentForm } from '../components/AppointmentForm';
import { appointmentService } from '../services/appointment.service';
import { patientService } from '../services/patient.service';
import { AppointmentStatus } from '../types/appointment.types';
import { Gender } from '../types/patient.types';

vi.mock('../services/appointment.service');
vi.mock('../services/patient.service');

describe('AppointmentForm', () => {
  const mockOnClose = vi.fn();
  const mockOnSuccess = vi.fn();

  beforeEach(() => {
    vi.clearAllMocks();
  });

  const renderComponent = (props = {}) => {
    return render(
      <AppointmentForm
        open={true}
        onClose={mockOnClose}
        onSuccess={mockOnSuccess}
        {...props}
      />
    );
  };

  it('should not render when open is false', () => {
    render(
      <AppointmentForm
        open={false}
        onClose={mockOnClose}
        onSuccess={mockOnSuccess}
      />
    );
    expect(screen.queryByText('Schedule Appointment')).not.toBeInTheDocument();
  });

  it('should render form when open is true', async () => {
    vi.mocked(patientService.getAllPatients).mockResolvedValue([]);

    renderComponent();

    await waitFor(() => {
      expect(screen.getByRole('heading', { name: /schedule appointment/i })).toBeInTheDocument();
    });

    expect(screen.getByLabelText(/patient/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/scheduled date & time/i)).toBeInTheDocument();
  });

  it('should load patients on open', async () => {
    const mockPatients = [
      {
        id: '1',
        firstName: 'John',
        lastName: 'Doe',
        fullName: 'John Doe',
        dateOfBirth: '1990-01-01T00:00:00Z',
        age: 36,
        gender: Gender.Male,
        phoneNumber: '1234567890',
        createdAt: '2026-01-01T00:00:00Z',
      },
    ];

    vi.mocked(patientService.getAllPatients).mockResolvedValue(mockPatients);

    renderComponent();

    await waitFor(() => {
      expect(patientService.getAllPatients).toHaveBeenCalled();
    });
  });

  it('should submit form with valid data', async () => {
    const mockPatients = [
      {
        id: 'patient-123',
        firstName: 'John',
        lastName: 'Doe',
        fullName: 'John Doe',
        dateOfBirth: '1990-01-01T00:00:00Z',
        age: 36,
        gender: Gender.Male,
        phoneNumber: '1234567890',
        createdAt: '2026-01-01T00:00:00Z',
      },
    ];

    const mockAppointment = {
      id: 'appointment-123',
      patientId: 'patient-123',
      scheduledDateTime: '2026-05-20T10:00:00',
      status: AppointmentStatus.Scheduled,
      reason: 'Checkup',
      notes: 'Annual physical',
      createdAt: '2026-05-19T00:00:00Z',
    };

    vi.mocked(patientService.getAllPatients).mockResolvedValue(mockPatients);
    vi.mocked(appointmentService.createAppointment).mockResolvedValue(mockAppointment);

    renderComponent({
      selectedPatientId: 'patient-123',
      selectedDateTime: '2026-05-20T10:00:00',
    });

    await waitFor(() => {
      expect(screen.getByRole('heading', { name: /schedule appointment/i })).toBeInTheDocument();
    });

    const reasonInput = screen.getByLabelText(/reason for visit/i);
    fireEvent.change(reasonInput, { target: { value: 'Checkup' } });

    const notesInput = screen.getByLabelText(/notes/i);
    fireEvent.change(notesInput, { target: { value: 'Annual physical' } });

    const submitButton = screen.getByRole('button', { name: /schedule appointment/i });
    fireEvent.click(submitButton);

    await waitFor(() => {
      expect(appointmentService.createAppointment).toHaveBeenCalledWith({
        patientId: 'patient-123',
        scheduledDateTime: '2026-05-20T10:00:00',
        reason: 'Checkup',
        notes: 'Annual physical',
      });
    });

    await waitFor(() => {
      expect(mockOnSuccess).toHaveBeenCalledWith(mockAppointment);
      expect(mockOnClose).toHaveBeenCalled();
    });
  });

  it('should display error message when creation fails', async () => {
    vi.mocked(patientService.getAllPatients).mockResolvedValue([]);
    vi.mocked(appointmentService.createAppointment).mockRejectedValue(new Error('API Error'));

    renderComponent({
      selectedPatientId: 'patient-123',
      selectedDateTime: '2026-05-20T10:00:00',
    });

    await waitFor(() => {
      expect(screen.getByRole('heading', { name: /schedule appointment/i })).toBeInTheDocument();
    });

    const submitButton = screen.getByRole('button', { name: /schedule appointment/i });
    fireEvent.click(submitButton);

    await waitFor(() => {
      expect(
        screen.getByText(/failed to create appointment/i)
      ).toBeInTheDocument();
    });
  });

  it('should close dialog when cancel is clicked', async () => {
    vi.mocked(patientService.getAllPatients).mockResolvedValue([]);

    renderComponent();

    await waitFor(() => {
      expect(screen.getByRole('heading', { name: /schedule appointment/i })).toBeInTheDocument();
    });

    const cancelButton = screen.getByRole('button', { name: /cancel/i });
    fireEvent.click(cancelButton);

    expect(mockOnClose).toHaveBeenCalled();
  });
});

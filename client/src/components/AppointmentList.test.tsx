import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { AppointmentList } from '../components/AppointmentList';
import { appointmentService } from '../services/appointment.service';
import { AppointmentStatus } from '../types/appointment.types';

vi.mock('../services/appointment.service');

describe('AppointmentList', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  const mockAppointments = [
    {
      id: '1',
      patientId: 'patient-1',
      patient: {
        id: 'patient-1',
        fullName: 'John Doe',
        age: 36,
        phoneNumber: '1234567890',
      },
      scheduledDateTime: '2026-05-20T10:00:00Z',
      status: AppointmentStatus.Scheduled,
      reason: 'Checkup',
      notes: 'Annual physical',
      createdAt: '2026-05-19T00:00:00Z',
    },
    {
      id: '2',
      patientId: 'patient-2',
      patient: {
        id: 'patient-2',
        fullName: 'Jane Smith',
        age: 28,
        phoneNumber: '9876543210',
      },
      scheduledDateTime: '2026-05-20T14:00:00Z',
      status: AppointmentStatus.CheckedIn,
      reason: 'Follow-up',
      createdAt: '2026-05-19T00:00:00Z',
    },
  ];

  const renderComponent = (props = {}) => {
    return render(
      <AppointmentList
        selectedDate="2026-05-20"
        {...props}
      />
    );
  };

  it('should display loading state initially', () => {
    vi.mocked(appointmentService.getAppointmentsByDate).mockImplementation(
      () => new Promise(() => {})
    );

    renderComponent();
    expect(screen.getByRole('progressbar')).toBeInTheDocument();
  });

  it('should load and display appointments', async () => {
    vi.mocked(appointmentService.getAppointmentsByDate).mockResolvedValue(mockAppointments);

    renderComponent();

    await waitFor(() => {
      expect(screen.getByText('John Doe')).toBeInTheDocument();
      expect(screen.getByText('Jane Smith')).toBeInTheDocument();
    });

    expect(screen.getByText('Checkup')).toBeInTheDocument();
    expect(screen.getByText('Follow-up')).toBeInTheDocument();
  });

  it('should display "no appointments" message when list is empty', async () => {
    vi.mocked(appointmentService.getAppointmentsByDate).mockResolvedValue([]);

    renderComponent();

    await waitFor(() => {
      expect(screen.getByText(/no appointments scheduled/i)).toBeInTheDocument();
    });
  });

  it('should display error message when load fails', async () => {
    vi.mocked(appointmentService.getAppointmentsByDate).mockRejectedValue(
      new Error('API Error')
    );

    renderComponent();

    await waitFor(() => {
      expect(screen.getByText(/failed to load appointments/i)).toBeInTheDocument();
    });
  });

  it('should update appointment status', async () => {
    vi.mocked(appointmentService.getAppointmentsByDate).mockResolvedValue(mockAppointments);

    const updatedAppointment = {
      ...mockAppointments[0],
      status: AppointmentStatus.CheckedIn,
      checkInTime: '2026-05-20T10:05:00Z',
    };

    vi.mocked(appointmentService.updateAppointmentStatus).mockResolvedValue(updatedAppointment);

    const mockOnStatusChange = vi.fn();
    renderComponent({ onStatusChange: mockOnStatusChange });

    await waitFor(() => {
      expect(screen.getByText('John Doe')).toBeInTheDocument();
    });

    // Find and click the menu button for the first appointment
    const menuButtons = screen.getAllByRole('button');
    const firstMenuButton = menuButtons.find((button) => button.querySelector('svg'));
    if (firstMenuButton) {
      fireEvent.click(firstMenuButton);
    }

    await waitFor(() => {
      const checkInOption = screen.getByText('Check In');
      expect(checkInOption).toBeInTheDocument();
    });

    const checkInOption = screen.getByText('Check In');
    fireEvent.click(checkInOption);

    await waitFor(() => {
      expect(appointmentService.updateAppointmentStatus).toHaveBeenCalledWith('1', {
        status: AppointmentStatus.CheckedIn,
        checkInTime: expect.any(String),
        completedTime: undefined,
      });
    });

    await waitFor(() => {
      expect(mockOnStatusChange).toHaveBeenCalledWith(updatedAppointment);
    });
  });

  it('should call onAppointmentClick when row is clicked', async () => {
    vi.mocked(appointmentService.getAppointmentsByDate).mockResolvedValue(mockAppointments);

    const mockOnClick = vi.fn();
    renderComponent({ onAppointmentClick: mockOnClick });

    await waitFor(() => {
      expect(screen.getByText('John Doe')).toBeInTheDocument();
    });

    const firstRow = screen.getByText('John Doe').closest('tr');
    if (firstRow) {
      fireEvent.click(firstRow);
    }

    expect(mockOnClick).toHaveBeenCalledWith(mockAppointments[0]);
  });

  it('should reload appointments when date changes', async () => {
    vi.mocked(appointmentService.getAppointmentsByDate).mockResolvedValue(mockAppointments);

    const { rerender } = renderComponent({ selectedDate: '2026-05-20' });

    await waitFor(() => {
      expect(appointmentService.getAppointmentsByDate).toHaveBeenCalledWith('2026-05-20');
    });

    rerender(
      <AppointmentList
        selectedDate="2026-05-21"
      />
    );

    await waitFor(() => {
      expect(appointmentService.getAppointmentsByDate).toHaveBeenCalledWith('2026-05-21');
    });
  });

  it('should display correct status colors', async () => {
    const appointmentsWithVariousStatuses = [
      { ...mockAppointments[0], status: AppointmentStatus.Scheduled },
      { ...mockAppointments[1], status: AppointmentStatus.Completed },
    ];

    vi.mocked(appointmentService.getAppointmentsByDate).mockResolvedValue(
      appointmentsWithVariousStatuses
    );

    renderComponent();

    await waitFor(() => {
      expect(screen.getByText('Scheduled')).toBeInTheDocument();
      expect(screen.getByText('Completed')).toBeInTheDocument();
    });
  });
});

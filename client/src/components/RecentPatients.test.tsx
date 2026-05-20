import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { BrowserRouter } from 'react-router-dom';
import { RecentPatients } from '../components/RecentPatients';
import { patientService } from '../services/patient.service';
import { Gender } from '../types/patient.types';

vi.mock('../services/patient.service');

const mockNavigate = vi.fn();
vi.mock('react-router-dom', async () => {
  const actual = await vi.importActual('react-router-dom');
  return {
    ...actual,
    useNavigate: () => mockNavigate,
  };
});

describe('RecentPatients', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  const renderComponent = () => {
    return render(
      <BrowserRouter>
        <RecentPatients />
      </BrowserRouter>
    );
  };

  it('should display loading state initially', () => {
    vi.mocked(patientService.getAllPatients).mockImplementation(
      () => new Promise(() => {})
    );

    renderComponent();
    expect(screen.getByRole('progressbar')).toBeInTheDocument();
  });

  it('should display recent patients', async () => {
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
        createdAt: '2025-01-15T00:00:00Z',
      },
      {
        id: '2',
        firstName: 'Jane',
        lastName: 'Smith',
        fullName: 'Jane Smith',
        dateOfBirth: '1995-05-15T00:00:00Z',
        age: 29,
        gender: Gender.Female,
        phoneNumber: '9876543210',
        email: 'jane@example.com',
        createdAt: '2025-01-14T00:00:00Z',
      },
    ];

    vi.mocked(patientService.getAllPatients).mockResolvedValue(mockPatients);

    renderComponent();

    await waitFor(() => {
      expect(screen.getByText('John Doe')).toBeInTheDocument();
      expect(screen.getByText('Jane Smith')).toBeInTheDocument();
    });
  });

  it('should display "No recent patients" when list is empty', async () => {
    vi.mocked(patientService.getAllPatients).mockResolvedValue([]);

    renderComponent();

    await waitFor(() => {
      expect(screen.getByText(/no recent patients/i)).toBeInTheDocument();
    });
  });

  it('should navigate to patient detail when patient is clicked', async () => {
    const mockPatients = [
      {
        id: '123',
        firstName: 'John',
        lastName: 'Doe',
        fullName: 'John Doe',
        dateOfBirth: '1990-01-01T00:00:00Z',
        age: 34,
        gender: Gender.Male,
        phoneNumber: '1234567890',
        email: 'john@example.com',
        createdAt: '2025-01-15T00:00:00Z',
      },
    ];

    vi.mocked(patientService.getAllPatients).mockResolvedValue(mockPatients);

    renderComponent();

    await waitFor(() => {
      expect(screen.getByText('John Doe')).toBeInTheDocument();
    });

    const patientItem = screen.getByText('John Doe');
    fireEvent.click(patientItem);

    expect(mockNavigate).toHaveBeenCalledWith('/patients/123');
  });

  it('should display only top 5 recent patients', async () => {
    const mockPatients = Array.from({ length: 10 }, (_, i) => ({
      id: `${i + 1}`,
      firstName: `Patient${i + 1}`,
      lastName: 'Test',
      fullName: `Patient${i + 1} Test`,
      dateOfBirth: '1990-01-01T00:00:00Z',
      age: 34,
      gender: Gender.Male,
      phoneNumber: '1234567890',
      email: `patient${i + 1}@example.com`,
      createdAt: new Date(2025, 0, 15 - i).toISOString(),
    }));

    vi.mocked(patientService.getAllPatients).mockResolvedValue(mockPatients);

    renderComponent();

    await waitFor(() => {
      expect(screen.getByText('Patient1 Test')).toBeInTheDocument();
    });

    // Should show first 5 patients
    expect(screen.getByText('Patient1 Test')).toBeInTheDocument();
    expect(screen.getByText('Patient5 Test')).toBeInTheDocument();
    
    // Should not show 6th patient
    expect(screen.queryByText('Patient6 Test')).not.toBeInTheDocument();
  });

  it('should format dates correctly', async () => {
    const now = new Date();
    // Set time to start of today to ensure consistent date comparison
    const today = new Date(now.getFullYear(), now.getMonth(), now.getDate());
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
        createdAt: today.toISOString(),
      },
    ];

    vi.mocked(patientService.getAllPatients).mockResolvedValue(mockPatients);

    renderComponent();

    await waitFor(() => {
      // The component should show either "Today" or a date within last 24 hours
      expect(screen.getByText(/Today|\d+ days? ago/)).toBeInTheDocument();
    });
  });
});

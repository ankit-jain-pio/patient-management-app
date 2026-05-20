import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { BrowserRouter } from 'react-router-dom';
import { PatientSearch } from '../components/PatientSearch';
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

describe('PatientSearch', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  const renderComponent = () => {
    return render(
      <BrowserRouter>
        <PatientSearch />
      </BrowserRouter>
    );
  };

  it('should render search input', () => {
    renderComponent();
    expect(screen.getByPlaceholderText(/search by name or phone/i)).toBeInTheDocument();
  });

  it('should perform search when Enter is pressed', async () => {
    const mockResults = [
      {
        id: '1',
        firstName: 'John',
        lastName: 'Doe',
        fullName: 'John Doe',
        age: 34,
        gender: Gender.Male,
        phoneNumber: '1234567890',
        lastVisit: '2025-01-01T00:00:00Z',
        totalVisits: 5,
      },
    ];

    vi.mocked(patientService.searchPatients).mockResolvedValue(mockResults);

    renderComponent();

    const searchInput = screen.getByPlaceholderText(/search by name or phone/i);
    fireEvent.change(searchInput, { target: { value: 'John' } });
    fireEvent.keyPress(searchInput, { key: 'Enter', code: 'Enter', charCode: 13 });

    await waitFor(() => {
      expect(patientService.searchPatients).toHaveBeenCalledWith({
        searchTerm: 'John',
        pageNumber: 1,
        pageSize: 20,
      });
    });

    await waitFor(() => {
      expect(screen.getByText('John Doe')).toBeInTheDocument();
    });
  });

  it('should display "No patients found" when search returns empty results', async () => {
    vi.mocked(patientService.searchPatients).mockResolvedValue([]);

    renderComponent();

    const searchInput = screen.getByPlaceholderText(/search by name or phone/i);
    fireEvent.change(searchInput, { target: { value: 'NonExistent' } });
    fireEvent.keyPress(searchInput, { key: 'Enter', code: 'Enter', charCode: 13 });

    await waitFor(() => {
      expect(screen.getByText(/no patients found matching "NonExistent"/i)).toBeInTheDocument();
    });
  });

  it('should navigate to patient detail when view button is clicked', async () => {
    const mockResults = [
      {
        id: '123',
        firstName: 'Jane',
        lastName: 'Smith',
        fullName: 'Jane Smith',
        age: 29,
        gender: Gender.Female,
        phoneNumber: '9876543210',
        lastVisit: '2025-01-15T00:00:00Z',
        totalVisits: 3,
      },
    ];

    vi.mocked(patientService.searchPatients).mockResolvedValue(mockResults);

    renderComponent();

    const searchInput = screen.getByPlaceholderText(/search by name or phone/i);
    fireEvent.change(searchInput, { target: { value: 'Jane' } });
    fireEvent.keyPress(searchInput, { key: 'Enter', code: 'Enter', charCode: 13 });

    await waitFor(() => {
      expect(screen.getByText('Jane Smith')).toBeInTheDocument();
    });

    const viewButton = screen.getByTitle('View Patient');
    fireEvent.click(viewButton);

    expect(mockNavigate).toHaveBeenCalledWith('/patients/123');
  });

  it('should display error message when search fails', async () => {
    vi.mocked(patientService.searchPatients).mockRejectedValue(new Error('API Error'));

    renderComponent();

    const searchInput = screen.getByPlaceholderText(/search by name or phone/i);
    fireEvent.change(searchInput, { target: { value: 'Error' } });
    fireEvent.keyPress(searchInput, { key: 'Enter', code: 'Enter', charCode: 13 });

    await waitFor(() => {
      expect(screen.getByText(/failed to search patients/i)).toBeInTheDocument();
    });
  });
});

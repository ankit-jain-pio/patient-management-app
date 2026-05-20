import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { PrescriptionForm } from '../components/PrescriptionForm';
import type { AddPrescriptionRequest } from '../types/consultation.types';

describe('PrescriptionForm', () => {
  const mockOnChange = vi.fn();

  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('should render add prescription form', () => {
    render(<PrescriptionForm prescriptions={[]} onChange={mockOnChange} />);

    expect(screen.getByLabelText(/medication name/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/dosage/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/frequency/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/duration \(days\)/i)).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /add/i })).toBeInTheDocument();
  });

  it('should display "no prescriptions" message when list is empty', () => {
    render(<PrescriptionForm prescriptions={[]} onChange={mockOnChange} />);

    expect(screen.getByText(/no prescriptions added yet/i)).toBeInTheDocument();
  });

  it('should disable Add button when required fields are empty', () => {
    render(<PrescriptionForm prescriptions={[]} onChange={mockOnChange} />);

    const addButton = screen.getByRole('button', { name: /add/i });
    expect(addButton).toBeDisabled();
  });

  it('should enable Add button when all required fields are filled', () => {
    render(<PrescriptionForm prescriptions={[]} onChange={mockOnChange} />);

    fireEvent.change(screen.getByLabelText(/medication name/i), {
      target: { value: 'Paracetamol' },
    });
    fireEvent.change(screen.getByLabelText(/dosage/i), {
      target: { value: '500mg' },
    });
    fireEvent.change(screen.getByLabelText(/frequency/i), {
      target: { value: '3 times daily' },
    });
    fireEvent.change(screen.getByLabelText(/duration \(days\)/i), {
      target: { value: '5' },
    });

    const addButton = screen.getByRole('button', { name: /add/i });
    expect(addButton).not.toBeDisabled();
  });

  it('should add prescription to list when Add button is clicked', async () => {
    render(<PrescriptionForm prescriptions={[]} onChange={mockOnChange} />);

    fireEvent.change(screen.getByLabelText(/medication name/i), {
      target: { value: 'Paracetamol' },
    });
    fireEvent.change(screen.getByLabelText(/dosage/i), {
      target: { value: '500mg' },
    });
    fireEvent.change(screen.getByLabelText(/frequency/i), {
      target: { value: '3 times daily' },
    });
    fireEvent.change(screen.getByLabelText(/duration \(days\)/i), {
      target: { value: '5' },
    });
    fireEvent.change(screen.getByLabelText(/instructions/i), {
      target: { value: 'Take after meals' },
    });

    const addButton = screen.getByRole('button', { name: /add/i });
    fireEvent.click(addButton);

    await waitFor(() => {
      expect(mockOnChange).toHaveBeenCalledWith([
        {
          medicationName: 'Paracetamol',
          dosage: '500mg',
          frequency: '3 times daily',
          durationInDays: 5,
          instructions: 'Take after meals',
        },
      ]);
    });
  });

  it('should display prescriptions in table', () => {
    const prescriptions: AddPrescriptionRequest[] = [
      {
        medicationName: 'Paracetamol',
        dosage: '500mg',
        frequency: '3 times daily',
        durationInDays: 5,
        instructions: 'Take after meals',
      },
      {
        medicationName: 'Amoxicillin',
        dosage: '250mg',
        frequency: '2 times daily',
        durationInDays: 7,
      },
    ];

    render(<PrescriptionForm prescriptions={prescriptions} onChange={mockOnChange} />);

    expect(screen.getByText('Paracetamol')).toBeInTheDocument();
    expect(screen.getByText('500mg')).toBeInTheDocument();
    expect(screen.getByText('3 times daily')).toBeInTheDocument();
    expect(screen.getByText('5 days')).toBeInTheDocument();
    expect(screen.getByText('Take after meals')).toBeInTheDocument();

    expect(screen.getByText('Amoxicillin')).toBeInTheDocument();
    expect(screen.getByText('250mg')).toBeInTheDocument();
    expect(screen.getByText('2 times daily')).toBeInTheDocument();
    expect(screen.getByText('7 days')).toBeInTheDocument();
  });

  it('should remove prescription when delete button is clicked', () => {
    const prescriptions: AddPrescriptionRequest[] = [
      {
        medicationName: 'Paracetamol',
        dosage: '500mg',
        frequency: '3 times daily',
        durationInDays: 5,
      },
    ];

    render(<PrescriptionForm prescriptions={prescriptions} onChange={mockOnChange} />);

    const deleteButton = screen.getByRole('button', { name: '' }); // IconButton has no accessible name
    fireEvent.click(deleteButton);

    expect(mockOnChange).toHaveBeenCalledWith([]);
  });

  it('should disable all inputs when disabled prop is true', () => {
    render(<PrescriptionForm prescriptions={[]} onChange={mockOnChange} disabled={true} />);

    expect(screen.getByLabelText(/medication name/i)).toBeDisabled();
    expect(screen.getByLabelText(/dosage/i)).toBeDisabled();
    expect(screen.getByLabelText(/frequency/i)).toBeDisabled();
    expect(screen.getByRole('button', { name: /add/i })).toBeDisabled();
  });
});

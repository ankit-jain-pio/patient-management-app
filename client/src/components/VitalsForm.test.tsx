import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, fireEvent } from '@testing-library/react';
import { VitalsForm } from '../components/VitalsForm';
import type { Vitals } from '../types/consultation.types';

describe('VitalsForm', () => {
  const mockOnChange = vi.fn();

  const defaultVitals: Vitals = {
    temperature: 37.2,
    bloodPressure: '120/80',
    pulseRate: 72,
    oxygenSaturation: 98,
    weight: 70,
    height: 175,
    respiratoryRate: '16/min',
    bmi: 22.9,
  };

  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('should render all vitals input fields', () => {
    render(<VitalsForm vitals={{}} onChange={mockOnChange} />);

    expect(screen.getByLabelText(/temperature/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/blood pressure/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/pulse rate/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/oxygen saturation/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/weight/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/height/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/respiratory rate/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/bmi/i)).toBeInTheDocument();
  });

  it('should display existing vitals data', () => {
    render(<VitalsForm vitals={defaultVitals} onChange={mockOnChange} />);

    expect(screen.getByLabelText(/temperature/i)).toHaveValue(37.2);
    expect(screen.getByLabelText(/blood pressure/i)).toHaveValue('120/80');
    expect(screen.getByLabelText(/pulse rate/i)).toHaveValue(72);
    expect(screen.getByLabelText(/oxygen saturation/i)).toHaveValue(98);
  });

  it('should call onChange when temperature is updated', () => {
    render(<VitalsForm vitals={{}} onChange={mockOnChange} />);

    const temperatureInput = screen.getByLabelText(/temperature/i);
    fireEvent.change(temperatureInput, { target: { value: '38.5' } });

    expect(mockOnChange).toHaveBeenCalledWith({ temperature: 38.5 });
  });

  it('should call onChange when blood pressure is updated', () => {
    render(<VitalsForm vitals={{}} onChange={mockOnChange} />);

    const bpInput = screen.getByLabelText(/blood pressure/i);
    fireEvent.change(bpInput, { target: { value: '130/85' } });

    expect(mockOnChange).toHaveBeenCalledWith({ bloodPressure: '130/85' });
  });

  it('should call onChange when pulse rate is updated', () => {
    render(<VitalsForm vitals={{}} onChange={mockOnChange} />);

    const pulseInput = screen.getByLabelText(/pulse rate/i);
    fireEvent.change(pulseInput, { target: { value: '80' } });

    expect(mockOnChange).toHaveBeenCalledWith({ pulseRate: 80 });
  });

  it('should disable all fields when disabled prop is true', () => {
    render(<VitalsForm vitals={{}} onChange={mockOnChange} disabled={true} />);

    expect(screen.getByLabelText(/temperature/i)).toBeDisabled();
    expect(screen.getByLabelText(/blood pressure/i)).toBeDisabled();
    expect(screen.getByLabelText(/pulse rate/i)).toBeDisabled();
  });

  it('should handle empty values correctly', () => {
    render(<VitalsForm vitals={defaultVitals} onChange={mockOnChange} />);

    const temperatureInput = screen.getByLabelText(/temperature/i);
    fireEvent.change(temperatureInput, { target: { value: '' } });

    expect(mockOnChange).toHaveBeenCalledWith({
      ...defaultVitals,
      temperature: undefined,
    });
  });
});

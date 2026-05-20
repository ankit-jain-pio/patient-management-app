import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, fireEvent } from '@testing-library/react';
import { ConsultationDetailModal } from '../components/ConsultationDetailModal';
import { exportService } from '../services/export.service';
import type { Consultation } from '../types/consultation.types';

vi.mock('../services/export.service');

describe('ConsultationDetailModal', () => {
  const mockOnClose = vi.fn();

  const mockConsultation: Consultation = {
    id: 'consultation-123',
    patientId: 'patient-123',
    patient: {
      id: 'patient-123',
      fullName: 'John Doe',
      age: 36,
      phoneNumber: '1234567890',
    },
    consultationDate: '2026-05-20T10:00:00Z',
    vitals: {
      temperature: 37.2,
      bloodPressure: '120/80',
      pulseRate: 72,
      oxygenSaturation: 98,
    },
    chiefComplaint: 'Headache',
    symptoms: 'Severe headache for 2 days',
    diagnosis: 'Migraine',
    clinicalNotes: 'Patient appears stable',
    treatmentPlan: 'Pain relief medication',
    followUpInstructions: 'Return if symptoms worsen',
    prescriptions: [
      {
        id: 'prescription-1',
        consultationId: 'consultation-123',
        medicationName: 'Ibuprofen',
        dosage: '400mg',
        frequency: '3 times daily',
        durationInDays: 5,
        instructions: 'Take after meals',
        prescribedDate: '2026-05-20T10:00:00Z',
      },
    ],
    createdAt: '2026-05-20T10:00:00Z',
  };

  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('should render consultation details when open', () => {
    render(
      <ConsultationDetailModal
        open={true}
        consultation={mockConsultation}
        onClose={mockOnClose}
      />
    );

    expect(screen.getByText('Consultation Details')).toBeInTheDocument();
    expect(screen.getByText('John Doe | Age: 36 | Phone: 1234567890')).toBeInTheDocument();
    expect(screen.getByText('Headache')).toBeInTheDocument();
    expect(screen.getByText('Migraine')).toBeInTheDocument();
  });

  it('should display vitals information', () => {
    render(
      <ConsultationDetailModal
        open={true}
        consultation={mockConsultation}
        onClose={mockOnClose}
      />
    );

    expect(screen.getByText(/Temperature: 37.2°C/)).toBeInTheDocument();
    expect(screen.getByText(/BP: 120\/80/)).toBeInTheDocument();
    expect(screen.getByText(/Pulse: 72 bpm/)).toBeInTheDocument();
    expect(screen.getByText(/O2: 98%/)).toBeInTheDocument();
  });

  it('should display prescriptions in table', () => {
    render(
      <ConsultationDetailModal
        open={true}
        consultation={mockConsultation}
        onClose={mockOnClose}
      />
    );

    expect(screen.getByText('Prescriptions (1)')).toBeInTheDocument();
    expect(screen.getByText('Ibuprofen')).toBeInTheDocument();
    expect(screen.getByText('400mg')).toBeInTheDocument();
    expect(screen.getByText('3 times daily')).toBeInTheDocument();
    expect(screen.getByText('5 days')).toBeInTheDocument();
    expect(screen.getByText('Take after meals')).toBeInTheDocument();
  });

  it('should call onClose when Close button is clicked', () => {
    render(
      <ConsultationDetailModal
        open={true}
        consultation={mockConsultation}
        onClose={mockOnClose}
      />
    );

    const closeButton = screen.getByRole('button', { name: /close/i });
    fireEvent.click(closeButton);

    expect(mockOnClose).toHaveBeenCalled();
  });

  it('should download prescription PDF when Print Prescription button is clicked', () => {
    const mockDownloadFile = vi.fn();
    const mockGetPrescriptionPdfUrl = vi.fn().mockReturnValue('http://example.com/prescription.pdf');

    vi.mocked(exportService.downloadFile).mockImplementation(mockDownloadFile);
    vi.mocked(exportService.getPrescriptionPdfUrl).mockImplementation(mockGetPrescriptionPdfUrl);

    render(
      <ConsultationDetailModal
        open={true}
        consultation={mockConsultation}
        onClose={mockOnClose}
      />
    );

    const printButton = screen.getByRole('button', { name: /print prescription/i });
    fireEvent.click(printButton);

    expect(mockGetPrescriptionPdfUrl).toHaveBeenCalledWith('consultation-123');
    expect(mockDownloadFile).toHaveBeenCalledWith(
      'http://example.com/prescription.pdf',
      'prescription_consultation-123.pdf'
    );
  });

  it('should download summary PDF when Download Summary button is clicked', () => {
    const mockDownloadFile = vi.fn();
    const mockGetSummaryPdfUrl = vi.fn().mockReturnValue('http://example.com/summary.pdf');

    vi.mocked(exportService.downloadFile).mockImplementation(mockDownloadFile);
    vi.mocked(exportService.getConsultationSummaryPdfUrl).mockImplementation(mockGetSummaryPdfUrl);

    render(
      <ConsultationDetailModal
        open={true}
        consultation={mockConsultation}
        onClose={mockOnClose}
      />
    );

    const downloadButton = screen.getByRole('button', { name: /download summary/i });
    fireEvent.click(downloadButton);

    expect(mockGetSummaryPdfUrl).toHaveBeenCalledWith('consultation-123');
    expect(mockDownloadFile).toHaveBeenCalledWith(
      'http://example.com/summary.pdf',
      'consultation_consultation-123.pdf'
    );
  });

  it('should not render when open is false', () => {
    render(
      <ConsultationDetailModal
        open={false}
        consultation={mockConsultation}
        onClose={mockOnClose}
      />
    );

    expect(screen.queryByText('Consultation Details')).not.toBeInTheDocument();
  });
});

import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, fireEvent } from '@testing-library/react';
import { ExportButtons } from '../components/ExportButtons';
import { exportService } from '../services/export.service';

vi.mock('../services/export.service');

describe('ExportButtons', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('should render export patients and appointments buttons when no patientId', () => {
    render(<ExportButtons />);

    expect(screen.getByRole('button', { name: /export all patients/i })).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /export appointments/i })).toBeInTheDocument();
  });

  it('should render export patient history button when patientId is provided', () => {
    render(<ExportButtons patientId="patient-123" />);

    expect(screen.getByRole('button', { name: /export patient history/i })).toBeInTheDocument();
    expect(screen.queryByRole('button', { name: /export all patients/i })).not.toBeInTheDocument();
  });

  it('should call exportPatientsCsv when Export All Patients button is clicked', () => {
    const mockDownloadFile = vi.fn();
    const mockExportUrl = 'http://example.com/patients.csv';

    vi.mocked(exportService.downloadFile).mockImplementation(mockDownloadFile);
    vi.mocked(exportService.exportPatientsCsv).mockReturnValue(mockExportUrl);

    render(<ExportButtons />);

    const exportButton = screen.getByRole('button', { name: /export all patients/i });
    fireEvent.click(exportButton);

    expect(exportService.exportPatientsCsv).toHaveBeenCalled();
    expect(mockDownloadFile).toHaveBeenCalledWith(
      mockExportUrl,
      expect.stringMatching(/^patients_\d+\.csv$/)
    );
  });

  it('should call exportAppointmentsCsv when Export Appointments button is clicked', () => {
    const mockDownloadFile = vi.fn();
    const mockExportUrl = 'http://example.com/appointments.csv';

    vi.mocked(exportService.downloadFile).mockImplementation(mockDownloadFile);
    vi.mocked(exportService.exportAppointmentsCsv).mockReturnValue(mockExportUrl);

    const dateRange = { startDate: '2026-01-01', endDate: '2026-12-31' };
    render(<ExportButtons dateRange={dateRange} />);

    const exportButton = screen.getByRole('button', { name: /export appointments/i });
    fireEvent.click(exportButton);

    expect(exportService.exportAppointmentsCsv).toHaveBeenCalledWith(dateRange);
    expect(mockDownloadFile).toHaveBeenCalledWith(
      mockExportUrl,
      expect.stringMatching(/^appointments_\d+\.csv$/)
    );
  });

  it('should call exportPatientHistoryCsv when Export Patient History button is clicked', () => {
    const mockDownloadFile = vi.fn();
    const mockExportUrl = 'http://example.com/history.csv';

    vi.mocked(exportService.downloadFile).mockImplementation(mockDownloadFile);
    vi.mocked(exportService.exportPatientHistoryCsv).mockReturnValue(mockExportUrl);

    const dateRange = { startDate: '2026-01-01', endDate: '2026-12-31' };
    render(<ExportButtons patientId="patient-123" dateRange={dateRange} />);

    const exportButton = screen.getByRole('button', { name: /export patient history/i });
    fireEvent.click(exportButton);

    expect(exportService.exportPatientHistoryCsv).toHaveBeenCalledWith('patient-123', dateRange);
    expect(mockDownloadFile).toHaveBeenCalledWith(
      mockExportUrl,
      expect.stringMatching(/^patient_history_patient-123_\d+\.csv$/)
    );
  });
});

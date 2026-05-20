import React from 'react';
import { Box, Button, Paper, Typography } from '@mui/material';
import { Download as DownloadIcon } from '@mui/icons-material';
import { exportService } from '../services/export.service';
import type { DateRangeFilter } from '../types/history.types';

interface ExportButtonsProps {
  patientId?: string;
  dateRange?: DateRangeFilter;
}

export const ExportButtons: React.FC<ExportButtonsProps> = ({ patientId, dateRange }) => {
  const handleExportPatients = () => {
    const url = exportService.exportPatientsCsv();
    exportService.downloadFile(url, `patients_${Date.now()}.csv`);
  };

  const handleExportAppointments = () => {
    const url = exportService.exportAppointmentsCsv(dateRange);
    exportService.downloadFile(url, `appointments_${Date.now()}.csv`);
  };

  const handleExportPatientHistory = () => {
    if (!patientId) return;
    const url = exportService.exportPatientHistoryCsv(patientId, dateRange);
    exportService.downloadFile(url, `patient_history_${patientId}_${Date.now()}.csv`);
  };

  return (
    <Paper sx={{ p: 2, mb: 3 }}>
      <Typography variant="h6" gutterBottom>
        Export Data
      </Typography>
      <Box sx={{ display: 'flex', gap: 2, flexWrap: 'wrap' }}>
        {!patientId && (
          <Button
            variant="outlined"
            startIcon={<DownloadIcon />}
            onClick={handleExportPatients}
          >
            Export All Patients (CSV)
          </Button>
        )}
        {!patientId && (
          <Button
            variant="outlined"
            startIcon={<DownloadIcon />}
            onClick={handleExportAppointments}
          >
            Export Appointments (CSV)
          </Button>
        )}
        {patientId && (
          <Button
            variant="contained"
            startIcon={<DownloadIcon />}
            onClick={handleExportPatientHistory}
          >
            Export Patient History (CSV)
          </Button>
        )}
      </Box>
    </Paper>
  );
};

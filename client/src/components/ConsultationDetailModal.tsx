import React from 'react';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Typography,
  Box,
  Divider,
  Chip,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
} from '@mui/material';
import { Print as PrintIcon, PictureAsPdf as PdfIcon } from '@mui/icons-material';
import { exportService } from '../services/export.service';
import type { Consultation } from '../types/consultation.types';

interface ConsultationDetailModalProps {
  open: boolean;
  consultation: Consultation;
  onClose: () => void;
}

export const ConsultationDetailModal: React.FC<ConsultationDetailModalProps> = ({
  open,
  consultation,
  onClose,
}) => {
  const formatDate = (dateString: string): string => {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  const handlePrintPrescription = () => {
    const url = exportService.getPrescriptionPdfUrl(consultation.id);
    exportService.downloadFile(url, `prescription_${consultation.id}.pdf`);
  };

  const handleDownloadSummary = () => {
    const url = exportService.getConsultationSummaryPdfUrl(consultation.id);
    exportService.downloadFile(url, `consultation_${consultation.id}.pdf`);
  };

  return (
    <Dialog open={open} onClose={onClose} maxWidth="md" fullWidth>
      <DialogTitle>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <Typography variant="h6">Consultation Details</Typography>
          <Typography variant="caption" color="text.secondary">
            {formatDate(consultation.consultationDate)}
          </Typography>
        </Box>
      </DialogTitle>

      <DialogContent dividers>
        {/* Patient Info */}
        {consultation.patient && (
          <Box sx={{ mb: 3 }}>
            <Typography variant="subtitle2" color="text.secondary" gutterBottom>
              Patient Information
            </Typography>
            <Typography variant="body1">
              {consultation.patient.fullName} | Age: {consultation.patient.age} | Phone:{' '}
              {consultation.patient.phoneNumber}
            </Typography>
          </Box>
        )}

        {/* Vitals */}
        {consultation.vitals && (
          <Box sx={{ mb: 3 }}>
            <Typography variant="subtitle1" gutterBottom>
              Vital Signs
            </Typography>
            <Box
              sx={{
                display: 'grid',
                gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))',
                gap: 1,
              }}
            >
              {consultation.vitals.temperature && (
                <Chip label={`Temperature: ${consultation.vitals.temperature}°C`} size="small" />
              )}
              {consultation.vitals.bloodPressure && (
                <Chip label={`BP: ${consultation.vitals.bloodPressure}`} size="small" />
              )}
              {consultation.vitals.pulseRate && (
                <Chip label={`Pulse: ${consultation.vitals.pulseRate} bpm`} size="small" />
              )}
              {consultation.vitals.oxygenSaturation && (
                <Chip label={`O2: ${consultation.vitals.oxygenSaturation}%`} size="small" />
              )}
              {consultation.vitals.weight && (
                <Chip label={`Weight: ${consultation.vitals.weight} kg`} size="small" />
              )}
              {consultation.vitals.height && (
                <Chip label={`Height: ${consultation.vitals.height} cm`} size="small" />
              )}
            </Box>
          </Box>
        )}

        <Divider sx={{ my: 2 }} />

        {/* Chief Complaint */}
        {consultation.chiefComplaint && (
          <Box sx={{ mb: 2 }}>
            <Typography variant="subtitle1" gutterBottom>
              Chief Complaint
            </Typography>
            <Typography variant="body2">{consultation.chiefComplaint}</Typography>
          </Box>
        )}

        {/* Symptoms */}
        {consultation.symptoms && (
          <Box sx={{ mb: 2 }}>
            <Typography variant="subtitle1" gutterBottom>
              Symptoms
            </Typography>
            <Typography variant="body2">{consultation.symptoms}</Typography>
          </Box>
        )}

        <Divider sx={{ my: 2 }} />

        {/* Diagnosis */}
        {consultation.diagnosis && (
          <Box sx={{ mb: 2 }}>
            <Typography variant="subtitle1" gutterBottom>
              Diagnosis
            </Typography>
            <Typography variant="body2">{consultation.diagnosis}</Typography>
          </Box>
        )}

        {/* Clinical Notes */}
        {consultation.clinicalNotes && (
          <Box sx={{ mb: 2 }}>
            <Typography variant="subtitle1" gutterBottom>
              Clinical Notes
            </Typography>
            <Typography variant="body2">{consultation.clinicalNotes}</Typography>
          </Box>
        )}

        {/* Treatment Plan */}
        {consultation.treatmentPlan && (
          <Box sx={{ mb: 2 }}>
            <Typography variant="subtitle1" gutterBottom>
              Treatment Plan
            </Typography>
            <Typography variant="body2">{consultation.treatmentPlan}</Typography>
          </Box>
        )}

        {/* Follow-up Instructions */}
        {consultation.followUpInstructions && (
          <Box sx={{ mb: 2 }}>
            <Typography variant="subtitle1" gutterBottom>
              Follow-up Instructions
            </Typography>
            <Typography variant="body2">{consultation.followUpInstructions}</Typography>
          </Box>
        )}

        {/* Next Visit Date */}
        {consultation.nextVisitDate && (
          <Box sx={{ mb: 2 }}>
            <Typography variant="subtitle1" gutterBottom>
              Next Visit Scheduled
            </Typography>
            <Typography variant="body2">{formatDate(consultation.nextVisitDate)}</Typography>
          </Box>
        )}

        <Divider sx={{ my: 2 }} />

        {/* Prescriptions */}
        {consultation.prescriptions && consultation.prescriptions.length > 0 && (
          <Box>
            <Typography variant="subtitle1" gutterBottom>
              Prescriptions ({consultation.prescriptions.length})
            </Typography>
            <TableContainer component={Paper} variant="outlined">
              <Table size="small">
                <TableHead>
                  <TableRow>
                    <TableCell>Medication</TableCell>
                    <TableCell>Dosage</TableCell>
                    <TableCell>Frequency</TableCell>
                    <TableCell>Duration</TableCell>
                    <TableCell>Instructions</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {consultation.prescriptions.map((prescription) => (
                    <TableRow key={prescription.id}>
                      <TableCell>{prescription.medicationName}</TableCell>
                      <TableCell>{prescription.dosage}</TableCell>
                      <TableCell>{prescription.frequency}</TableCell>
                      <TableCell>{prescription.durationInDays} days</TableCell>
                      <TableCell>{prescription.instructions || '-'}</TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </TableContainer>
          </Box>
        )}
      </DialogContent>

      <DialogActions>
        <Button onClick={handleDownloadSummary} startIcon={<PdfIcon />}>
          Download Summary
        </Button>
        {consultation.prescriptions && consultation.prescriptions.length > 0 && (
          <Button onClick={handlePrintPrescription} startIcon={<PrintIcon />} variant="outlined">
            Print Prescription
          </Button>
        )}
        <Button onClick={onClose}>Close</Button>
      </DialogActions>
    </Dialog>
  );
};

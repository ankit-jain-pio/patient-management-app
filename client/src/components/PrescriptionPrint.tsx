import React from 'react';
import { Box, Typography, Divider, Table, TableBody, TableCell, TableRow } from '@mui/material';
import type { Consultation } from '../types/consultation.types';

interface PrescriptionPrintProps {
  consultation: Consultation;
}

export const PrescriptionPrint: React.FC<PrescriptionPrintProps> = ({ consultation }) => {
  const formatDate = (dateString: string): string => {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
    });
  };

  return (
    <Box
      sx={{
        p: 4,
        maxWidth: '800px',
        margin: '0 auto',
        fontFamily: 'Arial, sans-serif',
        '@media print': {
          p: 2,
        },
      }}
    >
      {/* Clinic Header */}
      <Box sx={{ textAlign: 'center', mb: 3 }}>
        <Typography variant="h4" gutterBottom>
          Medical Clinic
        </Typography>
        <Typography variant="body2" color="text.secondary">
          123 Healthcare Avenue, Medical City
        </Typography>
        <Typography variant="body2" color="text.secondary">
          Phone: (555) 123-4567 | Email: info@medicalclinic.com
        </Typography>
      </Box>

      <Divider sx={{ my: 2 }} />

      {/* Prescription Header */}
      <Box sx={{ mb: 3 }}>
        <Typography variant="h5" gutterBottom>
          PRESCRIPTION
        </Typography>
        <Typography variant="body2">Date: {formatDate(consultation.consultationDate)}</Typography>
        <Typography variant="body2">Prescription ID: {consultation.id}</Typography>
      </Box>

      {/* Patient Information */}
      {consultation.patient && (
        <Box sx={{ mb: 3 }}>
          <Typography variant="h6" gutterBottom>
            Patient Information
          </Typography>
          <Table size="small">
            <TableBody>
              <TableRow>
                <TableCell sx={{ fontWeight: 'bold', width: '150px', border: 'none' }}>
                  Name:
                </TableCell>
                <TableCell sx={{ border: 'none' }}>{consultation.patient.fullName}</TableCell>
              </TableRow>
              <TableRow>
                <TableCell sx={{ fontWeight: 'bold', border: 'none' }}>Age:</TableCell>
                <TableCell sx={{ border: 'none' }}>{consultation.patient.age} years</TableCell>
              </TableRow>
              <TableRow>
                <TableCell sx={{ fontWeight: 'bold', border: 'none' }}>Phone:</TableCell>
                <TableCell sx={{ border: 'none' }}>{consultation.patient.phoneNumber}</TableCell>
              </TableRow>
            </TableBody>
          </Table>
        </Box>
      )}

      <Divider sx={{ my: 2 }} />

      {/* Diagnosis */}
      {consultation.diagnosis && (
        <Box sx={{ mb: 3 }}>
          <Typography variant="h6" gutterBottom>
            Diagnosis
          </Typography>
          <Typography variant="body1">{consultation.diagnosis}</Typography>
        </Box>
      )}

      {/* Prescriptions */}
      {consultation.prescriptions && consultation.prescriptions.length > 0 && (
        <Box sx={{ mb: 3 }}>
          <Typography variant="h6" gutterBottom>
            Medications Prescribed
          </Typography>
          {consultation.prescriptions.map((prescription, index) => (
            <Box
              key={prescription.id}
              sx={{
                mb: 2,
                p: 2,
                border: '1px solid #e0e0e0',
                borderRadius: 1,
              }}
            >
              <Typography variant="subtitle1" sx={{ fontWeight: 'bold' }}>
                {index + 1}. {prescription.medicationName}
              </Typography>
              <Typography variant="body2">
                <strong>Dosage:</strong> {prescription.dosage}
              </Typography>
              <Typography variant="body2">
                <strong>Frequency:</strong> {prescription.frequency}
              </Typography>
              <Typography variant="body2">
                <strong>Duration:</strong> {prescription.durationInDays} days
              </Typography>
              {prescription.instructions && (
                <Typography variant="body2">
                  <strong>Instructions:</strong> {prescription.instructions}
                </Typography>
              )}
            </Box>
          ))}
        </Box>
      )}

      {/* Follow-up Instructions */}
      {consultation.followUpInstructions && (
        <Box sx={{ mb: 3 }}>
          <Typography variant="h6" gutterBottom>
            Follow-up Instructions
          </Typography>
          <Typography variant="body2">{consultation.followUpInstructions}</Typography>
        </Box>
      )}

      {/* Next Visit */}
      {consultation.nextVisitDate && (
        <Box sx={{ mb: 3 }}>
          <Typography variant="body2">
            <strong>Next Visit:</strong> {formatDate(consultation.nextVisitDate)}
          </Typography>
        </Box>
      )}

      <Divider sx={{ my: 3 }} />

      {/* Doctor Signature */}
      <Box sx={{ mt: 4 }}>
        <Typography variant="body2">Doctor's Signature: ___________________________</Typography>
        <Typography variant="body2" sx={{ mt: 1 }}>
          Date: {formatDate(consultation.consultationDate)}
        </Typography>
      </Box>

      {/* Footer */}
      <Box sx={{ mt: 4, textAlign: 'center' }}>
        <Typography variant="caption" color="text.secondary">
          This prescription is valid for 30 days from the date of issue.
        </Typography>
      </Box>
    </Box>
  );
};

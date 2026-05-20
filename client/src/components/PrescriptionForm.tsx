import React, { useState } from 'react';
import {
  Box,
  TextField,
  Typography,
  Button,
  IconButton,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
} from '@mui/material';
import { Add as AddIcon, Delete as DeleteIcon } from '@mui/icons-material';
import type { AddPrescriptionRequest } from '../types/consultation.types';

interface PrescriptionFormProps {
  prescriptions: AddPrescriptionRequest[];
  onChange: (prescriptions: AddPrescriptionRequest[]) => void;
  disabled?: boolean;
}

const emptyPrescription: AddPrescriptionRequest = {
  medicationName: '',
  dosage: '',
  frequency: '',
  durationInDays: 0,
  instructions: '',
};

export const PrescriptionForm: React.FC<PrescriptionFormProps> = ({
  prescriptions,
  onChange,
  disabled = false,
}) => {
  const [currentPrescription, setCurrentPrescription] = useState<AddPrescriptionRequest>(emptyPrescription);

  const handleAddPrescription = () => {
    if (currentPrescription.medicationName && currentPrescription.dosage && currentPrescription.frequency) {
      onChange([...prescriptions, currentPrescription]);
      setCurrentPrescription(emptyPrescription);
    }
  };

  const handleRemovePrescription = (index: number) => {
    onChange(prescriptions.filter((_, i) => i !== index));
  };

  const handleCurrentChange = (field: keyof AddPrescriptionRequest) => (
    event: React.ChangeEvent<HTMLInputElement>
  ) => {
    const value = field === 'durationInDays' ? parseInt(event.target.value) || 0 : event.target.value;
    setCurrentPrescription({ ...currentPrescription, [field]: value });
  };

  return (
    <Box>
      <Typography variant="h6" gutterBottom>
        Prescriptions
      </Typography>

      {/* Add Prescription Form */}
      <Paper sx={{ p: 2, mb: 2 }}>
        <Typography variant="subtitle2" gutterBottom>
          Add Prescription
        </Typography>
        <Box
          sx={{
            display: 'grid',
            gridTemplateColumns: { xs: '1fr', sm: '1fr 1fr' },
            gap: 2,
          }}
        >
          <TextField
            fullWidth
            required
            label="Medication Name"
            placeholder="e.g., Amoxicillin"
            value={currentPrescription.medicationName}
            onChange={handleCurrentChange('medicationName')}
            disabled={disabled}
          />
          <TextField
            fullWidth
            required
            label="Dosage"
            placeholder="e.g., 500mg"
            value={currentPrescription.dosage}
            onChange={handleCurrentChange('dosage')}
            disabled={disabled}
          />
          <TextField
            fullWidth
            required
            label="Frequency"
            placeholder="e.g., 3 times daily"
            value={currentPrescription.frequency}
            onChange={handleCurrentChange('frequency')}
            disabled={disabled}
          />
          <TextField
            fullWidth
            required
            label="Duration (Days)"
            type="number"
            value={currentPrescription.durationInDays || ''}
            onChange={handleCurrentChange('durationInDays')}
            disabled={disabled}
            slotProps={{
              htmlInput: { min: '1', max: '365' },
            }}
          />
          <Box sx={{ gridColumn: { xs: '1', sm: '1 / -1' } }}>
            <TextField
              fullWidth
              label="Instructions"
              placeholder="e.g., Take after meals"
              value={currentPrescription.instructions}
              onChange={handleCurrentChange('instructions')}
              disabled={disabled}
              multiline
              rows={2}
            />
          </Box>
          <Box sx={{ gridColumn: { xs: '1', sm: '1 / -1' } }}>
            <Button
              fullWidth
              variant="contained"
              startIcon={<AddIcon />}
              onClick={handleAddPrescription}
              disabled={
                disabled ||
                !currentPrescription.medicationName ||
                !currentPrescription.dosage ||
                !currentPrescription.frequency ||
                !currentPrescription.durationInDays
              }
            >
              Add Prescription
            </Button>
          </Box>
        </Box>
      </Paper>

      {/* Prescriptions List */}
      {prescriptions.length > 0 && (
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Medication</TableCell>
                <TableCell>Dosage</TableCell>
                <TableCell>Frequency</TableCell>
                <TableCell>Duration</TableCell>
                <TableCell>Instructions</TableCell>
                <TableCell align="right">Action</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {prescriptions.map((prescription, index) => (
                <TableRow key={index}>
                  <TableCell>{prescription.medicationName}</TableCell>
                  <TableCell>{prescription.dosage}</TableCell>
                  <TableCell>{prescription.frequency}</TableCell>
                  <TableCell>{prescription.durationInDays} days</TableCell>
                  <TableCell>{prescription.instructions || '-'}</TableCell>
                  <TableCell align="right">
                    <IconButton
                      size="small"
                      onClick={() => handleRemovePrescription(index)}
                      disabled={disabled}
                      color="error"
                    >
                      <DeleteIcon />
                    </IconButton>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}

      {prescriptions.length === 0 && (
        <Typography variant="body2" color="text.secondary" sx={{ mt: 2, textAlign: 'center' }}>
          No prescriptions added yet. Add medications above.
        </Typography>
      )}
    </Box>
  );
};

import React, { useState, useEffect } from 'react';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
  CircularProgress,
  Alert,
  Box,
  Autocomplete,
} from '@mui/material';
import { appointmentService } from '../services/appointment.service';
import { patientService } from '../services/patient.service';
import type { CreateAppointmentRequest, Appointment } from '../types/appointment.types';
import type { Patient } from '../types/patient.types';

interface AppointmentFormProps {
  open: boolean;
  onClose: () => void;
  onSuccess: (appointment: Appointment) => void;
  selectedPatientId?: string;
  selectedDateTime?: string;
}

export const AppointmentForm: React.FC<AppointmentFormProps> = ({
  open,
  onClose,
  onSuccess,
  selectedPatientId,
  selectedDateTime,
}) => {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');
  const [patients, setPatients] = useState<Patient[]>([]);
  const [loadingPatients, setLoadingPatients] = useState(false);
  const [formData, setFormData] = useState<CreateAppointmentRequest>({
    patientId: selectedPatientId || '',
    scheduledDateTime: selectedDateTime || '',
    reason: '',
    notes: '',
  });

  useEffect(() => {
    if (open) {
      loadPatients();
    }
  }, [open]);

  useEffect(() => {
    if (selectedPatientId) {
      setFormData((prev) => ({ ...prev, patientId: selectedPatientId }));
    }
  }, [selectedPatientId]);

  useEffect(() => {
    if (selectedDateTime) {
      setFormData((prev) => ({ ...prev, scheduledDateTime: selectedDateTime }));
    }
  }, [selectedDateTime]);

  const loadPatients = async (): Promise<void> => {
    setLoadingPatients(true);
    try {
      const data = await patientService.getAllPatients();
      setPatients(data);
    } catch (err) {
      console.error('Failed to load patients:', err);
    } finally {
      setLoadingPatients(false);
    }
  };

  const handleChange = (field: keyof CreateAppointmentRequest, value: string): void => {
    setFormData((prev) => ({
      ...prev,
      [field]: value,
    }));
  };

  const handleSubmit = async (e: React.FormEvent): Promise<void> => {
    e.preventDefault();
    setIsLoading(true);
    setError('');

    try {
      const appointment = await appointmentService.createAppointment(formData);
      onSuccess(appointment);
      handleClose();
    } catch (err) {
      setError('Failed to create appointment. Please check all required fields.');
      console.error('Create appointment error:', err);
    } finally {
      setIsLoading(false);
    }
  };

  const handleClose = (): void => {
    if (!isLoading) {
      setFormData({
        patientId: '',
        scheduledDateTime: '',
        reason: '',
        notes: '',
      });
      setError('');
      onClose();
    }
  };

  const selectedPatient = patients.find((p) => p.id === formData.patientId);

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <form onSubmit={handleSubmit}>
        <DialogTitle>Schedule Appointment</DialogTitle>
        <DialogContent>
          {error && (
            <Alert severity="error" sx={{ mb: 2 }}>
              {error}
            </Alert>
          )}

          <Box sx={{ mt: 2, display: 'grid', gap: 2 }}>
            <Autocomplete
              options={patients}
              getOptionLabel={(option) => `${option.fullName} - ${option.phoneNumber}`}
              value={selectedPatient || null}
              onChange={(_, newValue) => {
                handleChange('patientId', newValue?.id || '');
              }}
              loading={loadingPatients}
              renderInput={(params) => (
                <TextField
                  {...params}
                  required
                  label="Patient"
                  placeholder="Search patient..."
                  slotProps={{
                    input: {
                      ...params.slotProps?.input,
                      endAdornment: (
                        <>
                          {loadingPatients ? <CircularProgress size={20} /> : null}
                          {params.slotProps?.input?.endAdornment}
                        </>
                      ),
                    },
                  }}
                />
              )}
              disabled={isLoading || !!selectedPatientId}
            />

            <TextField
              required
              fullWidth
              label="Scheduled Date & Time"
              type="datetime-local"
              slotProps={{
                inputLabel: { shrink: true },
              }}
              value={formData.scheduledDateTime}
              onChange={(e) => handleChange('scheduledDateTime', e.target.value)}
              disabled={isLoading}
            />

            <TextField
              fullWidth
              label="Reason for Visit"
              multiline
              rows={2}
              value={formData.reason}
              onChange={(e) => handleChange('reason', e.target.value)}
              disabled={isLoading}
              placeholder="e.g., Routine checkup, Follow-up consultation..."
            />

            <TextField
              fullWidth
              label="Notes"
              multiline
              rows={3}
              value={formData.notes}
              onChange={(e) => handleChange('notes', e.target.value)}
              disabled={isLoading}
              placeholder="Additional notes or instructions..."
            />
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose} disabled={isLoading}>
            Cancel
          </Button>
          <Button
            type="submit"
            variant="contained"
            disabled={isLoading}
            startIcon={isLoading ? <CircularProgress size={20} /> : null}
          >
            {isLoading ? 'Scheduling...' : 'Schedule Appointment'}
          </Button>
        </DialogActions>
      </form>
    </Dialog>
  );
};

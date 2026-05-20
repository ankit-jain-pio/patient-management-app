import React, { useState } from 'react';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
  Select,
  MenuItem,
  FormControl,
  InputLabel,
  CircularProgress,
  Alert,
  Box,
} from '@mui/material';
import { patientService } from '../services/patient.service';
import type { CreatePatientRequest } from '../types/patient.types';
import { Gender } from '../types/patient.types';

interface PatientFormProps {
  open: boolean;
  onClose: () => void;
  onSuccess: () => void;
}

export const PatientForm: React.FC<PatientFormProps> = ({ open, onClose, onSuccess }) => {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');
  const [formData, setFormData] = useState<CreatePatientRequest>({
    firstName: '',
    lastName: '',
    dateOfBirth: '',
    gender: Gender.Male,
    phoneNumber: '',
    email: '',
    address: {
      street: '',
      city: '',
      state: '',
      postalCode: '',
      country: 'India',
    },
    emergencyContactName: '',
    emergencyContactPhone: '',
    bloodGroup: '',
    allergies: '',
    medicalHistory: '',
  });

  const handleChange = (field: string, value: string | number): void => {
    setFormData((prev) => ({
      ...prev,
      [field]: value,
    }));
  };

  const handleAddressChange = (field: string, value: string): void => {
    setFormData((prev) => ({
      ...prev,
      address: {
        ...prev.address!,
        [field]: value,
      },
    }));
  };

  const handleSubmit = async (e: React.FormEvent): Promise<void> => {
    e.preventDefault();
    setIsLoading(true);
    setError('');

    try {
      await patientService.createPatient(formData);
      onSuccess();
      handleClose();
    } catch (err) {
      setError('Failed to create patient. Please check all required fields.');
      console.error('Create patient error:', err);
    } finally {
      setIsLoading(false);
    }
  };

  const handleClose = (): void => {
    if (!isLoading) {
      setFormData({
        firstName: '',
        lastName: '',
        dateOfBirth: '',
        gender: Gender.Male,
        phoneNumber: '',
        email: '',
        address: {
          street: '',
          city: '',
          state: '',
          postalCode: '',
          country: 'India',
        },
        emergencyContactName: '',
        emergencyContactPhone: '',
        bloodGroup: '',
        allergies: '',
        medicalHistory: '',
      });
      setError('');
      onClose();
    }
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="md" fullWidth>
      <form onSubmit={handleSubmit}>
        <DialogTitle>Register New Patient</DialogTitle>
        <DialogContent>
          {error && (
            <Alert severity="error" sx={{ mb: 2 }}>
              {error}
            </Alert>
          )}

          <Box sx={{ mt: 2, display: 'grid', gap: 2 }}>
            {/* Personal Information */}
            <Box sx={{ fontWeight: 'bold', mb: 1 }}>Personal Information</Box>
            <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', sm: '1fr 1fr' }, gap: 2 }}>
              <TextField
                required
                fullWidth
                label="First Name"
                value={formData.firstName}
                onChange={(e) => handleChange('firstName', e.target.value)}
                disabled={isLoading}
              />
              <TextField
                required
                fullWidth
                label="Last Name"
                value={formData.lastName}
                onChange={(e) => handleChange('lastName', e.target.value)}
                disabled={isLoading}
              />
              <TextField
                required
                fullWidth
                label="Date of Birth"
                type="date"
                slotProps={{
                  inputLabel: { shrink: true },
                }}
                value={formData.dateOfBirth}
                onChange={(e) => handleChange('dateOfBirth', e.target.value)}
                disabled={isLoading}
              />
              <FormControl fullWidth required>
                <InputLabel>Gender</InputLabel>
                <Select
                  value={formData.gender}
                  label="Gender"
                  onChange={(e) => handleChange('gender', e.target.value)}
                  disabled={isLoading}
                >
                  <MenuItem value={Gender.Male}>Male</MenuItem>
                  <MenuItem value={Gender.Female}>Female</MenuItem>
                  <MenuItem value={Gender.Other}>Other</MenuItem>
                </Select>
              </FormControl>
            </Box>

            {/* Contact Information */}
            <Box sx={{ fontWeight: 'bold', mt: 2, mb: 1 }}>Contact Information</Box>
            <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', sm: '1fr 1fr' }, gap: 2 }}>
              <TextField
                required
                fullWidth
                label="Phone Number"
                value={formData.phoneNumber}
                onChange={(e) => handleChange('phoneNumber', e.target.value)}
                disabled={isLoading}
              />
              <TextField
                fullWidth
                label="Email"
                type="email"
                value={formData.email}
                onChange={(e) => handleChange('email', e.target.value)}
                disabled={isLoading}
              />
            </Box>

            {/* Address */}
            <Box sx={{ fontWeight: 'bold', mt: 2, mb: 1 }}>Address</Box>
            <Box sx={{ display: 'grid', gap: 2 }}>
              <TextField
                fullWidth
                label="Street"
                value={formData.address?.street}
                onChange={(e) => handleAddressChange('street', e.target.value)}
                disabled={isLoading}
              />
              <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', sm: '1fr 1fr' }, gap: 2 }}>
                <TextField
                  fullWidth
                  label="City"
                  value={formData.address?.city}
                  onChange={(e) => handleAddressChange('city', e.target.value)}
                  disabled={isLoading}
                />
                <TextField
                  fullWidth
                  label="State"
                  value={formData.address?.state}
                  onChange={(e) => handleAddressChange('state', e.target.value)}
                  disabled={isLoading}
                />
                <TextField
                  fullWidth
                  label="Postal Code"
                  value={formData.address?.postalCode}
                  onChange={(e) => handleAddressChange('postalCode', e.target.value)}
                  disabled={isLoading}
                />
                <TextField
                  fullWidth
                  label="Country"
                  value={formData.address?.country}
                  onChange={(e) => handleAddressChange('country', e.target.value)}
                  disabled={isLoading}
                />
              </Box>
            </Box>

            {/* Emergency Contact */}
            <Box sx={{ fontWeight: 'bold', mt: 2, mb: 1 }}>Emergency Contact</Box>
            <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', sm: '1fr 1fr' }, gap: 2 }}>
              <TextField
                fullWidth
                label="Emergency Contact Name"
                value={formData.emergencyContactName}
                onChange={(e) => handleChange('emergencyContactName', e.target.value)}
                disabled={isLoading}
              />
              <TextField
                fullWidth
                label="Emergency Contact Phone"
                value={formData.emergencyContactPhone}
                onChange={(e) => handleChange('emergencyContactPhone', e.target.value)}
                disabled={isLoading}
              />
            </Box>

            {/* Medical Information */}
            <Box sx={{ fontWeight: 'bold', mt: 2, mb: 1 }}>Medical Information</Box>
            <Box sx={{ display: 'grid', gap: 2 }}>
              <TextField
                fullWidth
                label="Blood Group"
                value={formData.bloodGroup}
                onChange={(e) => handleChange('bloodGroup', e.target.value)}
                disabled={isLoading}
                sx={{ gridColumn: { xs: '1', sm: '1 / 2' } }}
              />
              <TextField
                fullWidth
                multiline
                rows={2}
                label="Allergies"
                value={formData.allergies}
                onChange={(e) => handleChange('allergies', e.target.value)}
                disabled={isLoading}
              />
              <TextField
                fullWidth
                multiline
                rows={3}
                label="Medical History"
                value={formData.medicalHistory}
                onChange={(e) => handleChange('medicalHistory', e.target.value)}
                disabled={isLoading}
              />
            </Box>
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
            {isLoading ? 'Creating...' : 'Create Patient'}
          </Button>
        </DialogActions>
      </form>
    </Dialog>
  );
};

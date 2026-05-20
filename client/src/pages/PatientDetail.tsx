import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Box,
  Paper,
  Typography,
  Button,
  TextField,
  Select,
  MenuItem,
  FormControl,
  InputLabel,
  CircularProgress,
  Alert,
  Divider,
  IconButton,
} from '@mui/material';
import {
  Edit as EditIcon,
  Save as SaveIcon,
  Cancel as CancelIcon,
  ArrowBack as ArrowBackIcon,
} from '@mui/icons-material';
import { patientService } from '../services/patient.service';
import type { Patient, UpdatePatientRequest } from '../types/patient.types';
import { Gender } from '../types/patient.types';

export const PatientDetail: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [patient, setPatient] = useState<Patient | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isEditing, setIsEditing] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  const [error, setError] = useState('');
  const [editData, setEditData] = useState<UpdatePatientRequest | null>(null);

  useEffect(() => {
    if (id) {
      loadPatient(id);
    }
  }, [id]);

  const loadPatient = async (patientId: string): Promise<void> => {
    setIsLoading(true);
    setError('');

    try {
      const data = await patientService.getPatientById(patientId);
      setPatient(data);
      setEditData({
        id: data.id,
        firstName: data.firstName,
        lastName: data.lastName,
        dateOfBirth: data.dateOfBirth.split('T')[0],
        gender: data.gender,
        phoneNumber: data.phoneNumber,
        email: data.email || '',
        address: data.address || {
          street: '',
          city: '',
          state: '',
          postalCode: '',
          country: 'India',
        },
        emergencyContactName: data.emergencyContactName || '',
        emergencyContactPhone: data.emergencyContactPhone || '',
        bloodGroup: data.bloodGroup || '',
        allergies: data.allergies || '',
        medicalHistory: data.medicalHistory || '',
      });
    } catch (err) {
      setError('Failed to load patient details.');
      console.error('Load patient error:', err);
    } finally {
      setIsLoading(false);
    }
  };

  const handleEdit = (): void => {
    setIsEditing(true);
  };

  const handleCancel = (): void => {
    setIsEditing(false);
    if (patient) {
      setEditData({
        id: patient.id,
        firstName: patient.firstName,
        lastName: patient.lastName,
        dateOfBirth: patient.dateOfBirth.split('T')[0],
        gender: patient.gender,
        phoneNumber: patient.phoneNumber,
        email: patient.email || '',
        address: patient.address || {
          street: '',
          city: '',
          state: '',
          postalCode: '',
          country: 'India',
        },
        emergencyContactName: patient.emergencyContactName || '',
        emergencyContactPhone: patient.emergencyContactPhone || '',
        bloodGroup: patient.bloodGroup || '',
        allergies: patient.allergies || '',
        medicalHistory: patient.medicalHistory || '',
      });
    }
  };

  const handleSave = async (): Promise<void> => {
    if (!id || !editData) return;

    setIsSaving(true);
    setError('');

    try {
      const updated = await patientService.updatePatient(id, editData);
      setPatient(updated);
      setIsEditing(false);
    } catch (err) {
      setError('Failed to update patient. Please check all fields.');
      console.error('Update patient error:', err);
    } finally {
      setIsSaving(false);
    }
  };

  const handleChange = (field: string, value: string | number): void => {
    if (!editData) return;
    setEditData({
      ...editData,
      [field]: value,
    });
  };

  const handleAddressChange = (field: string, value: string): void => {
    if (!editData) return;
    setEditData({
      ...editData,
      address: {
        ...editData.address!,
        [field]: value,
      },
    });
  };

  const getGenderLabel = (gender: Gender): string => {
    switch (gender) {
      case Gender.Male:
        return 'Male';
      case Gender.Female:
        return 'Female';
      case Gender.Other:
        return 'Other';
      default:
        return 'Unknown';
    }
  };

  if (isLoading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '50vh' }}>
        <CircularProgress />
      </Box>
    );
  }

  if (!patient || !editData) {
    return (
      <Box>
        <Alert severity="error">Patient not found</Alert>
        <Button startIcon={<ArrowBackIcon />} onClick={() => navigate('/patients')} sx={{ mt: 2 }}>
          Back to Patients
        </Button>
      </Box>
    );
  }

  return (
    <Box>
      <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 3 }}>
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
          <IconButton onClick={() => navigate('/patients')} size="small">
            <ArrowBackIcon />
          </IconButton>
          <Typography variant="h4">{patient.fullName}</Typography>
        </Box>
        {!isEditing ? (
          <Button startIcon={<EditIcon />} variant="contained" onClick={handleEdit}>
            Edit
          </Button>
        ) : (
          <Box sx={{ display: 'flex', gap: 1 }}>
            <Button
              startIcon={<CancelIcon />}
              onClick={handleCancel}
              disabled={isSaving}
            >
              Cancel
            </Button>
            <Button
              startIcon={isSaving ? <CircularProgress size={20} /> : <SaveIcon />}
              variant="contained"
              onClick={handleSave}
              disabled={isSaving}
            >
              {isSaving ? 'Saving...' : 'Save'}
            </Button>
          </Box>
        )}
      </Box>

      {error && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}

      <Paper sx={{ p: 3 }}>
        <Box sx={{ display: 'grid', gap: 3 }}>
          {/* Personal Information */}
          <Box>
            <Typography variant="h6" gutterBottom>
              Personal Information
            </Typography>
            <Divider />
          </Box>
          <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', sm: '1fr 1fr' }, gap: 3 }}>
            <Box>
              {isEditing ? (
                <TextField
                  fullWidth
                  label="First Name"
                  value={editData.firstName}
                  onChange={(e) => handleChange('firstName', e.target.value)}
                />
              ) : (
                <Box>
                  <Typography variant="caption" color="text.secondary">
                    First Name
                  </Typography>
                  <Typography variant="body1">{patient.firstName}</Typography>
                </Box>
              )}
            </Box>
            <Box>
              {isEditing ? (
                <TextField
                  fullWidth
                  label="Last Name"
                  value={editData.lastName}
                  onChange={(e) => handleChange('lastName', e.target.value)}
                />
              ) : (
                <Box>
                  <Typography variant="caption" color="text.secondary">
                    Last Name
                  </Typography>
                  <Typography variant="body1">{patient.lastName}</Typography>
                </Box>
              )}
            </Box>
            <Box>
              {isEditing ? (
                <TextField
                  fullWidth
                  label="Date of Birth"
                  type="date"
                  slotProps={{ inputLabel: { shrink: true } }}
                  value={editData.dateOfBirth}
                  onChange={(e) => handleChange('dateOfBirth', e.target.value)}
                />
              ) : (
                <Box>
                  <Typography variant="caption" color="text.secondary">
                    Date of Birth (Age)
                  </Typography>
                  <Typography variant="body1">
                    {new Date(patient.dateOfBirth).toLocaleDateString()} ({patient.age} years)
                  </Typography>
                </Box>
              )}
            </Box>
            <Box>
              {isEditing ? (
                <FormControl fullWidth>
                  <InputLabel>Gender</InputLabel>
                  <Select
                    value={editData.gender}
                    label="Gender"
                    onChange={(e) => handleChange('gender', e.target.value)}
                  >
                    <MenuItem value={Gender.Male}>Male</MenuItem>
                    <MenuItem value={Gender.Female}>Female</MenuItem>
                    <MenuItem value={Gender.Other}>Other</MenuItem>
                  </Select>
                </FormControl>
              ) : (
                <Box>
                  <Typography variant="caption" color="text.secondary">
                    Gender
                  </Typography>
                  <Typography variant="body1">{getGenderLabel(patient.gender)}</Typography>
                </Box>
              )}
            </Box>
          </Box>

          {/* Contact Information */}
          <Box sx={{ mt: 2 }}>
            <Typography variant="h6" gutterBottom>
              Contact Information
            </Typography>
            <Divider />
          </Box>
          <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', sm: '1fr 1fr' }, gap: 3 }}>
            <Box>
              {isEditing ? (
                <TextField
                  fullWidth
                  label="Phone Number"
                  value={editData.phoneNumber}
                  onChange={(e) => handleChange('phoneNumber', e.target.value)}
                />
              ) : (
                <Box>
                  <Typography variant="caption" color="text.secondary">
                    Phone Number
                  </Typography>
                  <Typography variant="body1">{patient.phoneNumber}</Typography>
                </Box>
              )}
            </Box>
            <Box>
              {isEditing ? (
                <TextField
                  fullWidth
                  label="Email"
                  type="email"
                  value={editData.email}
                  onChange={(e) => handleChange('email', e.target.value)}
                />
              ) : (
                <Box>
                  <Typography variant="caption" color="text.secondary">
                    Email
                  </Typography>
                  <Typography variant="body1">{patient.email || 'N/A'}</Typography>
                </Box>
              )}
            </Box>
          </Box>

          {/* Address */}
          <Box sx={{ mt: 2 }}>
            <Typography variant="h6" gutterBottom>
              Address
            </Typography>
            <Divider />
          </Box>
          {isEditing ? (
            <Box sx={{ display: 'grid', gap: 2 }}>
              <TextField
                fullWidth
                label="Street"
                value={editData.address?.street}
                onChange={(e) => handleAddressChange('street', e.target.value)}
              />
              <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', sm: '1fr 1fr' }, gap: 2 }}>
                <TextField
                  fullWidth
                  label="City"
                  value={editData.address?.city}
                  onChange={(e) => handleAddressChange('city', e.target.value)}
                />
                <TextField
                  fullWidth
                  label="State"
                  value={editData.address?.state}
                  onChange={(e) => handleAddressChange('state', e.target.value)}
                />
                <TextField
                  fullWidth
                  label="Postal Code"
                  value={editData.address?.postalCode}
                  onChange={(e) => handleAddressChange('postalCode', e.target.value)}
                />
                <TextField
                  fullWidth
                  label="Country"
                  value={editData.address?.country}
                  onChange={(e) => handleAddressChange('country', e.target.value)}
                />
              </Box>
            </Box>
          ) : (
            <Box>
              <Typography variant="body1">
                {patient.address
                  ? `${patient.address.street}, ${patient.address.city}, ${patient.address.state} ${patient.address.postalCode}`
                  : 'N/A'}
              </Typography>
            </Box>
          )}

          {/* Emergency Contact */}
          <Box sx={{ mt: 2 }}>
            <Typography variant="h6" gutterBottom>
              Emergency Contact
            </Typography>
            <Divider />
          </Box>
          <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', sm: '1fr 1fr' }, gap: 3 }}>
            <Box>
              {isEditing ? (
                <TextField
                  fullWidth
                  label="Emergency Contact Name"
                  value={editData.emergencyContactName}
                  onChange={(e) => handleChange('emergencyContactName', e.target.value)}
                />
              ) : (
                <Box>
                  <Typography variant="caption" color="text.secondary">
                    Name
                  </Typography>
                  <Typography variant="body1">
                    {patient.emergencyContactName || 'N/A'}
                  </Typography>
                </Box>
              )}
            </Box>
            <Box>
              {isEditing ? (
                <TextField
                  fullWidth
                  label="Emergency Contact Phone"
                  value={editData.emergencyContactPhone}
                  onChange={(e) => handleChange('emergencyContactPhone', e.target.value)}
                />
              ) : (
                <Box>
                  <Typography variant="caption" color="text.secondary">
                    Phone
                  </Typography>
                  <Typography variant="body1">
                    {patient.emergencyContactPhone || 'N/A'}
                  </Typography>
                </Box>
              )}
            </Box>
          </Box>

          {/* Medical Information */}
          <Box sx={{ mt: 2 }}>
            <Typography variant="h6" gutterBottom>
              Medical Information
            </Typography>
            <Divider />
          </Box>
          <Box sx={{ display: 'grid', gap: 3 }}>
            <Box sx={{ gridColumn: { xs: '1', sm: '1 / 2' } }}>
              {isEditing ? (
                <TextField
                  fullWidth
                  label="Blood Group"
                  value={editData.bloodGroup}
                  onChange={(e) => handleChange('bloodGroup', e.target.value)}
                />
              ) : (
                <Box>
                  <Typography variant="caption" color="text.secondary">
                    Blood Group
                  </Typography>
                  <Typography variant="body1">{patient.bloodGroup || 'N/A'}</Typography>
                </Box>
              )}
            </Box>
            <Box>
              {isEditing ? (
                <TextField
                  fullWidth
                  multiline
                  rows={2}
                  label="Allergies"
                  value={editData.allergies}
                  onChange={(e) => handleChange('allergies', e.target.value)}
                />
              ) : (
                <Box>
                  <Typography variant="caption" color="text.secondary">
                    Allergies
                  </Typography>
                  <Typography variant="body1">{patient.allergies || 'None'}</Typography>
                </Box>
              )}
            </Box>
            <Box>
              {isEditing ? (
                <TextField
                  fullWidth
                  multiline
                  rows={3}
                  label="Medical History"
                  value={editData.medicalHistory}
                  onChange={(e) => handleChange('medicalHistory', e.target.value)}
                />
              ) : (
                <Box>
                  <Typography variant="caption" color="text.secondary">
                    Medical History
                  </Typography>
                  <Typography variant="body1">{patient.medicalHistory || 'None'}</Typography>
                </Box>
              )}
            </Box>
          </Box>
        </Box>
      </Paper>
    </Box>
  );
};

import React, { useState } from 'react';
import {
  Paper,
  TextField,
  InputAdornment,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Typography,
  CircularProgress,
  Box,
  Chip,
  IconButton,
} from '@mui/material';
import { Search as SearchIcon, Visibility as VisibilityIcon } from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import { patientService } from '../services/patient.service';
import type { PatientSearchResult } from '../types/patient.types';
import { Gender } from '../types/patient.types';

export const PatientSearch: React.FC = () => {
  const navigate = useNavigate();
  const [searchTerm, setSearchTerm] = useState('');
  const [patients, setPatients] = useState<PatientSearchResult[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSearch = async (): Promise<void> => {
    if (!searchTerm.trim()) {
      setPatients([]);
      return;
    }

    setIsLoading(true);
    setError('');

    try {
      const results = await patientService.searchPatients({
        searchTerm: searchTerm.trim(),
        pageNumber: 1,
        pageSize: 20,
      });
      setPatients(results);
    } catch (err) {
      setError('Failed to search patients. Please try again.');
      console.error('Search error:', err);
    } finally {
      setIsLoading(false);
    }
  };

  const handleKeyPress = (e: React.KeyboardEvent): void => {
    if (e.key === 'Enter') {
      handleSearch();
    }
  };

  const handleViewPatient = (patientId: string): void => {
    navigate(`/patients/${patientId}`);
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

  const formatDate = (dateString: string): string => {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
    });
  };

  return (
    <Box>
      <Typography variant="h6" gutterBottom>
        Search Patients
      </Typography>
      <TextField
        fullWidth
        placeholder="Search by name or phone number..."
        value={searchTerm}
        onChange={(e) => setSearchTerm(e.target.value)}
        onKeyPress={handleKeyPress}
        slotProps={{
          input: {
            startAdornment: (
              <InputAdornment position="start">
                <SearchIcon />
              </InputAdornment>
            ),
          },
        }}
        sx={{ mb: 2 }}
      />

      {error && (
        <Typography color="error" sx={{ mb: 2 }}>
          {error}
        </Typography>
      )}

      {isLoading ? (
        <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}>
          <CircularProgress />
        </Box>
      ) : patients.length > 0 ? (
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Name</TableCell>
                <TableCell>Age</TableCell>
                <TableCell>Gender</TableCell>
                <TableCell>Phone</TableCell>
                <TableCell>Last Visit</TableCell>
                <TableCell>Total Visits</TableCell>
                <TableCell align="right">Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {patients.map((patient) => (
                <TableRow key={patient.id} hover>
                  <TableCell>{patient.fullName}</TableCell>
                  <TableCell>{patient.age}</TableCell>
                  <TableCell>
                    <Chip label={getGenderLabel(patient.gender)} size="small" />
                  </TableCell>
                  <TableCell>{patient.phoneNumber}</TableCell>
                  <TableCell>{formatDate(patient.lastVisit)}</TableCell>
                  <TableCell>{patient.totalVisits}</TableCell>
                  <TableCell align="right">
                    <IconButton
                      size="small"
                      color="primary"
                      onClick={() => handleViewPatient(patient.id)}
                      title="View Patient"
                    >
                      <VisibilityIcon />
                    </IconButton>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      ) : searchTerm && !isLoading ? (
        <Paper sx={{ p: 4, textAlign: 'center' }}>
          <Typography color="text.secondary">
            No patients found matching "{searchTerm}"
          </Typography>
        </Paper>
      ) : null}
    </Box>
  );
};

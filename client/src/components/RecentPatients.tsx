import React, { useState, useEffect } from 'react';
import {
  Paper,
  Typography,
  List,
  ListItem,
  ListItemButton,
  ListItemText,
  Box,
  CircularProgress,
  Chip,
} from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { patientService } from '../services/patient.service';
import type { Patient } from '../types/patient.types';

export const RecentPatients: React.FC = () => {
  const navigate = useNavigate();
  const [patients, setPatients] = useState<Patient[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    loadRecentPatients();
  }, []);

  const loadRecentPatients = async (): Promise<void> => {
    setIsLoading(true);
    try {
      const allPatients = await patientService.getAllPatients();
      // Get the most recent 5 patients by createdAt date
      const sorted = allPatients
        .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
        .slice(0, 5);
      setPatients(sorted);
    } catch (err) {
      console.error('Failed to load recent patients:', err);
    } finally {
      setIsLoading(false);
    }
  };

  const handlePatientClick = (patientId: string): void => {
    navigate(`/patients/${patientId}`);
  };

  const formatDate = (dateString: string): string => {
    const date = new Date(dateString);
    const now = new Date();
    
    // Reset time to midnight for accurate day comparison
    const dateOnly = new Date(date.getFullYear(), date.getMonth(), date.getDate());
    const nowOnly = new Date(now.getFullYear(), now.getMonth(), now.getDate());
    
    const diffTime = nowOnly.getTime() - dateOnly.getTime();
    const diffDays = Math.floor(diffTime / (1000 * 60 * 60 * 24));

    if (diffDays === 0) return 'Today';
    if (diffDays === 1) return 'Yesterday';
    if (diffDays < 7) return `${diffDays} days ago`;
    return date.toLocaleDateString('en-US', { month: 'short', day: 'numeric' });
  };

  return (
    <Paper sx={{ p: 2 }}>
      <Typography variant="h6" gutterBottom>
        Recent Patients
      </Typography>
      {isLoading ? (
        <Box sx={{ display: 'flex', justifyContent: 'center', p: 2 }}>
          <CircularProgress size={30} />
        </Box>
      ) : patients.length > 0 ? (
        <List disablePadding>
          {patients.map((patient) => (
            <ListItem key={patient.id} disablePadding divider>
              <ListItemButton onClick={() => handlePatientClick(patient.id)}>
                <ListItemText
                  primary={patient.fullName}
                  secondary={
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mt: 0.5 }}>
                      <Typography variant="caption" color="text.secondary">
                        {patient.phoneNumber}
                      </Typography>
                      <Chip
                        label={formatDate(patient.createdAt)}
                        size="small"
                        variant="outlined"
                        sx={{ height: 20, fontSize: '0.7rem' }}
                      />
                    </Box>
                  }
                />
              </ListItemButton>
            </ListItem>
          ))}
        </List>
      ) : (
        <Typography variant="body2" color="text.secondary" sx={{ textAlign: 'center', py: 2 }}>
          No recent patients
        </Typography>
      )}
    </Paper>
  );
};

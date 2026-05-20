import React from 'react';
import { Typography, Box, Paper } from '@mui/material';

export const Dashboard: React.FC = () => {
  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Dashboard
      </Typography>
      <Box
        sx={{
          display: 'grid',
          gridTemplateColumns: { xs: '1fr', md: 'repeat(2, 1fr)', lg: 'repeat(4, 1fr)' },
          gap: 3,
        }}
      >
        <Paper sx={{ p: 2 }}>
          <Typography variant="h6" color="text.secondary">
            Total Patients
          </Typography>
          <Typography variant="h3">0</Typography>
        </Paper>
        <Paper sx={{ p: 2 }}>
          <Typography variant="h6" color="text.secondary">
            Today's Appointments
          </Typography>
          <Typography variant="h3">0</Typography>
        </Paper>
        <Paper sx={{ p: 2 }}>
          <Typography variant="h6" color="text.secondary">
            Pending Consultations
          </Typography>
          <Typography variant="h3">0</Typography>
        </Paper>
        <Paper sx={{ p: 2 }}>
          <Typography variant="h6" color="text.secondary">
            Completed Today
          </Typography>
          <Typography variant="h3">0</Typography>
        </Paper>
      </Box>
    </Box>
  );
};

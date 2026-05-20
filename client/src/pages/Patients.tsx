import React, { useState } from 'react';
import { Box, Typography, Button } from '@mui/material';
import { Add as AddIcon } from '@mui/icons-material';
import { PatientSearch } from '../components/PatientSearch';
import { PatientForm } from '../components/PatientForm';
import { RecentPatients } from '../components/RecentPatients';

export const Patients: React.FC = () => {
  const [openForm, setOpenForm] = useState(false);
  const [refreshKey, setRefreshKey] = useState(0);

  const handleOpenForm = (): void => {
    setOpenForm(true);
  };

  const handleCloseForm = (): void => {
    setOpenForm(false);
  };

  const handleFormSuccess = (): void => {
    setRefreshKey((prev) => prev + 1);
  };

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography variant="h4">Patients</Typography>
        <Button
          variant="contained"
          startIcon={<AddIcon />}
          onClick={handleOpenForm}
        >
          New Patient
        </Button>
      </Box>

      <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', md: '2fr 1fr' }, gap: 3 }}>
        <Box>
          <PatientSearch key={refreshKey} />
        </Box>
        <Box>
          <RecentPatients key={refreshKey} />
        </Box>
      </Box>

      <PatientForm
        open={openForm}
        onClose={handleCloseForm}
        onSuccess={handleFormSuccess}
      />
    </Box>
  );
};

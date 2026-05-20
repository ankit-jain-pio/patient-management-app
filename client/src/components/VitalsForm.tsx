import React from 'react';
import { Box, TextField, Typography } from '@mui/material';
import type { Vitals } from '../types/consultation.types';

interface VitalsFormProps {
  vitals: Vitals;
  onChange: (vitals: Vitals) => void;
  disabled?: boolean;
}

export const VitalsForm: React.FC<VitalsFormProps> = ({ vitals, onChange, disabled = false }) => {
  const handleChange = (field: keyof Vitals) => (event: React.ChangeEvent<HTMLInputElement>) => {
    const value = event.target.value;
    onChange({
      ...vitals,
      [field]: value === '' ? undefined : field === 'bloodPressure' || field === 'respiratoryRate' ? value : parseFloat(value),
    });
  };

  return (
    <Box>
      <Typography variant="h6" gutterBottom>
        Vital Signs
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
          label="Temperature (°C)"
          type="number"
          value={vitals.temperature ?? ''}
          onChange={handleChange('temperature')}
          disabled={disabled}
          slotProps={{
            htmlInput: { step: '0.1', min: '35', max: '43' },
          }}
          helperText="Normal: 36.5-37.5°C"
        />
        <TextField
          fullWidth
          label="Blood Pressure"
          placeholder="120/80"
          value={vitals.bloodPressure ?? ''}
          onChange={handleChange('bloodPressure')}
          disabled={disabled}
          helperText="Format: Systolic/Diastolic"
        />
        <TextField
          fullWidth
          label="Pulse Rate (bpm)"
          type="number"
          value={vitals.pulseRate ?? ''}
          onChange={handleChange('pulseRate')}
          disabled={disabled}
          slotProps={{
            htmlInput: { step: '1', min: '30', max: '200' },
          }}
          helperText="Normal: 60-100 bpm"
        />
        <TextField
          fullWidth
          label="Oxygen Saturation (%)"
          type="number"
          value={vitals.oxygenSaturation ?? ''}
          onChange={handleChange('oxygenSaturation')}
          disabled={disabled}
          slotProps={{
            htmlInput: { step: '0.1', min: '70', max: '100' },
          }}
          helperText="Normal: >95%"
        />
        <TextField
          fullWidth
          label="Weight (kg)"
          type="number"
          value={vitals.weight ?? ''}
          onChange={handleChange('weight')}
          disabled={disabled}
          slotProps={{
            htmlInput: { step: '0.1', min: '0', max: '300' },
          }}
        />
        <TextField
          fullWidth
          label="Height (cm)"
          type="number"
          value={vitals.height ?? ''}
          onChange={handleChange('height')}
          disabled={disabled}
          slotProps={{
            htmlInput: { step: '0.1', min: '0', max: '250' },
          }}
        />
        <TextField
          fullWidth
          label="Respiratory Rate"
          placeholder="16/min"
          value={vitals.respiratoryRate ?? ''}
          onChange={handleChange('respiratoryRate')}
          disabled={disabled}
          helperText="Normal: 12-20/min"
        />
        <TextField
          fullWidth
          label="BMI"
          type="number"
          value={vitals.bmi ?? ''}
          onChange={handleChange('bmi')}
          disabled={disabled}
          slotProps={{
            htmlInput: { step: '0.1', min: '10', max: '60' },
          }}
          helperText="Body Mass Index"
        />
      </Box>
    </Box>
  );
};

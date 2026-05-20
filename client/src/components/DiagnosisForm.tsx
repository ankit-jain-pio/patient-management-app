import React from 'react';
import { Box, TextField, Typography } from '@mui/material';

interface DiagnosisFormProps {
  diagnosis: string;
  clinicalNotes: string;
  treatmentPlan: string;
  followUpInstructions: string;
  nextVisitDate?: string;
  onChange: (
    field: 'diagnosis' | 'clinicalNotes' | 'treatmentPlan' | 'followUpInstructions' | 'nextVisitDate',
    value: string
  ) => void;
  disabled?: boolean;
}

export const DiagnosisForm: React.FC<DiagnosisFormProps> = ({
  diagnosis,
  clinicalNotes,
  treatmentPlan,
  followUpInstructions,
  nextVisitDate,
  onChange,
  disabled = false,
}) => {
  const handleChange = (
    field: 'diagnosis' | 'clinicalNotes' | 'treatmentPlan' | 'followUpInstructions' | 'nextVisitDate'
  ) => (event: React.ChangeEvent<HTMLInputElement>) => {
    onChange(field, event.target.value);
  };

  return (
    <Box>
      <Typography variant="h6" gutterBottom>
        Diagnosis & Treatment
      </Typography>
      <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
        <TextField
          fullWidth
          required
          label="Diagnosis"
          placeholder="Primary diagnosis..."
          value={diagnosis}
          onChange={handleChange('diagnosis')}
          disabled={disabled}
          multiline
          rows={2}
          helperText="Primary diagnosis or provisional diagnosis"
        />
        <TextField
          fullWidth
          label="Clinical Notes"
          placeholder="Examination findings, observations..."
          value={clinicalNotes}
          onChange={handleChange('clinicalNotes')}
          disabled={disabled}
          multiline
          rows={3}
          helperText="Detailed examination findings and clinical observations"
        />
        <TextField
          fullWidth
          required
          label="Treatment Plan"
          placeholder="Medications, procedures, lifestyle changes..."
          value={treatmentPlan}
          onChange={handleChange('treatmentPlan')}
          disabled={disabled}
          multiline
          rows={3}
          helperText="Recommended treatment approach"
        />
        <TextField
          fullWidth
          label="Follow-up Instructions"
          placeholder="When to return, warning signs to watch for..."
          value={followUpInstructions}
          onChange={handleChange('followUpInstructions')}
          disabled={disabled}
          multiline
          rows={2}
          helperText="Instructions for patient follow-up care"
        />
        <TextField
          fullWidth
          label="Next Visit Date"
          type="date"
          value={nextVisitDate ?? ''}
          onChange={handleChange('nextVisitDate')}
          disabled={disabled}
          slotProps={{
            inputLabel: { shrink: true },
          }}
          helperText="Schedule next follow-up visit if needed"
        />
      </Box>
    </Box>
  );
};

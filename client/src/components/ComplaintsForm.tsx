import React from 'react';
import { Box, TextField, Typography } from '@mui/material';

interface ComplaintsFormProps {
  chiefComplaint: string;
  symptoms: string;
  onChange: (field: 'chiefComplaint' | 'symptoms', value: string) => void;
  disabled?: boolean;
}

export const ComplaintsForm: React.FC<ComplaintsFormProps> = ({
  chiefComplaint,
  symptoms,
  onChange,
  disabled = false,
}) => {
  const handleChange = (field: 'chiefComplaint' | 'symptoms') => (
    event: React.ChangeEvent<HTMLInputElement>
  ) => {
    onChange(field, event.target.value);
  };

  return (
    <Box>
      <Typography variant="h6" gutterBottom>
        Complaints & Symptoms
      </Typography>
      <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
        <TextField
          fullWidth
          required
          label="Chief Complaint"
          placeholder="Main reason for visit..."
          value={chiefComplaint}
          onChange={handleChange('chiefComplaint')}
          disabled={disabled}
          multiline
          rows={2}
          helperText="Primary reason patient came for consultation"
        />
        <TextField
          fullWidth
          label="Symptoms"
          placeholder="Describe all symptoms in detail..."
          value={symptoms}
          onChange={handleChange('symptoms')}
          disabled={disabled}
          multiline
          rows={4}
          helperText="Duration, severity, associated symptoms, etc."
        />
      </Box>
    </Box>
  );
};

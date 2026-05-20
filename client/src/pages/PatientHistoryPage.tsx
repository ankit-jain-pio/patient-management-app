import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import {
  Box,
  Paper,
  Typography,
  TextField,
  Button,
  CircularProgress,
  Alert,
  Chip,
  List,
  ListItem,
  ListItemIcon,
  ListItemText,
  Divider,
} from '@mui/material';
import {
  LocalHospital as ConsultationIcon,
  Event as AppointmentIcon,
  Refresh as RefreshIcon,
  ChevronRight as ChevronRightIcon,
} from '@mui/icons-material';
import { exportService } from '../services/export.service';
import { ConsultationDetailModal } from '../components/ConsultationDetailModal';
import { ExportButtons } from '../components/ExportButtons';
import type { PatientHistory, DateRangeFilter } from '../types/history.types';
import type { Consultation } from '../types/consultation.types';

export const PatientHistoryPage: React.FC = () => {
  const { patientId } = useParams<{ patientId: string }>();
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [history, setHistory] = useState<PatientHistory | null>(null);
  const [selectedConsultation, setSelectedConsultation] = useState<Consultation | null>(null);
  const [dateRange, setDateRange] = useState<DateRangeFilter>({
    startDate: '',
    endDate: '',
  });

  useEffect(() => {
    if (patientId) {
      loadHistory();
    }
  }, [patientId]);

  const loadHistory = async () => {
    if (!patientId) return;

    try {
      setLoading(true);
      setError('');
      const data = await exportService.getPatientHistory(patientId, dateRange);
      setHistory(data);
    } catch (err) {
      console.error('Load history error:', err);
      setError('Failed to load patient history');
    } finally {
      setLoading(false);
    }
  };

  const handleApplyFilter = () => {
    loadHistory();
  };

  const handleClearFilter = () => {
    setDateRange({ startDate: '', endDate: '' });
    setTimeout(() => loadHistory(), 0);
  };

  const handleConsultationClick = (consultation: Consultation) => {
    setSelectedConsultation(consultation);
  };

  const formatDate = (dateString: string): string => {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
    });
  };

  const formatTime = (dateString: string): string => {
    const date = new Date(dateString);
    return date.toLocaleTimeString('en-US', {
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  if (loading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '400px' }}>
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return (
      <Box sx={{ p: 3 }}>
        <Alert severity="error" onClose={() => setError('')}>
          {error}
        </Alert>
      </Box>
    );
  }

  if (!history) {
    return (
      <Box sx={{ p: 3 }}>
        <Alert severity="info">No history data available</Alert>
      </Box>
    );
  }

  // Combine consultations and appointments for timeline
  const timelineItems = [
    ...history.consultations.map((c) => ({
      type: 'consultation' as const,
      date: c.consultationDate,
      data: c,
    })),
    ...history.appointments.map((a) => ({
      type: 'appointment' as const,
      date: a.scheduledDateTime,
      data: a,
    })),
  ].sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime());

  return (
    <Box sx={{ p: 3 }}>
      {/* Header */}
      <Box sx={{ mb: 3 }}>
        <Typography variant="h4" gutterBottom>
          Patient History
        </Typography>
        <Typography variant="body1" color="text.secondary">
          {history.patientName} | Age: {history.age}
        </Typography>
        <Box sx={{ mt: 2, display: 'flex', gap: 2, alignItems: 'center' }}>
          <Chip label={`${history.totalConsultations} Consultations`} color="primary" />
          <Chip label={`${history.totalAppointments} Appointments`} />
        </Box>
      </Box>

      {/* Date Range Filter */}
      <Paper sx={{ p: 2, mb: 3 }}>
        <Typography variant="h6" gutterBottom>
          Filter by Date Range
        </Typography>
        <Box sx={{ display: 'flex', gap: 2, alignItems: 'center', flexWrap: 'wrap' }}>
          <TextField
            label="Start Date"
            type="date"
            value={dateRange.startDate || ''}
            onChange={(e) => setDateRange({ ...dateRange, startDate: e.target.value })}
            slotProps={{
              inputLabel: { shrink: true },
            }}
            sx={{ minWidth: '200px' }}
          />
          <TextField
            label="End Date"
            type="date"
            value={dateRange.endDate || ''}
            onChange={(e) => setDateRange({ ...dateRange, endDate: e.target.value })}
            slotProps={{
              inputLabel: { shrink: true },
            }}
            sx={{ minWidth: '200px' }}
          />
          <Button variant="contained" onClick={handleApplyFilter}>
            Apply Filter
          </Button>
          <Button variant="outlined" onClick={handleClearFilter}>
            Clear
          </Button>
          <Button variant="outlined" startIcon={<RefreshIcon />} onClick={loadHistory}>
            Refresh
          </Button>
        </Box>
      </Paper>

      {/* Export Buttons */}
      {patientId && <ExportButtons patientId={patientId} dateRange={dateRange} />}

      {/* History List */}
      <Paper sx={{ p: 3 }}>
        <Typography variant="h6" gutterBottom>
          Visit History
        </Typography>

        {timelineItems.length === 0 ? (
          <Typography variant="body2" color="text.secondary" sx={{ textAlign: 'center', py: 4 }}>
            No visit history available for the selected date range
          </Typography>
        ) : (
          <List>
            {timelineItems.map((item, index) => (
              <React.Fragment key={`${item.type}-${index}`}>
                <ListItem
                  alignItems="flex-start"
                  sx={{
                    cursor: item.type === 'consultation' ? 'pointer' : 'default',
                    '&:hover': item.type === 'consultation' ? { bgcolor: 'action.hover' } : {},
                    borderRadius: 1,
                  }}
                  onClick={
                    item.type === 'consultation'
                      ? () => handleConsultationClick(item.data as Consultation)
                      : undefined
                  }
                >
                  <ListItemIcon sx={{ mt: 1 }}>
                    {item.type === 'consultation' ? (
                      <ConsultationIcon color="primary" />
                    ) : (
                      <AppointmentIcon color="secondary" />
                    )}
                  </ListItemIcon>
                  <ListItemText
                    primary={
                      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                        <Typography variant="h6">
                          {item.type === 'consultation' ? 'Consultation' : 'Appointment'}
                        </Typography>
                        <Typography variant="body2" color="text.secondary">
                          {formatDate(item.date)} at {formatTime(item.date)}
                        </Typography>
                      </Box>
                    }
                    secondary={
                      <Box sx={{ mt: 1 }}>
                        {item.type === 'consultation' ? (
                          <>
                            <Typography variant="body2" color="text.secondary">
                              <strong>Chief Complaint:</strong> {item.data.chiefComplaint || 'N/A'}
                            </Typography>
                            <Typography variant="body2" color="text.secondary">
                              <strong>Diagnosis:</strong> {item.data.diagnosis || 'Not documented'}
                            </Typography>
                            {item.data.prescriptions && item.data.prescriptions.length > 0 && (
                              <Chip
                                label={`${item.data.prescriptions.length} prescription(s)`}
                                size="small"
                                color="primary"
                                sx={{ mt: 0.5 }}
                              />
                            )}
                          </>
                        ) : (
                          <>
                            <Typography variant="body2" color="text.secondary">
                              <strong>Reason:</strong> {item.data.reason || 'N/A'}
                            </Typography>
                            <Typography variant="body2" color="text.secondary">
                              <strong>Status:</strong> {item.data.status}
                            </Typography>
                          </>
                        )}
                      </Box>
                    }
                  />
                  {item.type === 'consultation' && (
                    <ListItemIcon sx={{ mt: 1 }}>
                      <ChevronRightIcon />
                    </ListItemIcon>
                  )}
                </ListItem>
                {index < timelineItems.length - 1 && <Divider />}
              </React.Fragment>
            ))}
          </List>
        )}
      </Paper>

      {/* Consultation Detail Modal */}
      {selectedConsultation && (
        <ConsultationDetailModal
          open={!!selectedConsultation}
          consultation={selectedConsultation}
          onClose={() => setSelectedConsultation(null)}
        />
      )}
    </Box>
  );
};

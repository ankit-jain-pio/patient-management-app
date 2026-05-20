import React, { useState, useEffect } from 'react';
import {
  Paper,
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
  Menu,
  MenuItem,
  ListItemIcon,
  ListItemText,
} from '@mui/material';
import {
  MoreVert as MoreVertIcon,
  Cancel as CancelIcon,
  PersonAdd as PersonAddIcon,
  PlayArrow as PlayArrowIcon,
  Done as DoneIcon,
  EventBusy as EventBusyIcon,
} from '@mui/icons-material';
import { appointmentService } from '../services/appointment.service';
import type { Appointment } from '../types/appointment.types';
import { AppointmentStatus } from '../types/appointment.types';

interface AppointmentListProps {
  selectedDate: string;
  onStatusChange?: (appointment: Appointment) => void;
  onAppointmentClick?: (appointment: Appointment) => void;
  refreshTrigger?: number;
}

export const AppointmentList: React.FC<AppointmentListProps> = ({
  selectedDate,
  onStatusChange,
  onAppointmentClick,
  refreshTrigger,
}) => {
  const [appointments, setAppointments] = useState<Appointment[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const [selectedAppointment, setSelectedAppointment] = useState<Appointment | null>(null);
  const [updatingStatus, setUpdatingStatus] = useState(false);

  useEffect(() => {
    loadAppointments();
  }, [selectedDate, refreshTrigger]);

  const loadAppointments = async (): Promise<void> => {
    setIsLoading(true);
    setError('');

    try {
      const data = await appointmentService.getAppointmentsByDate(selectedDate);
      // Sort by scheduled time
      const sorted = data.sort((a, b) => 
        new Date(a.scheduledDateTime).getTime() - new Date(b.scheduledDateTime).getTime()
      );
      setAppointments(sorted);
    } catch (err) {
      setError('Failed to load appointments.');
      console.error('Load appointments error:', err);
    } finally {
      setIsLoading(false);
    }
  };

  const handleMenuOpen = (event: React.MouseEvent<HTMLElement>, appointment: Appointment): void => {
    setAnchorEl(event.currentTarget);
    setSelectedAppointment(appointment);
  };

  const handleMenuClose = (): void => {
    setAnchorEl(null);
  };

  const handleStatusChange = async (newStatus: AppointmentStatus): Promise<void> => {
    if (!selectedAppointment) return;

    setUpdatingStatus(true);
    try {
      const updated = await appointmentService.updateAppointmentStatus(selectedAppointment.id, {
        status: newStatus,
        checkInTime: newStatus === AppointmentStatus.CheckedIn ? new Date().toISOString() : undefined,
        completedTime: newStatus === AppointmentStatus.Completed ? new Date().toISOString() : undefined,
      });

      setAppointments((prev) =>
        prev.map((apt) => (apt.id === updated.id ? updated : apt))
      );

      if (onStatusChange) {
        onStatusChange(updated);
      }
    } catch (err) {
      console.error('Update status error:', err);
      setError('Failed to update appointment status.');
    } finally {
      setUpdatingStatus(false);
      handleMenuClose();
    }
  };

  const getStatusColor = (status: AppointmentStatus): 'default' | 'primary' | 'success' | 'error' | 'warning' => {
    switch (status) {
      case AppointmentStatus.Scheduled:
        return 'primary';
      case AppointmentStatus.CheckedIn:
        return 'warning';
      case AppointmentStatus.InProgress:
        return 'warning';
      case AppointmentStatus.Completed:
        return 'success';
      case AppointmentStatus.Cancelled:
        return 'error';
      case AppointmentStatus.NoShow:
        return 'error';
      default:
        return 'default';
    }
  };

  const getStatusLabel = (status: AppointmentStatus): string => {
    switch (status) {
      case AppointmentStatus.Scheduled:
        return 'Scheduled';
      case AppointmentStatus.CheckedIn:
        return 'Checked In';
      case AppointmentStatus.InProgress:
        return 'In Progress';
      case AppointmentStatus.Completed:
        return 'Completed';
      case AppointmentStatus.Cancelled:
        return 'Cancelled';
      case AppointmentStatus.NoShow:
        return 'No Show';
      default:
        return 'Unknown';
    }
  };

  const formatTime = (dateTimeString: string): string => {
    const date = new Date(dateTimeString);
    return date.toLocaleTimeString('en-US', {
      hour: '2-digit',
      minute: '2-digit',
      hour12: true,
    });
  };

  const canChangeStatus = (appointment: Appointment): boolean => {
    return appointment.status !== AppointmentStatus.Completed && 
           appointment.status !== AppointmentStatus.Cancelled &&
           appointment.status !== AppointmentStatus.NoShow;
  };

  if (isLoading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}>
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return (
      <Paper sx={{ p: 3 }}>
        <Typography color="error">{error}</Typography>
      </Paper>
    );
  }

  if (appointments.length === 0) {
    return (
      <Paper sx={{ p: 4, textAlign: 'center' }}>
        <Typography color="text.secondary">
          No appointments scheduled for {new Date(selectedDate).toLocaleDateString()}
        </Typography>
      </Paper>
    );
  }

  return (
    <>
      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Time</TableCell>
              <TableCell>Patient</TableCell>
              <TableCell>Phone</TableCell>
              <TableCell>Reason</TableCell>
              <TableCell>Status</TableCell>
              <TableCell align="right">Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {appointments.map((appointment) => (
              <TableRow
                key={appointment.id}
                hover
                onClick={() => onAppointmentClick && onAppointmentClick(appointment)}
                sx={{ cursor: onAppointmentClick ? 'pointer' : 'default' }}
              >
                <TableCell>
                  <Typography variant="body2" sx={{ fontWeight: 'medium' }}>
                    {formatTime(appointment.scheduledDateTime)}
                  </Typography>
                </TableCell>
                <TableCell>
                  <Typography variant="body2">
                    {appointment.patient?.fullName || 'N/A'}
                  </Typography>
                  <Typography variant="caption" color="text.secondary">
                    Age: {appointment.patient?.age || 'N/A'}
                  </Typography>
                </TableCell>
                <TableCell>{appointment.patient?.phoneNumber || 'N/A'}</TableCell>
                <TableCell>
                  <Typography variant="body2" noWrap sx={{ maxWidth: 200 }}>
                    {appointment.reason || '-'}
                  </Typography>
                </TableCell>
                <TableCell>
                  <Chip
                    label={getStatusLabel(appointment.status)}
                    color={getStatusColor(appointment.status)}
                    size="small"
                  />
                </TableCell>
                <TableCell align="right">
                  {canChangeStatus(appointment) && (
                    <IconButton
                      size="small"
                      onClick={(e) => {
                        e.stopPropagation();
                        handleMenuOpen(e, appointment);
                      }}
                      disabled={updatingStatus}
                    >
                      <MoreVertIcon />
                    </IconButton>
                  )}
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>

      <Menu
        anchorEl={anchorEl}
        open={Boolean(anchorEl)}
        onClose={handleMenuClose}
      >
        {selectedAppointment?.status === AppointmentStatus.Scheduled && (
          <MenuItem onClick={() => handleStatusChange(AppointmentStatus.CheckedIn)}>
            <ListItemIcon>
              <PersonAddIcon fontSize="small" />
            </ListItemIcon>
            <ListItemText>Check In</ListItemText>
          </MenuItem>
        )}
        {selectedAppointment?.status === AppointmentStatus.CheckedIn && (
          <MenuItem onClick={() => handleStatusChange(AppointmentStatus.InProgress)}>
            <ListItemIcon>
              <PlayArrowIcon fontSize="small" />
            </ListItemIcon>
            <ListItemText>Start Consultation</ListItemText>
          </MenuItem>
        )}
        {(selectedAppointment?.status === AppointmentStatus.CheckedIn ||
          selectedAppointment?.status === AppointmentStatus.InProgress) && (
          <MenuItem onClick={() => handleStatusChange(AppointmentStatus.Completed)}>
            <ListItemIcon>
              <DoneIcon fontSize="small" />
            </ListItemIcon>
            <ListItemText>Mark Completed</ListItemText>
          </MenuItem>
        )}
        <MenuItem onClick={() => handleStatusChange(AppointmentStatus.Cancelled)}>
          <ListItemIcon>
            <CancelIcon fontSize="small" color="error" />
          </ListItemIcon>
          <ListItemText>Cancel</ListItemText>
        </MenuItem>
        <MenuItem onClick={() => handleStatusChange(AppointmentStatus.NoShow)}>
          <ListItemIcon>
            <EventBusyIcon fontSize="small" color="error" />
          </ListItemIcon>
          <ListItemText>Mark No Show</ListItemText>
        </MenuItem>
      </Menu>
    </>
  );
};

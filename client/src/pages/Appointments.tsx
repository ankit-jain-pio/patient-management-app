import React, { useState } from 'react';
import { Box, Typography, Button, TextField, Tabs, Tab, Paper } from '@mui/material';
import { Add as AddIcon } from '@mui/icons-material';
import { AppointmentForm } from '../components/AppointmentForm';
import { AppointmentList } from '../components/AppointmentList';
import type { Appointment } from '../types/appointment.types';
import { AppointmentStatus } from '../types/appointment.types';

export const Appointments: React.FC = () => {
  const [selectedDate, setSelectedDate] = useState<string>(
    new Date().toISOString().split('T')[0]
  );
  const [openForm, setOpenForm] = useState(false);
  const [refreshTrigger, setRefreshTrigger] = useState(0);
  const [statusFilter, setStatusFilter] = useState<'all' | AppointmentStatus>('all');

  const handleOpenForm = (): void => {
    setOpenForm(true);
  };

  const handleCloseForm = (): void => {
    setOpenForm(false);
  };

  const handleFormSuccess = (appointment: Appointment): void => {
    setRefreshTrigger((prev) => prev + 1);
    // If the appointment is for a different date, switch to that date
    const appointmentDate = new Date(appointment.scheduledDateTime).toISOString().split('T')[0];
    if (appointmentDate !== selectedDate) {
      setSelectedDate(appointmentDate);
    }
  };

  const handleStatusChange = (): void => {
    setRefreshTrigger((prev) => prev + 1);
  };

  const handleDateChange = (event: React.ChangeEvent<HTMLInputElement>): void => {
    setSelectedDate(event.target.value);
  };

  const handleTabChange = (_: React.SyntheticEvent, newValue: 'all' | AppointmentStatus): void => {
    setStatusFilter(newValue);
  };

  const setToday = (): void => {
    setSelectedDate(new Date().toISOString().split('T')[0]);
  };

  const adjustDate = (days: number): void => {
    const currentDate = new Date(selectedDate);
    currentDate.setDate(currentDate.getDate() + days);
    setSelectedDate(currentDate.toISOString().split('T')[0]);
  };

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography variant="h4">Appointments</Typography>
        <Button
          variant="contained"
          startIcon={<AddIcon />}
          onClick={handleOpenForm}
        >
          New Appointment
        </Button>
      </Box>

      {/* Date Navigation */}
      <Paper sx={{ p: 2, mb: 3 }}>
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, flexWrap: 'wrap' }}>
          <Button size="small" onClick={() => adjustDate(-1)}>
            Previous Day
          </Button>
          <TextField
            type="date"
            size="small"
            value={selectedDate}
            onChange={handleDateChange}
            slotProps={{
              inputLabel: { shrink: true },
            }}
            sx={{ minWidth: 180 }}
          />
          <Button size="small" variant="outlined" onClick={setToday}>
            Today
          </Button>
          <Button size="small" onClick={() => adjustDate(1)}>
            Next Day
          </Button>
        </Box>
      </Paper>

      {/* Status Filter Tabs */}
      <Paper sx={{ mb: 2 }}>
        <Tabs
          value={statusFilter}
          onChange={handleTabChange}
          variant="scrollable"
          scrollButtons="auto"
        >
          <Tab label="All" value="all" />
          <Tab label="Scheduled" value={AppointmentStatus.Scheduled} />
          <Tab label="Checked In" value={AppointmentStatus.CheckedIn} />
          <Tab label="In Progress" value={AppointmentStatus.InProgress} />
          <Tab label="Completed" value={AppointmentStatus.Completed} />
          <Tab label="Cancelled" value={AppointmentStatus.Cancelled} />
        </Tabs>
      </Paper>

      {/* Appointment List */}
      <AppointmentList
        selectedDate={selectedDate}
        onStatusChange={handleStatusChange}
        refreshTrigger={refreshTrigger}
      />

      {/* Appointment Form Dialog */}
      <AppointmentForm
        open={openForm}
        onClose={handleCloseForm}
        onSuccess={handleFormSuccess}
      />
    </Box>
  );
};

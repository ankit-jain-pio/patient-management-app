import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Box,
  Paper,
  Stepper,
  Step,
  StepLabel,
  Button,
  Typography,
  CircularProgress,
  Alert,
} from '@mui/material';
import { VitalsForm } from '../components/VitalsForm';
import { ComplaintsForm } from '../components/ComplaintsForm';
import { DiagnosisForm } from '../components/DiagnosisForm';
import { PrescriptionForm } from '../components/PrescriptionForm';
import { consultationService } from '../services/consultation.service';
import { patientService } from '../services/patient.service';
import type { ConsultationFormData, Vitals, AddPrescriptionRequest } from '../types/consultation.types';
import type { Patient } from '../types/patient.types';

const steps = ['Vitals', 'Complaints & Symptoms', 'Diagnosis & Treatment', 'Prescriptions'];

export const ConsultationForm: React.FC = () => {
  const { patientId, appointmentId } = useParams<{ patientId: string; appointmentId?: string }>();
  const navigate = useNavigate();
  
  const [activeStep, setActiveStep] = useState(0);
  const [loading, setLoading] = useState(false);
  const [loadingPatient, setLoadingPatient] = useState(true);
  const [error, setError] = useState('');
  const [patient, setPatient] = useState<Patient | null>(null);
  const [consultationId, setConsultationId] = useState<string | null>(null);
  
  const [formData, setFormData] = useState<ConsultationFormData>({
    vitals: {},
    chiefComplaint: '',
    symptoms: '',
    diagnosis: '',
    clinicalNotes: '',
    treatmentPlan: '',
    followUpInstructions: '',
    nextVisitDate: undefined,
    prescriptions: [],
  });

  useEffect(() => {
    loadPatientData();
  }, [patientId]);

  const loadPatientData = async () => {
    if (!patientId) {
      setError('Patient ID is required');
      setLoadingPatient(false);
      return;
    }

    try {
      setLoadingPatient(true);
      const patientData = await patientService.getPatientById(patientId);
      setPatient(patientData);
      setError('');
    } catch (err) {
      console.error('Load patient error:', err);
      setError('Failed to load patient data');
    } finally {
      setLoadingPatient(false);
    }
  };

  const handleVitalsChange = (vitals: Vitals) => {
    setFormData({ ...formData, vitals });
  };

  const handleComplaintsChange = (field: 'chiefComplaint' | 'symptoms', value: string) => {
    setFormData({ ...formData, [field]: value });
  };

  const handleDiagnosisChange = (
    field: 'diagnosis' | 'clinicalNotes' | 'treatmentPlan' | 'followUpInstructions' | 'nextVisitDate',
    value: string
  ) => {
    setFormData({ ...formData, [field]: value });
  };

  const handlePrescriptionsChange = (prescriptions: AddPrescriptionRequest[]) => {
    setFormData({ ...formData, prescriptions });
  };

  const isStepValid = (step: number): boolean => {
    switch (step) {
      case 0: // Vitals - optional, always valid
        return true;
      case 1: // Complaints - chief complaint required
        return formData.chiefComplaint.trim().length > 0;
      case 2: // Diagnosis - diagnosis and treatment plan required
        return formData.diagnosis.trim().length > 0 && formData.treatmentPlan.trim().length > 0;
      case 3: // Prescriptions - optional, always valid
        return true;
      default:
        return false;
    }
  };

  const handleNext = async () => {
    if (!isStepValid(activeStep)) {
      setError('Please fill in all required fields');
      return;
    }

    setError('');

    // Save consultation data at key steps
    if (activeStep === 1) {
      // After complaints step, create consultation
      await createConsultation();
    } else if (activeStep === 2) {
      // After diagnosis step, update consultation
      await updateConsultation();
    }

    if (activeStep < steps.length - 1) {
      setActiveStep((prev) => prev + 1);
    }
  };

  const handleBack = () => {
    setActiveStep((prev) => prev - 1);
  };

  const createConsultation = async () => {
    if (!patientId) return;

    try {
      setLoading(true);
      const consultation = await consultationService.createConsultation({
        patientId,
        appointmentId: appointmentId || undefined,
        vitals: formData.vitals,
        chiefComplaint: formData.chiefComplaint,
        symptoms: formData.symptoms,
      });
      setConsultationId(consultation.id);
      setError('');
    } catch (err) {
      console.error('Create consultation error:', err);
      setError('Failed to save consultation data');
      throw err;
    } finally {
      setLoading(false);
    }
  };

  const updateConsultation = async () => {
    if (!consultationId) return;

    try {
      setLoading(true);
      await consultationService.updateConsultation(consultationId, {
        vitals: formData.vitals,
        chiefComplaint: formData.chiefComplaint,
        symptoms: formData.symptoms,
        diagnosis: formData.diagnosis,
        clinicalNotes: formData.clinicalNotes,
        treatmentPlan: formData.treatmentPlan,
        followUpInstructions: formData.followUpInstructions,
        nextVisitDate: formData.nextVisitDate,
      });
      setError('');
    } catch (err) {
      console.error('Update consultation error:', err);
      setError('Failed to update consultation');
      throw err;
    } finally {
      setLoading(false);
    }
  };

  const handleFinish = async () => {
    if (!consultationId) {
      setError('Consultation not created');
      return;
    }

    try {
      setLoading(true);
      
      // Add all prescriptions
      for (const prescription of formData.prescriptions) {
        await consultationService.addPrescription(consultationId, prescription);
      }

      // Navigate back to patient detail or appointments
      navigate(patientId ? `/patients/${patientId}` : '/appointments', {
        state: { message: 'Consultation completed successfully' },
      });
    } catch (err) {
      console.error('Add prescriptions error:', err);
      setError('Failed to add prescriptions');
    } finally {
      setLoading(false);
    }
  };

  const renderStepContent = (step: number) => {
    switch (step) {
      case 0:
        return <VitalsForm vitals={formData.vitals} onChange={handleVitalsChange} disabled={loading} />;
      case 1:
        return (
          <ComplaintsForm
            chiefComplaint={formData.chiefComplaint}
            symptoms={formData.symptoms}
            onChange={handleComplaintsChange}
            disabled={loading}
          />
        );
      case 2:
        return (
          <DiagnosisForm
            diagnosis={formData.diagnosis}
            clinicalNotes={formData.clinicalNotes}
            treatmentPlan={formData.treatmentPlan}
            followUpInstructions={formData.followUpInstructions}
            nextVisitDate={formData.nextVisitDate}
            onChange={handleDiagnosisChange}
            disabled={loading}
          />
        );
      case 3:
        return (
          <PrescriptionForm
            prescriptions={formData.prescriptions}
            onChange={handlePrescriptionsChange}
            disabled={loading}
          />
        );
      default:
        return null;
    }
  };

  if (loadingPatient) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '400px' }}>
        <CircularProgress />
      </Box>
    );
  }

  if (!patient) {
    return (
      <Box sx={{ p: 3 }}>
        <Alert severity="error">Patient not found</Alert>
      </Box>
    );
  }

  return (
    <Box sx={{ p: 3 }}>
      <Paper sx={{ p: 3 }}>
        {/* Patient Info Header */}
        <Box sx={{ mb: 3 }}>
          <Typography variant="h5" gutterBottom>
            New Consultation
          </Typography>
          <Typography variant="body1" color="text.secondary">
            Patient: {patient.fullName} | Age: {patient.age} | Phone: {patient.phoneNumber}
          </Typography>
        </Box>

        {/* Stepper */}
        <Stepper activeStep={activeStep} sx={{ mb: 4 }}>
          {steps.map((label) => (
            <Step key={label}>
              <StepLabel>{label}</StepLabel>
            </Step>
          ))}
        </Stepper>

        {/* Error Alert */}
        {error && (
          <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError('')}>
            {error}
          </Alert>
        )}

        {/* Step Content */}
        <Box sx={{ mb: 3, minHeight: '400px' }}>
          {renderStepContent(activeStep)}
        </Box>

        {/* Navigation Buttons */}
        <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
          <Button
            disabled={activeStep === 0 || loading}
            onClick={handleBack}
          >
            Back
          </Button>
          <Box sx={{ display: 'flex', gap: 1 }}>
            <Button
              variant="outlined"
              onClick={() => navigate(patientId ? `/patients/${patientId}` : '/appointments')}
              disabled={loading}
            >
              Cancel
            </Button>
            {activeStep === steps.length - 1 ? (
              <Button
                variant="contained"
                onClick={handleFinish}
                disabled={loading || !consultationId}
              >
                {loading ? <CircularProgress size={24} /> : 'Finish'}
              </Button>
            ) : (
              <Button
                variant="contained"
                onClick={handleNext}
                disabled={loading || !isStepValid(activeStep)}
              >
                {loading ? <CircularProgress size={24} /> : 'Next'}
              </Button>
            )}
          </Box>
        </Box>
      </Paper>
    </Box>
  );
};

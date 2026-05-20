using MediatR;
using Microsoft.EntityFrameworkCore;
using PatientManagement.Application.Common.Interfaces;
using PatientManagement.Application.DTOs.Appointments;
using PatientManagement.Application.DTOs.Consultations;
using PatientManagement.Domain.Entities;
using PatientManagement.Domain.ValueObjects;

namespace PatientManagement.Application.Consultations.Commands.CreateConsultation;

public class CreateConsultationCommandHandler : IRequestHandler<CreateConsultationCommand, ConsultationDto>
{
    private readonly IApplicationDbContext _context;

    public CreateConsultationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ConsultationDto> Handle(CreateConsultationCommand request, CancellationToken cancellationToken)
    {
        // Verify patient exists
        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.Id == request.PatientId, cancellationToken);

        if (patient == null)
        {
            throw new KeyNotFoundException($"Patient with ID {request.PatientId} not found");
        }

        // Verify appointment exists if provided
        if (request.AppointmentId.HasValue)
        {
            var appointmentExists = await _context.Appointments
                .AnyAsync(a => a.Id == request.AppointmentId.Value, cancellationToken);

            if (!appointmentExists)
            {
                throw new KeyNotFoundException($"Appointment with ID {request.AppointmentId} not found");
            }
        }

        // Create Vitals value object if provided
        Vitals? vitals = null;
        if (request.Vitals != null)
        {
            vitals = new Vitals(
                request.Vitals.Temperature,
                request.Vitals.BloodPressure,
                request.Vitals.PulseRate,
                request.Vitals.Weight,
                request.Vitals.Height,
                request.Vitals.OxygenSaturation,
                request.Vitals.RespiratoryRate
            );
        }

        // Create consultation
        var consultation = new Consultation
        {
            PatientId = request.PatientId,
            AppointmentId = request.AppointmentId,
            ConsultationDate = DateTime.UtcNow,
            Vitals = vitals,
            ChiefComplaint = request.ChiefComplaint,
            Symptoms = request.Symptoms
        };

        _context.Consultations.Add(consultation);
        await _context.SaveChangesAsync(cancellationToken);

        return new ConsultationDto
        {
            Id = consultation.Id,
            PatientId = consultation.PatientId,
            Patient = new PatientSummaryDto
            {
                Id = patient.Id,
                FullName = patient.FullName,
                Age = patient.Age,
                PhoneNumber = patient.PhoneNumber
            },
            AppointmentId = consultation.AppointmentId,
            ConsultationDate = consultation.ConsultationDate,
            Vitals = consultation.Vitals != null ? new VitalsDto
            {
                Temperature = consultation.Vitals.Temperature,
                BloodPressure = consultation.Vitals.BloodPressure,
                PulseRate = consultation.Vitals.PulseRate,
                Weight = consultation.Vitals.Weight,
                Height = consultation.Vitals.Height,
                OxygenSaturation = consultation.Vitals.OxygenSaturation,
                RespiratoryRate = consultation.Vitals.RespiratoryRate,
                BMI = consultation.Vitals.BMI
            } : null,
            ChiefComplaint = consultation.ChiefComplaint,
            Symptoms = consultation.Symptoms,
            Diagnosis = consultation.Diagnosis,
            ClinicalNotes = consultation.ClinicalNotes,
            TreatmentPlan = consultation.TreatmentPlan,
            FollowUpInstructions = consultation.FollowUpInstructions,
            NextVisitDate = consultation.NextVisitDate,
            Prescriptions = new List<PrescriptionDto>(),
            CreatedAt = consultation.CreatedAt,
            UpdatedAt = consultation.UpdatedAt
        };
    }
}

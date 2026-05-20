using MediatR;
using Microsoft.EntityFrameworkCore;
using PatientManagement.Application.Common.Interfaces;
using PatientManagement.Application.DTOs.Appointments;
using PatientManagement.Application.DTOs.Consultations;

namespace PatientManagement.Application.Consultations.Queries.GetConsultation;

public class GetConsultationQuery : IRequest<ConsultationDto?>
{
    public Guid Id { get; set; }
}

public class GetConsultationQueryHandler : IRequestHandler<GetConsultationQuery, ConsultationDto?>
{
    private readonly IApplicationDbContext _context;

    public GetConsultationQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ConsultationDto?> Handle(GetConsultationQuery request, CancellationToken cancellationToken)
    {
        var consultation = await _context.Consultations
            .Include(c => c.Patient)
            .Include(c => c.Prescriptions)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (consultation == null)
        {
            return null;
        }

        return new ConsultationDto
        {
            Id = consultation.Id,
            PatientId = consultation.PatientId,
            Patient = new PatientSummaryDto
            {
                Id = consultation.Patient.Id,
                FullName = consultation.Patient.FullName,
                Age = consultation.Patient.Age,
                PhoneNumber = consultation.Patient.PhoneNumber
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
            Prescriptions = consultation.Prescriptions.Select(p => new PrescriptionDto
            {
                Id = p.Id,
                ConsultationId = p.ConsultationId,
                MedicationName = p.MedicationName,
                Dosage = p.Dosage,
                Frequency = p.Frequency,
                DurationInDays = p.DurationInDays,
                Instructions = p.Instructions,
                PrescribedDate = p.PrescribedDate
            }).ToList(),
            CreatedAt = consultation.CreatedAt,
            UpdatedAt = consultation.UpdatedAt
        };
    }
}

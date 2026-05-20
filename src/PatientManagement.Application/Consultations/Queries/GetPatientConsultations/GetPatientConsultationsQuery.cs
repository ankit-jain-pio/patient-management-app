using MediatR;
using Microsoft.EntityFrameworkCore;
using PatientManagement.Application.Common.Interfaces;
using PatientManagement.Application.DTOs.Appointments;
using PatientManagement.Application.DTOs.Consultations;

namespace PatientManagement.Application.Consultations.Queries.GetPatientConsultations;

public class GetPatientConsultationsQuery : IRequest<List<ConsultationDto>>
{
    public Guid PatientId { get; set; }
}

public class GetPatientConsultationsQueryHandler : IRequestHandler<GetPatientConsultationsQuery, List<ConsultationDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPatientConsultationsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ConsultationDto>> Handle(GetPatientConsultationsQuery request, CancellationToken cancellationToken)
    {
        var consultations = await _context.Consultations
            .AsNoTracking()
            .Include(c => c.Patient)
            .Include(c => c.Prescriptions)
            .Where(c => c.PatientId == request.PatientId)
            .OrderByDescending(c => c.ConsultationDate)
            .ToListAsync(cancellationToken);

        return consultations.Select(consultation => new ConsultationDto
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
        }).ToList();
    }
}

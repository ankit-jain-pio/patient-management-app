using MediatR;
using Microsoft.EntityFrameworkCore;
using PatientManagement.Application.Common.Interfaces;
using PatientManagement.Application.DTOs.Appointments;
using PatientManagement.Application.DTOs.Consultations;

namespace PatientManagement.Application.Patients.Queries.GetPatientHistory;

public class GetPatientHistoryQuery : IRequest<PatientHistoryDto>
{
    public Guid PatientId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class PatientHistoryDto
{
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public int Age { get; set; }
    public List<ConsultationDto> Consultations { get; set; } = new();
    public List<AppointmentDto> Appointments { get; set; } = new();
    public int TotalConsultations { get; set; }
    public int TotalAppointments { get; set; }
}

public class GetPatientHistoryQueryHandler : IRequestHandler<GetPatientHistoryQuery, PatientHistoryDto>
{
    private readonly IApplicationDbContext _context;

    public GetPatientHistoryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PatientHistoryDto> Handle(GetPatientHistoryQuery request, CancellationToken cancellationToken)
    {
        var patient = await _context.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.PatientId, cancellationToken);

        if (patient == null)
        {
            throw new KeyNotFoundException($"Patient with ID {request.PatientId} not found");
        }

        // Build consultation query with date filtering
        var consultationsQuery = _context.Consultations
            .AsNoTracking()
            .Include(c => c.Prescriptions)
            .Where(c => c.PatientId == request.PatientId);

        if (request.StartDate.HasValue)
        {
            consultationsQuery = consultationsQuery.Where(c => c.ConsultationDate >= request.StartDate.Value);
        }

        if (request.EndDate.HasValue)
        {
            var endOfDay = request.EndDate.Value.Date.AddDays(1);
            consultationsQuery = consultationsQuery.Where(c => c.ConsultationDate < endOfDay);
        }

        var consultations = await consultationsQuery
            .OrderByDescending(c => c.ConsultationDate)
            .ToListAsync(cancellationToken);

        // Build appointment query with date filtering
        var appointmentsQuery = _context.Appointments
            .AsNoTracking()
            .Where(a => a.PatientId == request.PatientId);

        if (request.StartDate.HasValue)
        {
            appointmentsQuery = appointmentsQuery.Where(a => a.ScheduledDateTime >= request.StartDate.Value);
        }

        if (request.EndDate.HasValue)
        {
            var endOfDay = request.EndDate.Value.Date.AddDays(1);
            appointmentsQuery = appointmentsQuery.Where(a => a.ScheduledDateTime < endOfDay);
        }

        var appointments = await appointmentsQuery
            .OrderByDescending(a => a.ScheduledDateTime)
            .ToListAsync(cancellationToken);

        return new PatientHistoryDto
        {
            PatientId = patient.Id,
            PatientName = patient.FullName,
            Age = patient.Age,
            Consultations = consultations.Select(c => new ConsultationDto
            {
                Id = c.Id,
                PatientId = c.PatientId,
                Patient = new PatientSummaryDto
                {
                    Id = patient.Id,
                    FullName = patient.FullName,
                    Age = patient.Age,
                    PhoneNumber = patient.PhoneNumber
                },
                AppointmentId = c.AppointmentId,
                ConsultationDate = c.ConsultationDate,
                Vitals = c.Vitals != null ? new VitalsDto
                {
                    Temperature = c.Vitals.Temperature,
                    BloodPressure = c.Vitals.BloodPressure,
                    PulseRate = c.Vitals.PulseRate,
                    Weight = c.Vitals.Weight,
                    Height = c.Vitals.Height,
                    OxygenSaturation = c.Vitals.OxygenSaturation,
                    RespiratoryRate = c.Vitals.RespiratoryRate,
                    BMI = c.Vitals.BMI
                } : null,
                ChiefComplaint = c.ChiefComplaint,
                Symptoms = c.Symptoms,
                Diagnosis = c.Diagnosis,
                ClinicalNotes = c.ClinicalNotes,
                TreatmentPlan = c.TreatmentPlan,
                FollowUpInstructions = c.FollowUpInstructions,
                NextVisitDate = c.NextVisitDate,
                Prescriptions = c.Prescriptions.Select(p => new PrescriptionDto
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
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            }).ToList(),
            Appointments = appointments.Select(a => new AppointmentDto
            {
                Id = a.Id,
                PatientId = a.PatientId,
                Patient = new PatientSummaryDto
                {
                    Id = patient.Id,
                    FullName = patient.FullName,
                    Age = patient.Age,
                    PhoneNumber = patient.PhoneNumber
                },
                ScheduledDateTime = a.ScheduledDateTime,
                Status = a.Status,
                Reason = a.Reason,
                Notes = a.Notes,
                CheckInTime = a.CheckInTime,
                CompletedTime = a.CompletedTime,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            }).ToList(),
            TotalConsultations = consultations.Count,
            TotalAppointments = appointments.Count
        };
    }
}

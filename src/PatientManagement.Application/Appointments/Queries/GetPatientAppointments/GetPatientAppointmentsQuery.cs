using MediatR;
using Microsoft.EntityFrameworkCore;
using PatientManagement.Application.Common.Interfaces;
using PatientManagement.Application.DTOs.Appointments;

namespace PatientManagement.Application.Appointments.Queries.GetPatientAppointments;

public class GetPatientAppointmentsQuery : IRequest<List<AppointmentDto>>
{
    public Guid PatientId { get; set; }
}

public class GetPatientAppointmentsQueryHandler : IRequestHandler<GetPatientAppointmentsQuery, List<AppointmentDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPatientAppointmentsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<AppointmentDto>> Handle(GetPatientAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var appointments = await _context.Appointments
            .Include(a => a.Patient)
            .Where(a => a.PatientId == request.PatientId)
            .OrderByDescending(a => a.ScheduledDateTime)
            .ToListAsync(cancellationToken);

        return appointments.Select(appointment => new AppointmentDto
        {
            Id = appointment.Id,
            PatientId = appointment.PatientId,
            Patient = new PatientSummaryDto
            {
                Id = appointment.Patient.Id,
                FullName = appointment.Patient.FullName,
                Age = appointment.Patient.Age,
                PhoneNumber = appointment.Patient.PhoneNumber
            },
            ScheduledDateTime = appointment.ScheduledDateTime,
            Status = appointment.Status,
            Reason = appointment.Reason,
            Notes = appointment.Notes,
            CheckInTime = appointment.CheckInTime,
            CompletedTime = appointment.CompletedTime,
            CreatedAt = appointment.CreatedAt,
            UpdatedAt = appointment.UpdatedAt
        }).ToList();
    }
}

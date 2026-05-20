using MediatR;
using Microsoft.EntityFrameworkCore;
using PatientManagement.Application.Common.Interfaces;
using PatientManagement.Application.DTOs.Appointments;

namespace PatientManagement.Application.Appointments.Queries.GetAppointmentsByDate;

public class GetAppointmentsByDateQuery : IRequest<List<AppointmentDto>>
{
    public DateTime Date { get; set; }
}

public class GetAppointmentsByDateQueryHandler : IRequestHandler<GetAppointmentsByDateQuery, List<AppointmentDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAppointmentsByDateQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<AppointmentDto>> Handle(GetAppointmentsByDateQuery request, CancellationToken cancellationToken)
    {
        var startOfDay = request.Date.Date;
        var endOfDay = startOfDay.AddDays(1);

        var appointments = await _context.Appointments
            .Include(a => a.Patient)
            .Where(a => a.ScheduledDateTime >= startOfDay && a.ScheduledDateTime < endOfDay)
            .OrderBy(a => a.ScheduledDateTime)
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

using MediatR;
using Microsoft.EntityFrameworkCore;
using PatientManagement.Application.Common.Interfaces;
using PatientManagement.Application.DTOs.Appointments;

namespace PatientManagement.Application.Appointments.Queries.GetAppointment;

public class GetAppointmentQuery : IRequest<AppointmentDto?>
{
    public Guid Id { get; set; }
}

public class GetAppointmentQueryHandler : IRequestHandler<GetAppointmentQuery, AppointmentDto?>
{
    private readonly IApplicationDbContext _context;

    public GetAppointmentQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AppointmentDto?> Handle(GetAppointmentQuery request, CancellationToken cancellationToken)
    {
        var appointment = await _context.Appointments
            .Include(a => a.Patient)
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

        if (appointment == null)
        {
            return null;
        }

        return new AppointmentDto
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
        };
    }
}

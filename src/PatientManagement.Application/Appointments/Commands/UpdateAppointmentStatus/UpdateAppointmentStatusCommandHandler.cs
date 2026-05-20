using MediatR;
using Microsoft.EntityFrameworkCore;
using PatientManagement.Application.Common.Interfaces;
using PatientManagement.Application.DTOs.Appointments;
using PatientManagement.Domain.Enums;

namespace PatientManagement.Application.Appointments.Commands.UpdateAppointmentStatus;

public class UpdateAppointmentStatusCommandHandler : IRequestHandler<UpdateAppointmentStatusCommand, AppointmentDto>
{
    private readonly IApplicationDbContext _context;

    public UpdateAppointmentStatusCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AppointmentDto> Handle(UpdateAppointmentStatusCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _context.Appointments
            .Include(a => a.Patient)
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

        if (appointment == null)
        {
            throw new KeyNotFoundException($"Appointment with ID {request.Id} not found");
        }

        // Update status and related timestamps
        appointment.Status = request.Status;

        switch (request.Status)
        {
            case AppointmentStatus.CheckedIn:
            case AppointmentStatus.InProgress:
                if (appointment.CheckInTime == null)
                {
                    appointment.CheckInTime = DateTime.UtcNow;
                }
                break;
            case AppointmentStatus.Completed:
                if (appointment.CheckInTime == null)
                {
                    appointment.CheckInTime = DateTime.UtcNow;
                }
                appointment.CompletedTime = DateTime.UtcNow;
                break;
        }

        await _context.SaveChangesAsync(cancellationToken);

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

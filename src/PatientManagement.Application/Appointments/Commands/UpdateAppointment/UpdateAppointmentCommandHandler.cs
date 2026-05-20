using MediatR;
using Microsoft.EntityFrameworkCore;
using PatientManagement.Application.Common.Interfaces;
using PatientManagement.Application.DTOs.Appointments;

namespace PatientManagement.Application.Appointments.Commands.UpdateAppointment;

public class UpdateAppointmentCommandHandler : IRequestHandler<UpdateAppointmentCommand, AppointmentDto>
{
    private readonly IApplicationDbContext _context;

    public UpdateAppointmentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AppointmentDto> Handle(UpdateAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _context.Appointments
            .Include(a => a.Patient)
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

        if (appointment == null)
        {
            throw new KeyNotFoundException($"Appointment with ID {request.Id} not found");
        }

        // Update appointment properties
        appointment.ScheduledDateTime = request.ScheduledDateTime;
        appointment.Reason = request.Reason;
        appointment.Notes = request.Notes;

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

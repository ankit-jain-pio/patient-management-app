using MediatR;
using Microsoft.EntityFrameworkCore;
using PatientManagement.Application.Common.Interfaces;
using PatientManagement.Application.DTOs.Appointments;
using PatientManagement.Domain.Entities;
using PatientManagement.Domain.Enums;

namespace PatientManagement.Application.Appointments.Commands.CreateAppointment;

public class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommand, AppointmentDto>
{
    private readonly IApplicationDbContext _context;

    public CreateAppointmentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AppointmentDto> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
        // Verify patient exists
        var patientExists = await _context.Patients
            .AnyAsync(p => p.Id == request.PatientId, cancellationToken);

        if (!patientExists)
        {
            throw new KeyNotFoundException($"Patient with ID {request.PatientId} not found");
        }

        // Create appointment
        var appointment = new Appointment
        {
            PatientId = request.PatientId,
            ScheduledDateTime = request.ScheduledDateTime,
            Reason = request.Reason,
            Notes = request.Notes,
            Status = AppointmentStatus.Scheduled
        };

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync(cancellationToken);

        // Load patient for response
        var patient = await _context.Patients
            .FirstAsync(p => p.Id == request.PatientId, cancellationToken);

        return new AppointmentDto
        {
            Id = appointment.Id,
            PatientId = appointment.PatientId,
            Patient = new PatientSummaryDto
            {
                Id = patient.Id,
                FullName = patient.FullName,
                Age = patient.Age,
                PhoneNumber = patient.PhoneNumber
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

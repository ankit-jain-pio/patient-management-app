using MediatR;
using PatientManagement.Application.DTOs.Appointments;

namespace PatientManagement.Application.Appointments.Commands.UpdateAppointment;

public class UpdateAppointmentCommand : IRequest<AppointmentDto>
{
    public Guid Id { get; set; }
    public DateTime ScheduledDateTime { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
}

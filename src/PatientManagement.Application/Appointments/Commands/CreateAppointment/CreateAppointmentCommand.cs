using MediatR;
using PatientManagement.Application.DTOs.Appointments;

namespace PatientManagement.Application.Appointments.Commands.CreateAppointment;

public class CreateAppointmentCommand : IRequest<AppointmentDto>
{
    public Guid PatientId { get; set; }
    public DateTime ScheduledDateTime { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
}

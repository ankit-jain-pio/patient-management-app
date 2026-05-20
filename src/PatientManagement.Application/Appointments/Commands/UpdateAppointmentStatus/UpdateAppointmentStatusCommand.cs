using MediatR;
using PatientManagement.Application.DTOs.Appointments;
using PatientManagement.Domain.Enums;

namespace PatientManagement.Application.Appointments.Commands.UpdateAppointmentStatus;

public class UpdateAppointmentStatusCommand : IRequest<AppointmentDto>
{
    public Guid Id { get; set; }
    public AppointmentStatus Status { get; set; }
}

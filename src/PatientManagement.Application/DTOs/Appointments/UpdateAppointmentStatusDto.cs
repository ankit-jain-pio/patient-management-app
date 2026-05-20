using PatientManagement.Domain.Enums;

namespace PatientManagement.Application.DTOs.Appointments;

public class UpdateAppointmentStatusDto
{
    public AppointmentStatus Status { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CompletedTime { get; set; }
}

namespace PatientManagement.Application.DTOs.Appointments;

public class UpdateAppointmentDto
{
    public DateTime ScheduledDateTime { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
}

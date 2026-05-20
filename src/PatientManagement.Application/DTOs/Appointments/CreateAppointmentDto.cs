namespace PatientManagement.Application.DTOs.Appointments;

public class CreateAppointmentDto
{
    public Guid PatientId { get; set; }
    public DateTime ScheduledDateTime { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
}

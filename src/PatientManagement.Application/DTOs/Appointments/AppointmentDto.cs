using PatientManagement.Domain.Enums;

namespace PatientManagement.Application.DTOs.Appointments;

public class AppointmentDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public PatientSummaryDto? Patient { get; set; }
    public DateTime ScheduledDateTime { get; set; }
    public AppointmentStatus Status { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CompletedTime { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

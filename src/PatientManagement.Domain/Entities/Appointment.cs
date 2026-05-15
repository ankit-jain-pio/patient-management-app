using PatientManagement.Domain.Enums;

namespace PatientManagement.Domain.Entities;

public class Appointment : BaseEntity
{
    public Guid PatientId { get; set; }
    public DateTime ScheduledDateTime { get; set; }
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;
    public string? Reason { get; set; }
    public string? Notes { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CompletedTime { get; set; }
    
    // Navigation property
    public Patient Patient { get; set; } = null!;
    public Consultation? Consultation { get; set; }
}

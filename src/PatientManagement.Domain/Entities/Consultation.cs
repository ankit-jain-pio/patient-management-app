using PatientManagement.Domain.ValueObjects;

namespace PatientManagement.Domain.Entities;

public class Consultation : BaseEntity
{
    public Guid PatientId { get; set; }
    public Guid? AppointmentId { get; set; }
    public DateTime ConsultationDate { get; set; } = DateTime.UtcNow;
    
    // Vitals
    public Vitals? Vitals { get; set; }
    
    // Clinical data
    public string? ChiefComplaint { get; set; }
    public string? Symptoms { get; set; }
    public string? Diagnosis { get; set; }
    public string? ClinicalNotes { get; set; }
    public string? TreatmentPlan { get; set; }
    public string? FollowUpInstructions { get; set; }
    public DateTime? NextVisitDate { get; set; }
    
    // Navigation properties
    public Patient Patient { get; set; } = null!;
    public Appointment? Appointment { get; set; }
    public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
}

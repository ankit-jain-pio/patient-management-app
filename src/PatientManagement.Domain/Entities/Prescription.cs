namespace PatientManagement.Domain.Entities;

public class Prescription : BaseEntity
{
    public Guid ConsultationId { get; set; }
    public string MedicationName { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public int DurationInDays { get; set; }
    public string? Instructions { get; set; }
    public DateTime PrescribedDate { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    public Consultation Consultation { get; set; } = null!;
}

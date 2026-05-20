using PatientManagement.Application.DTOs.Appointments;

namespace PatientManagement.Application.DTOs.Consultations;

public class ConsultationDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public PatientSummaryDto? Patient { get; set; }
    public Guid? AppointmentId { get; set; }
    public DateTime ConsultationDate { get; set; }
    
    // Vitals
    public VitalsDto? Vitals { get; set; }
    
    // Clinical data
    public string? ChiefComplaint { get; set; }
    public string? Symptoms { get; set; }
    public string? Diagnosis { get; set; }
    public string? ClinicalNotes { get; set; }
    public string? TreatmentPlan { get; set; }
    public string? FollowUpInstructions { get; set; }
    public DateTime? NextVisitDate { get; set; }
    
    // Prescriptions
    public List<PrescriptionDto> Prescriptions { get; set; } = new();
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

namespace PatientManagement.Application.DTOs.Consultations;

public class CreateConsultationDto
{
    public Guid PatientId { get; set; }
    public Guid? AppointmentId { get; set; }
    public VitalsDto? Vitals { get; set; }
    public string? ChiefComplaint { get; set; }
    public string? Symptoms { get; set; }
}

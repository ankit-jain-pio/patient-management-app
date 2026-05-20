using MediatR;
using PatientManagement.Application.DTOs.Consultations;

namespace PatientManagement.Application.Consultations.Commands.UpdateConsultation;

public class UpdateConsultationCommand : IRequest<ConsultationDto>
{
    public Guid Id { get; set; }
    public VitalsDto? Vitals { get; set; }
    public string? ChiefComplaint { get; set; }
    public string? Symptoms { get; set; }
    public string? Diagnosis { get; set; }
    public string? ClinicalNotes { get; set; }
    public string? TreatmentPlan { get; set; }
    public string? FollowUpInstructions { get; set; }
    public DateTime? NextVisitDate { get; set; }
}

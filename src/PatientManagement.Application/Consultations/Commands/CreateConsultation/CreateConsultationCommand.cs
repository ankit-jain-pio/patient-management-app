using MediatR;
using PatientManagement.Application.DTOs.Consultations;

namespace PatientManagement.Application.Consultations.Commands.CreateConsultation;

public class CreateConsultationCommand : IRequest<ConsultationDto>
{
    public Guid PatientId { get; set; }
    public Guid? AppointmentId { get; set; }
    public VitalsDto? Vitals { get; set; }
    public string? ChiefComplaint { get; set; }
    public string? Symptoms { get; set; }
}

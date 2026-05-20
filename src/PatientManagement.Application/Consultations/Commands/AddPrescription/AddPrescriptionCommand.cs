using MediatR;
using PatientManagement.Application.DTOs.Consultations;

namespace PatientManagement.Application.Consultations.Commands.AddPrescription;

public class AddPrescriptionCommand : IRequest<PrescriptionDto>
{
    public Guid ConsultationId { get; set; }
    public string MedicationName { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public int DurationInDays { get; set; }
    public string? Instructions { get; set; }
}

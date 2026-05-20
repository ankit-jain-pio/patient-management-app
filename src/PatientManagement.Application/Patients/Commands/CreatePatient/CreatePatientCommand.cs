using MediatR;
using PatientManagement.Application.DTOs.Patients;

namespace PatientManagement.Application.Patients.Commands.CreatePatient;

public class CreatePatientCommand : IRequest<PatientDto>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public int Gender { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public AddressDto? Address { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string? BloodGroup { get; set; }
    public string? Allergies { get; set; }
    public string? MedicalHistory { get; set; }
}

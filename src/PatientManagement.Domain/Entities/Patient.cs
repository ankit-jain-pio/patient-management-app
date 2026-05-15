using PatientManagement.Domain.Enums;
using PatientManagement.Domain.ValueObjects;

namespace PatientManagement.Domain.Entities;

public class Patient : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public Address? Address { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string? BloodGroup { get; set; }
    public string? Allergies { get; set; }
    public string? MedicalHistory { get; set; }
    
    // Navigation properties
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<Consultation> Consultations { get; set; } = new List<Consultation>();
    
    // Computed properties
    public int Age => DateTime.UtcNow.Year - DateOfBirth.Year - 
                      (DateTime.UtcNow.DayOfYear < DateOfBirth.DayOfYear ? 1 : 0);
    
    public string FullName => $"{FirstName} {LastName}";
}

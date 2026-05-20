using PatientManagement.Domain.Enums;

namespace PatientManagement.Application.DTOs.Patients;

public class PatientSearchResultDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public int Age { get; set; }
    public Gender Gender { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime LastVisit { get; set; }
    public int TotalVisits { get; set; }
}

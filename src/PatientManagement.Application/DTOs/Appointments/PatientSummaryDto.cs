namespace PatientManagement.Application.DTOs.Appointments;

public class PatientSummaryDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public int Age { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
}

namespace PatientManagement.Application.DTOs.Consultations;

public class VitalsDto
{
    public decimal? Temperature { get; set; } // Celsius
    public string? BloodPressure { get; set; } // e.g., "120/80"
    public int? PulseRate { get; set; } // beats per minute
    public decimal? Weight { get; set; } // kg
    public decimal? Height { get; set; } // cm
    public decimal? OxygenSaturation { get; set; } // percentage
    public string? RespiratoryRate { get; set; }
    public decimal? BMI { get; set; }
}

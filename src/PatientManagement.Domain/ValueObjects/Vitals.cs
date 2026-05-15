namespace PatientManagement.Domain.ValueObjects;

public class Vitals
{
    public decimal? Temperature { get; private set; } // Celsius
    public string? BloodPressure { get; private set; } // e.g., "120/80"
    public int? PulseRate { get; private set; } // beats per minute
    public decimal? Weight { get; private set; } // kg
    public decimal? Height { get; private set; } // cm
    public decimal? OxygenSaturation { get; private set; } // percentage
    public string? RespiratoryRate { get; private set; }
    
    public Vitals(
        decimal? temperature = null,
        string? bloodPressure = null,
        int? pulseRate = null,
        decimal? weight = null,
        decimal? height = null,
        decimal? oxygenSaturation = null,
        string? respiratoryRate = null)
    {
        // Validation according to assumptions (36-41°C, 40-150 bpm)
        if (temperature.HasValue && (temperature < 35 || temperature > 42))
            throw new ArgumentException("Temperature must be between 35°C and 42°C", nameof(temperature));
        
        if (pulseRate.HasValue && (pulseRate < 30 || pulseRate > 200))
            throw new ArgumentException("Pulse rate must be between 30 and 200 bpm", nameof(pulseRate));
        
        if (oxygenSaturation.HasValue && (oxygenSaturation < 0 || oxygenSaturation > 100))
            throw new ArgumentException("Oxygen saturation must be between 0 and 100%", nameof(oxygenSaturation));
        
        Temperature = temperature;
        BloodPressure = bloodPressure;
        PulseRate = pulseRate;
        Weight = weight;
        Height = height;
        OxygenSaturation = oxygenSaturation;
        RespiratoryRate = respiratoryRate;
    }
    
    public decimal? BMI => Weight.HasValue && Height.HasValue && Height > 0 
        ? Weight / ((Height / 100) * (Height / 100)) 
        : null;
}

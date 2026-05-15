using PatientManagement.Domain.ValueObjects;
using Xunit;
using FluentAssertions;

namespace PatientManagement.Domain.Tests.ValueObjects;

public class VitalsTests
{
    [Fact]
    public void Vitals_ShouldCreate_WhenValidValuesProvided()
    {
        // Arrange & Act
        var vitals = new Vitals(
            temperature: 37.5m,
            bloodPressure: "120/80",
            pulseRate: 72,
            weight: 70.5m,
            height: 175m,
            oxygenSaturation: 98m
        );
        
        // Assert
        vitals.Temperature.Should().Be(37.5m);
        vitals.BloodPressure.Should().Be("120/80");
        vitals.PulseRate.Should().Be(72);
    }
    
    [Theory]
    [InlineData(34.9)]
    [InlineData(42.1)]
    public void Vitals_ShouldThrowException_WhenTemperatureOutOfRange(decimal temperature)
    {
        // Arrange & Act
        Action act = () => new Vitals(temperature: temperature);
        
        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Temperature must be between 35°C and 42°C*");
    }
    
    [Theory]
    [InlineData(29)]
    [InlineData(201)]
    public void Vitals_ShouldThrowException_WhenPulseRateOutOfRange(int pulseRate)
    {
        // Arrange & Act
        Action act = () => new Vitals(pulseRate: pulseRate);
        
        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Pulse rate must be between 30 and 200 bpm*");
    }
    
    [Fact]
    public void Vitals_ShouldCalculateBMI_WhenWeightAndHeightProvided()
    {
        // Arrange
        var vitals = new Vitals(weight: 70m, height: 175m);
        
        // Act
        var bmi = vitals.BMI;
        
        // Assert
        bmi.Should().NotBeNull();
        bmi.Should().BeApproximately(22.86m, 0.01m);
    }
    
    [Fact]
    public void Vitals_ShouldReturnNullBMI_WhenWeightOrHeightMissing()
    {
        // Arrange
        var vitals = new Vitals(weight: 70m);
        
        // Act
        var bmi = vitals.BMI;
        
        // Assert
        bmi.Should().BeNull();
    }
}

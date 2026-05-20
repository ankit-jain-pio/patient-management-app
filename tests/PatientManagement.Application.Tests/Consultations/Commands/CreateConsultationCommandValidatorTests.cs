using FluentAssertions;
using PatientManagement.Application.Consultations.Commands.CreateConsultation;
using PatientManagement.Application.DTOs.Consultations;
using Xunit;

namespace PatientManagement.Application.Tests.Consultations.Commands;

public class CreateConsultationCommandValidatorTests
{
    private readonly CreateConsultationCommandValidator _validator;

    public CreateConsultationCommandValidatorTests()
    {
        _validator = new CreateConsultationCommandValidator();
    }

    [Fact]
    public void Validator_ShouldPass_WhenValidCommandProvided()
    {
        // Arrange
        var command = new CreateConsultationCommand
        {
            PatientId = Guid.NewGuid(),
            ChiefComplaint = "Fever",
            Vitals = new VitalsDto
            {
                Temperature = 37.5m,
                PulseRate = 80
            }
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validator_ShouldFail_WhenPatientIdIsEmpty()
    {
        // Arrange
        var command = new CreateConsultationCommand
        {
            PatientId = Guid.Empty
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PatientId");
    }

    [Fact]
    public void Validator_ShouldFail_WhenTemperatureIsOutOfRange()
    {
        // Arrange
        var command = new CreateConsultationCommand
        {
            PatientId = Guid.NewGuid(),
            Vitals = new VitalsDto
            {
                Temperature = 45m // Too high
            }
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.Contains("Temperature"));
    }

    [Fact]
    public void Validator_ShouldFail_WhenPulseRateIsOutOfRange()
    {
        // Arrange
        var command = new CreateConsultationCommand
        {
            PatientId = Guid.NewGuid(),
            Vitals = new VitalsDto
            {
                PulseRate = 250 // Too high
            }
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.Contains("PulseRate"));
    }

    [Fact]
    public void Validator_ShouldPass_WhenVitalsNotProvided()
    {
        // Arrange
        var command = new CreateConsultationCommand
        {
            PatientId = Guid.NewGuid(),
            ChiefComplaint = "Regular checkup"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}

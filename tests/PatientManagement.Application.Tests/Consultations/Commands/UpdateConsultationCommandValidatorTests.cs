using FluentAssertions;
using PatientManagement.Application.Consultations.Commands.UpdateConsultation;
using PatientManagement.Application.DTOs.Consultations;
using Xunit;

namespace PatientManagement.Application.Tests.Consultations.Commands;

public class UpdateConsultationCommandValidatorTests
{
    private readonly UpdateConsultationCommandValidator _validator;

    public UpdateConsultationCommandValidatorTests()
    {
        _validator = new UpdateConsultationCommandValidator();
    }

    [Fact]
    public void Validator_ShouldPass_WhenValidCommandProvided()
    {
        // Arrange
        var command = new UpdateConsultationCommand
        {
            Id = Guid.NewGuid(),
            Diagnosis = "Common cold",
            TreatmentPlan = "Rest and fluids"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validator_ShouldFail_WhenIdIsEmpty()
    {
        // Arrange
        var command = new UpdateConsultationCommand
        {
            Id = Guid.Empty,
            Diagnosis = "Test"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
    }

    [Fact]
    public void Validator_ShouldFail_WhenNextVisitDateIsInPast()
    {
        // Arrange
        var command = new UpdateConsultationCommand
        {
            Id = Guid.NewGuid(),
            NextVisitDate = DateTime.UtcNow.AddDays(-10)
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "NextVisitDate");
    }
}

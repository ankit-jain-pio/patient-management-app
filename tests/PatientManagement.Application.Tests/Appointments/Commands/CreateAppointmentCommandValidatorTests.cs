using FluentAssertions;
using PatientManagement.Application.Appointments.Commands.CreateAppointment;
using Xunit;

namespace PatientManagement.Application.Tests.Appointments.Commands;

public class CreateAppointmentCommandValidatorTests
{
    private readonly CreateAppointmentCommandValidator _validator;

    public CreateAppointmentCommandValidatorTests()
    {
        _validator = new CreateAppointmentCommandValidator();
    }

    [Fact]
    public void Validator_ShouldPass_WhenAllRequiredFieldsAreValid()
    {
        // Arrange
        var command = new CreateAppointmentCommand
        {
            PatientId = Guid.NewGuid(),
            ScheduledDateTime = DateTime.UtcNow.AddDays(1),
            Reason = "Regular checkup"
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
        var command = new CreateAppointmentCommand
        {
            PatientId = Guid.Empty,
            ScheduledDateTime = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PatientId");
    }

    [Fact]
    public void Validator_ShouldFail_WhenScheduledDateTimeIsInPast()
    {
        // Arrange
        var command = new CreateAppointmentCommand
        {
            PatientId = Guid.NewGuid(),
            ScheduledDateTime = DateTime.UtcNow.AddDays(-1)
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ScheduledDateTime");
    }

    [Fact]
    public void Validator_ShouldFail_WhenReasonExceedsMaxLength()
    {
        // Arrange
        var command = new CreateAppointmentCommand
        {
            PatientId = Guid.NewGuid(),
            ScheduledDateTime = DateTime.UtcNow.AddDays(1),
            Reason = new string('A', 501) // Exceeds 500 character limit
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Reason");
    }
}

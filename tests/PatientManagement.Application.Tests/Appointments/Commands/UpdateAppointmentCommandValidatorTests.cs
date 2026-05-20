using FluentAssertions;
using PatientManagement.Application.Appointments.Commands.UpdateAppointment;
using Xunit;

namespace PatientManagement.Application.Tests.Appointments.Commands;

public class UpdateAppointmentCommandValidatorTests
{
    private readonly UpdateAppointmentCommandValidator _validator;

    public UpdateAppointmentCommandValidatorTests()
    {
        _validator = new UpdateAppointmentCommandValidator();
    }

    [Fact]
    public void Validator_ShouldPass_WhenAllRequiredFieldsAreValid()
    {
        // Arrange
        var command = new UpdateAppointmentCommand
        {
            Id = Guid.NewGuid(),
            ScheduledDateTime = DateTime.UtcNow.AddDays(1)
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
        var command = new UpdateAppointmentCommand
        {
            Id = Guid.Empty,
            ScheduledDateTime = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
    }

    [Fact]
    public void Validator_ShouldFail_WhenScheduledDateTimeIsInPast()
    {
        // Arrange
        var command = new UpdateAppointmentCommand
        {
            Id = Guid.NewGuid(),
            ScheduledDateTime = DateTime.UtcNow.AddDays(-1)
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ScheduledDateTime");
    }
}

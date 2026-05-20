using FluentAssertions;
using PatientManagement.Application.Appointments.Commands.UpdateAppointmentStatus;
using PatientManagement.Domain.Enums;
using Xunit;

namespace PatientManagement.Application.Tests.Appointments.Commands;

public class UpdateAppointmentStatusCommandValidatorTests
{
    private readonly UpdateAppointmentStatusCommandValidator _validator;

    public UpdateAppointmentStatusCommandValidatorTests()
    {
        _validator = new UpdateAppointmentStatusCommandValidator();
    }

    [Fact]
    public void Validator_ShouldPass_WhenAllRequiredFieldsAreValid()
    {
        // Arrange
        var command = new UpdateAppointmentStatusCommand
        {
            Id = Guid.NewGuid(),
            Status = AppointmentStatus.Completed
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
        var command = new UpdateAppointmentStatusCommand
        {
            Id = Guid.Empty,
            Status = AppointmentStatus.Completed
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
    }

    [Fact]
    public void Validator_ShouldFail_WhenStatusIsInvalid()
    {
        // Arrange
        var command = new UpdateAppointmentStatusCommand
        {
            Id = Guid.NewGuid(),
            Status = (AppointmentStatus)999
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Status");
    }
}

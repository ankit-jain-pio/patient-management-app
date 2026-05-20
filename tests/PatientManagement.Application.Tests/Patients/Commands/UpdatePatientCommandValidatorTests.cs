using FluentAssertions;
using PatientManagement.Application.Patients.Commands.UpdatePatient;
using Xunit;

namespace PatientManagement.Application.Tests.Patients.Commands;

public class UpdatePatientCommandValidatorTests
{
    private readonly UpdatePatientCommandValidator _validator;

    public UpdatePatientCommandValidatorTests()
    {
        _validator = new UpdatePatientCommandValidator();
    }

    [Fact]
    public void Validator_ShouldPass_WhenAllRequiredFieldsAreValid()
    {
        // Arrange
        var command = new UpdatePatientCommand
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = DateTime.UtcNow.AddYears(-30),
            Gender = (int)Domain.Enums.Gender.Male,
            PhoneNumber = "1234567890"
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
        var command = new UpdatePatientCommand
        {
            Id = Guid.Empty,
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = DateTime.UtcNow.AddYears(-30),
            Gender = (int)Domain.Enums.Gender.Male,
            PhoneNumber = "1234567890"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
    }

    [Fact]
    public void Validator_ShouldFail_WhenFirstNameIsEmpty()
    {
        // Arrange
        var command = new UpdatePatientCommand
        {
            Id = Guid.NewGuid(),
            FirstName = "",
            LastName = "Doe",
            DateOfBirth = DateTime.UtcNow.AddYears(-30),
            Gender = (int)Domain.Enums.Gender.Male,
            PhoneNumber = "1234567890"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FirstName");
    }

    [Fact]
    public void Validator_ShouldFail_WhenPhoneNumberContainsInvalidCharacters()
    {
        // Arrange
        var command = new UpdatePatientCommand
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = DateTime.UtcNow.AddYears(-30),
            Gender = (int)Domain.Enums.Gender.Male,
            PhoneNumber = "123-ABC-7890"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PhoneNumber");
    }
}

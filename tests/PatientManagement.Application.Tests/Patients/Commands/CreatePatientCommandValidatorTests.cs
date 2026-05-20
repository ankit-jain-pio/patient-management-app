using FluentAssertions;
using PatientManagement.Application.DTOs.Patients;
using PatientManagement.Application.Patients.Commands.CreatePatient;
using Xunit;

namespace PatientManagement.Application.Tests.Patients.Commands;

public class CreatePatientCommandValidatorTests
{
    private readonly CreatePatientCommandValidator _validator;

    public CreatePatientCommandValidatorTests()
    {
        _validator = new CreatePatientCommandValidator();
    }

    [Fact]
    public void Validator_ShouldPass_WhenAllRequiredFieldsAreValid()
    {
        // Arrange
        var command = new CreatePatientCommand
        {
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
    public void Validator_ShouldFail_WhenFirstNameIsEmpty()
    {
        // Arrange
        var command = new CreatePatientCommand
        {
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
    public void Validator_ShouldFail_WhenDateOfBirthIsInFuture()
    {
        // Arrange
        var command = new CreatePatientCommand
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = DateTime.UtcNow.AddDays(1),
            Gender = (int)Domain.Enums.Gender.Male,
            PhoneNumber = "1234567890"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "DateOfBirth");
    }

    [Fact]
    public void Validator_ShouldFail_WhenPhoneNumberContainsInvalidCharacters()
    {
        // Arrange
        var command = new CreatePatientCommand
        {
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

    [Fact]
    public void Validator_ShouldFail_WhenEmailIsInvalid()
    {
        // Arrange
        var command = new CreatePatientCommand
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = DateTime.UtcNow.AddYears(-30),
            Gender = (int)Domain.Enums.Gender.Male,
            PhoneNumber = "1234567890",
            Email = "invalid-email"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public void Validator_ShouldFail_WhenAddressIsProvidedButIncomplete()
    {
        // Arrange
        var command = new CreatePatientCommand
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = DateTime.UtcNow.AddYears(-30),
            Gender = (int)Domain.Enums.Gender.Male,
            PhoneNumber = "1234567890",
            Address = new AddressDto
            {
                Street = "123 Main St",
                City = "", // Missing required field
                State = "CA",
                PostalCode = "12345"
            }
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.Contains("Address"));
    }
}

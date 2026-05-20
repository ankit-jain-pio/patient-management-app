using FluentAssertions;
using PatientManagement.Application.Consultations.Commands.AddPrescription;
using Xunit;

namespace PatientManagement.Application.Tests.Consultations.Commands;

public class AddPrescriptionCommandValidatorTests
{
    private readonly AddPrescriptionCommandValidator _validator;

    public AddPrescriptionCommandValidatorTests()
    {
        _validator = new AddPrescriptionCommandValidator();
    }

    [Fact]
    public void Validator_ShouldPass_WhenValidCommandProvided()
    {
        // Arrange
        var command = new AddPrescriptionCommand
        {
            ConsultationId = Guid.NewGuid(),
            MedicationName = "Paracetamol",
            Dosage = "500mg",
            Frequency = "Twice daily",
            DurationInDays = 5
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validator_ShouldFail_WhenConsultationIdIsEmpty()
    {
        // Arrange
        var command = new AddPrescriptionCommand
        {
            ConsultationId = Guid.Empty,
            MedicationName = "Test",
            Dosage = "100mg",
            Frequency = "Once",
            DurationInDays = 5
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ConsultationId");
    }

    [Fact]
    public void Validator_ShouldFail_WhenMedicationNameIsEmpty()
    {
        // Arrange
        var command = new AddPrescriptionCommand
        {
            ConsultationId = Guid.NewGuid(),
            MedicationName = "",
            Dosage = "100mg",
            Frequency = "Once",
            DurationInDays = 5
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "MedicationName");
    }

    [Fact]
    public void Validator_ShouldFail_WhenDurationIsZeroOrNegative()
    {
        // Arrange
        var command = new AddPrescriptionCommand
        {
            ConsultationId = Guid.NewGuid(),
            MedicationName = "Test",
            Dosage = "100mg",
            Frequency = "Once",
            DurationInDays = 0
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "DurationInDays");
    }

    [Fact]
    public void Validator_ShouldFail_WhenDurationExceeds365Days()
    {
        // Arrange
        var command = new AddPrescriptionCommand
        {
            ConsultationId = Guid.NewGuid(),
            MedicationName = "Test",
            Dosage = "100mg",
            Frequency = "Once",
            DurationInDays = 400
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "DurationInDays");
    }
}

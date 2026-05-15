using PatientManagement.Domain.Entities;
using PatientManagement.Domain.Enums;
using Xunit;
using FluentAssertions;

namespace PatientManagement.Domain.Tests.Entities;

public class PatientTests
{
    [Fact]
    public void Patient_ShouldCalculateAge_WhenDateOfBirthIsSet()
    {
        // Arrange
        var dateOfBirth = new DateTime(1990, 5, 15);
        var patient = new Patient
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = dateOfBirth,
            Gender = Gender.Male,
            PhoneNumber = "1234567890"
        };
        
        // Act
        var age = patient.Age;
        
        // Assert
        var expectedAge = DateTime.UtcNow.Year - dateOfBirth.Year;
        if (DateTime.UtcNow.DayOfYear < dateOfBirth.DayOfYear)
            expectedAge--;
        
        age.Should().Be(expectedAge);
    }
    
    [Fact]
    public void Patient_ShouldReturnFullName_WhenFirstAndLastNameAreSet()
    {
        // Arrange & Act
        var patient = new Patient
        {
            FirstName = "Jane",
            LastName = "Smith",
            DateOfBirth = DateTime.UtcNow.AddYears(-30),
            Gender = Gender.Female,
            PhoneNumber = "9876543210"
        };
        
        // Assert
        patient.FullName.Should().Be("Jane Smith");
    }
    
    [Fact]
    public void Patient_ShouldHaveEmptyCollections_WhenInitialized()
    {
        // Arrange & Act
        var patient = new Patient
        {
            FirstName = "Test",
            LastName = "User",
            DateOfBirth = DateTime.UtcNow.AddYears(-25),
            Gender = Gender.Other,
            PhoneNumber = "5555555555"
        };
        
        // Assert
        patient.Appointments.Should().BeEmpty();
        patient.Consultations.Should().BeEmpty();
    }
}

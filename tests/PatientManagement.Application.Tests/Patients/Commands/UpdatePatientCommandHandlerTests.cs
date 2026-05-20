using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PatientManagement.Application.DTOs.Patients;
using PatientManagement.Application.Patients.Commands.UpdatePatient;
using PatientManagement.Domain.Entities;
using PatientManagement.Domain.Enums;
using Xunit;

namespace PatientManagement.Application.Tests.Patients.Commands;

public class UpdatePatientCommandHandlerTests
{
    [Fact]
    public async Task Handler_ShouldUpdatePatient_WhenValidCommandProvided()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<PatientManagement.Infrastructure.Data.ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new PatientManagement.Infrastructure.Data.ApplicationDbContext(options);
        
        var existingPatient = new Patient
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(1990, 1, 1),
            Gender = Gender.Male,
            PhoneNumber = "1234567890"
        };
        context.Patients.Add(existingPatient);
        await context.SaveChangesAsync();

        var handler = new UpdatePatientCommandHandler(context);

        var command = new UpdatePatientCommand
        {
            Id = existingPatient.Id,
            FirstName = "Jane",
            LastName = "Smith",
            DateOfBirth = new DateTime(1985, 5, 15),
            Gender = (int)Gender.Female,
            PhoneNumber = "9876543210",
            Email = "jane.smith@example.com"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.FirstName.Should().Be("Jane");
        result.LastName.Should().Be("Smith");
        result.Email.Should().Be("jane.smith@example.com");

        var updatedPatient = await context.Patients.FirstAsync(p => p.Id == existingPatient.Id);
        updatedPatient.FirstName.Should().Be("Jane");
        updatedPatient.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Handler_ShouldThrowException_WhenPatientNotFound()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<PatientManagement.Infrastructure.Data.ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new PatientManagement.Infrastructure.Data.ApplicationDbContext(options);
        var handler = new UpdatePatientCommandHandler(context);

        var command = new UpdatePatientCommand
        {
            Id = Guid.NewGuid(),
            FirstName = "Jane",
            LastName = "Smith",
            DateOfBirth = new DateTime(1985, 5, 15),
            Gender = (int)Gender.Female,
            PhoneNumber = "9876543210"
        };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }
}

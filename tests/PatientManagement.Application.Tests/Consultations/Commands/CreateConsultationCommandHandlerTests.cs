using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PatientManagement.Application.Consultations.Commands.CreateConsultation;
using PatientManagement.Application.DTOs.Consultations;
using PatientManagement.Domain.Entities;
using PatientManagement.Domain.Enums;
using Xunit;

namespace PatientManagement.Application.Tests.Consultations.Commands;

public class CreateConsultationCommandHandlerTests
{
    [Fact]
    public async Task Handler_ShouldCreateConsultation_WhenValidCommandProvided()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<PatientManagement.Infrastructure.Data.ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new PatientManagement.Infrastructure.Data.ApplicationDbContext(options);
        
        var patient = new Patient
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(1990, 1, 1),
            Gender = Gender.Male,
            PhoneNumber = "1234567890"
        };
        context.Patients.Add(patient);
        await context.SaveChangesAsync();

        var handler = new CreateConsultationCommandHandler(context);

        var command = new CreateConsultationCommand
        {
            PatientId = patient.Id,
            ChiefComplaint = "Fever and headache",
            Symptoms = "High temperature, persistent headache",
            Vitals = new VitalsDto
            {
                Temperature = 38.5m,
                BloodPressure = "120/80",
                PulseRate = 80
            }
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.PatientId.Should().Be(patient.Id);
        result.ChiefComplaint.Should().Be("Fever and headache");
        result.Vitals.Should().NotBeNull();
        result.Vitals!.Temperature.Should().Be(38.5m);
        result.Patient.Should().NotBeNull();
        result.Patient!.FullName.Should().Be("John Doe");

        var savedConsultation = await context.Consultations.FirstOrDefaultAsync();
        savedConsultation.Should().NotBeNull();
        savedConsultation!.PatientId.Should().Be(patient.Id);
    }

    [Fact]
    public async Task Handler_ShouldThrowException_WhenPatientNotFound()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<PatientManagement.Infrastructure.Data.ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new PatientManagement.Infrastructure.Data.ApplicationDbContext(options);
        var handler = new CreateConsultationCommandHandler(context);

        var command = new CreateConsultationCommand
        {
            PatientId = Guid.NewGuid(),
            ChiefComplaint = "Test"
        };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handler_ShouldCreateConsultationWithoutVitals_WhenVitalsNotProvided()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<PatientManagement.Infrastructure.Data.ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new PatientManagement.Infrastructure.Data.ApplicationDbContext(options);
        
        var patient = new Patient
        {
            FirstName = "Jane",
            LastName = "Smith",
            DateOfBirth = new DateTime(1985, 5, 15),
            Gender = Gender.Female,
            PhoneNumber = "9876543210"
        };
        context.Patients.Add(patient);
        await context.SaveChangesAsync();

        var handler = new CreateConsultationCommandHandler(context);

        var command = new CreateConsultationCommand
        {
            PatientId = patient.Id,
            ChiefComplaint = "Regular checkup"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Vitals.Should().BeNull();
    }
}

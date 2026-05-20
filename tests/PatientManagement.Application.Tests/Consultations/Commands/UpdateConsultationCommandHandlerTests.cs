using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PatientManagement.Application.Consultations.Commands.UpdateConsultation;
using PatientManagement.Application.DTOs.Consultations;
using PatientManagement.Domain.Entities;
using PatientManagement.Domain.Enums;
using Xunit;

namespace PatientManagement.Application.Tests.Consultations.Commands;

public class UpdateConsultationCommandHandlerTests
{
    [Fact]
    public async Task Handler_ShouldUpdateConsultation_WhenValidCommandProvided()
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
        
        var consultation = new Consultation
        {
            PatientId = patient.Id,
            ChiefComplaint = "Initial complaint",
            ConsultationDate = DateTime.UtcNow
        };
        
        context.Patients.Add(patient);
        context.Consultations.Add(consultation);
        await context.SaveChangesAsync();

        var handler = new UpdateConsultationCommandHandler(context);

        var command = new UpdateConsultationCommand
        {
            Id = consultation.Id,
            Diagnosis = "Common cold",
            TreatmentPlan = "Rest and fluids",
            Vitals = new VitalsDto
            {
                Temperature = 37.2m,
                PulseRate = 75
            }
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Diagnosis.Should().Be("Common cold");
        result.TreatmentPlan.Should().Be("Rest and fluids");
        result.Vitals.Should().NotBeNull();
        result.Vitals!.Temperature.Should().Be(37.2m);

        var updatedConsultation = await context.Consultations.FirstAsync(c => c.Id == consultation.Id);
        updatedConsultation.Diagnosis.Should().Be("Common cold");
        updatedConsultation.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Handler_ShouldThrowException_WhenConsultationNotFound()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<PatientManagement.Infrastructure.Data.ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new PatientManagement.Infrastructure.Data.ApplicationDbContext(options);
        var handler = new UpdateConsultationCommandHandler(context);

        var command = new UpdateConsultationCommand
        {
            Id = Guid.NewGuid(),
            Diagnosis = "Test"
        };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }
}

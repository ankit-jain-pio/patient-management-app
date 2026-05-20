using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PatientManagement.Application.Consultations.Queries.GetConsultation;
using PatientManagement.Domain.Entities;
using PatientManagement.Domain.Enums;
using PatientManagement.Domain.ValueObjects;
using Xunit;

namespace PatientManagement.Application.Tests.Consultations.Queries;

public class GetConsultationQueryHandlerTests
{
    [Fact]
    public async Task Handler_ShouldReturnConsultation_WhenConsultationExists()
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
            ChiefComplaint = "Fever",
            Symptoms = "High temperature",
            Diagnosis = "Common cold",
            Vitals = new Vitals(38.5m, "120/80", 80),
            ConsultationDate = DateTime.UtcNow
        };
        
        context.Patients.Add(patient);
        context.Consultations.Add(consultation);
        await context.SaveChangesAsync();

        var handler = new GetConsultationQueryHandler(context);
        var query = new GetConsultationQuery { Id = consultation.Id };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(consultation.Id);
        result.PatientId.Should().Be(patient.Id);
        result.ChiefComplaint.Should().Be("Fever");
        result.Diagnosis.Should().Be("Common cold");
        result.Vitals.Should().NotBeNull();
        result.Vitals!.Temperature.Should().Be(38.5m);
        result.Patient.Should().NotBeNull();
        result.Patient!.FullName.Should().Be("John Doe");
    }

    [Fact]
    public async Task Handler_ShouldReturnNull_WhenConsultationDoesNotExist()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<PatientManagement.Infrastructure.Data.ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new PatientManagement.Infrastructure.Data.ApplicationDbContext(options);
        var handler = new GetConsultationQueryHandler(context);
        var query = new GetConsultationQuery { Id = Guid.NewGuid() };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}

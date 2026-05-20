using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PatientManagement.Application.Consultations.Queries.GetPatientConsultations;
using PatientManagement.Domain.Entities;
using PatientManagement.Domain.Enums;
using PatientManagement.Domain.ValueObjects;
using Xunit;

namespace PatientManagement.Application.Tests.Consultations.Queries;

public class GetPatientConsultationsQueryHandlerTests
{
    [Fact]
    public async Task Handler_ShouldReturnAllConsultationsForPatient_WhenConsultationsExist()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<PatientManagement.Infrastructure.Data.ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new PatientManagement.Infrastructure.Data.ApplicationDbContext(options);
        
        var patient1 = new Patient
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(1990, 1, 1),
            Gender = Gender.Male,
            PhoneNumber = "1111111111"
        };
        
        var patient2 = new Patient
        {
            FirstName = "Jane",
            LastName = "Smith",
            DateOfBirth = new DateTime(1985, 5, 15),
            Gender = Gender.Female,
            PhoneNumber = "2222222222"
        };
        
        context.Patients.AddRange(patient1, patient2);
        
        // Consultations for patient1
        context.Consultations.AddRange(
            new Consultation
            {
                PatientId = patient1.Id,
                ChiefComplaint = "Fever",
                ConsultationDate = DateTime.UtcNow.AddDays(-7),
                Vitals = new Vitals(38.5m, "120/80", 80)
            },
            new Consultation
            {
                PatientId = patient1.Id,
                ChiefComplaint = "Headache",
                ConsultationDate = DateTime.UtcNow.AddDays(-1)
            },
            new Consultation
            {
                PatientId = patient1.Id,
                ChiefComplaint = "Checkup",
                ConsultationDate = DateTime.UtcNow
            }
        );
        
        // Consultation for patient2 (should not be included)
        context.Consultations.Add(new Consultation
        {
            PatientId = patient2.Id,
            ChiefComplaint = "Different patient",
            ConsultationDate = DateTime.UtcNow
        });
        
        await context.SaveChangesAsync();

        var handler = new GetPatientConsultationsQueryHandler(context);
        var query = new GetPatientConsultationsQuery { PatientId = patient1.Id };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().BeInDescendingOrder(c => c.ConsultationDate);
        result.Should().OnlyContain(c => c.PatientId == patient1.Id);
        result[0].ChiefComplaint.Should().Be("Checkup");
    }

    [Fact]
    public async Task Handler_ShouldReturnEmptyList_WhenPatientHasNoConsultations()
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
            PhoneNumber = "1111111111"
        };
        
        context.Patients.Add(patient);
        await context.SaveChangesAsync();

        var handler = new GetPatientConsultationsQueryHandler(context);
        var query = new GetPatientConsultationsQuery { PatientId = patient.Id };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}

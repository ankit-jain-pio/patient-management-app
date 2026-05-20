using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PatientManagement.Application.Patients.Queries.GetPatient;
using PatientManagement.Domain.Entities;
using PatientManagement.Domain.Enums;
using Xunit;

namespace PatientManagement.Application.Tests.Patients.Queries;

public class GetPatientQueryHandlerTests
{
    [Fact]
    public async Task Handler_ShouldReturnPatient_WhenPatientExists()
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
            PhoneNumber = "1234567890",
            Email = "john.doe@example.com"
        };
        context.Patients.Add(patient);
        await context.SaveChangesAsync();

        var handler = new GetPatientQueryHandler(context);
        var query = new GetPatientQuery { Id = patient.Id };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(patient.Id);
        result.FirstName.Should().Be("John");
        result.LastName.Should().Be("Doe");
        result.FullName.Should().Be("John Doe");
        result.Email.Should().Be("john.doe@example.com");
    }

    [Fact]
    public async Task Handler_ShouldReturnNull_WhenPatientDoesNotExist()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<PatientManagement.Infrastructure.Data.ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new PatientManagement.Infrastructure.Data.ApplicationDbContext(options);
        var handler = new GetPatientQueryHandler(context);
        var query = new GetPatientQuery { Id = Guid.NewGuid() };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}

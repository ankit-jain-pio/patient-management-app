using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PatientManagement.Application.Patients.Queries.GetAllPatients;
using PatientManagement.Domain.Entities;
using PatientManagement.Domain.Enums;
using Xunit;

namespace PatientManagement.Application.Tests.Patients.Queries;

public class GetAllPatientsQueryHandlerTests
{
    [Fact]
    public async Task Handler_ShouldReturnAllPatients_WhenPatientsExist()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<PatientManagement.Infrastructure.Data.ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new PatientManagement.Infrastructure.Data.ApplicationDbContext(options);
        
        context.Patients.AddRange(
            new Patient
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateTime(1990, 1, 1),
                Gender = Gender.Male,
                PhoneNumber = "1111111111"
            },
            new Patient
            {
                FirstName = "Jane",
                LastName = "Smith",
                DateOfBirth = new DateTime(1985, 5, 15),
                Gender = Gender.Female,
                PhoneNumber = "2222222222"
            },
            new Patient
            {
                FirstName = "Bob",
                LastName = "Anderson",
                DateOfBirth = new DateTime(1995, 10, 20),
                Gender = Gender.Male,
                PhoneNumber = "3333333333"
            }
        );
        await context.SaveChangesAsync();

        var handler = new GetAllPatientsQueryHandler(context);
        var query = new GetAllPatientsQuery { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().BeInAscendingOrder(p => p.LastName);
    }

    [Fact]
    public async Task Handler_ShouldReturnPaginatedResults_WhenPageSizeSpecified()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<PatientManagement.Infrastructure.Data.ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new PatientManagement.Infrastructure.Data.ApplicationDbContext(options);
        
        for (int i = 1; i <= 15; i++)
        {
            context.Patients.Add(new Patient
            {
                FirstName = $"Patient{i}",
                LastName = $"Last{i:D2}",
                DateOfBirth = new DateTime(1990, 1, 1),
                Gender = Gender.Male,
                PhoneNumber = $"{i:D10}"
            });
        }
        await context.SaveChangesAsync();

        var handler = new GetAllPatientsQueryHandler(context);
        var query = new GetAllPatientsQuery { PageNumber = 1, PageSize = 5 };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(5);
        result[0].LastName.Should().Be("Last01");
    }
}

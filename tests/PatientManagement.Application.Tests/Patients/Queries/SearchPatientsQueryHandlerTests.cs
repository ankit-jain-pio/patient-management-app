using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PatientManagement.Application.Patients.Queries.SearchPatients;
using PatientManagement.Domain.Entities;
using PatientManagement.Domain.Enums;
using Xunit;

namespace PatientManagement.Application.Tests.Patients.Queries;

public class SearchPatientsQueryHandlerTests
{
    [Fact]
    public async Task Handler_ShouldReturnMatchingPatients_WhenSearchingByFirstName()
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
                LastName = "Johnson",
                DateOfBirth = new DateTime(1995, 10, 20),
                Gender = Gender.Male,
                PhoneNumber = "3333333333"
            }
        );
        await context.SaveChangesAsync();

        var handler = new SearchPatientsQueryHandler(context);
        var query = new SearchPatientsQuery { SearchTerm = "john", PageNumber = 1, PageSize = 20 };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2); // "John" and "Johnson"
        result.Should().Contain(p => p.FirstName == "John");
        result.Should().Contain(p => p.LastName == "Johnson");
    }

    [Fact]
    public async Task Handler_ShouldReturnMatchingPatients_WhenSearchingByPhoneNumber()
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
                PhoneNumber = "555-1234"
            },
            new Patient
            {
                FirstName = "Jane",
                LastName = "Smith",
                DateOfBirth = new DateTime(1985, 5, 15),
                Gender = Gender.Female,
                PhoneNumber = "555-5678"
            }
        );
        await context.SaveChangesAsync();

        var handler = new SearchPatientsQueryHandler(context);
        var query = new SearchPatientsQuery { SearchTerm = "555-1234", PageNumber = 1, PageSize = 20 };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result[0].PhoneNumber.Should().Be("555-1234");
    }

    [Fact]
    public async Task Handler_ShouldReturnAllPatients_WhenSearchTermIsEmpty()
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
            }
        );
        await context.SaveChangesAsync();

        var handler = new SearchPatientsQueryHandler(context);
        var query = new SearchPatientsQuery { SearchTerm = null, PageNumber = 1, PageSize = 20 };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
    }
}

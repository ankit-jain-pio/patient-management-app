using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using PatientManagement.Application.Common.Interfaces;
using PatientManagement.Application.DTOs.Patients;
using PatientManagement.Application.Patients.Commands.CreatePatient;
using PatientManagement.Domain.Entities;
using PatientManagement.Domain.Enums;
using Xunit;

namespace PatientManagement.Application.Tests.Patients.Commands;

public class CreatePatientCommandHandlerTests
{
    [Fact]
    public async Task Handler_ShouldCreatePatient_WhenValidCommandProvided()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<PatientManagement.Infrastructure.Data.ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new PatientManagement.Infrastructure.Data.ApplicationDbContext(options);
        var handler = new CreatePatientCommandHandler(context);

        var command = new CreatePatientCommand
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(1990, 1, 1),
            Gender = (int)Gender.Male,
            PhoneNumber = "1234567890",
            Email = "john.doe@example.com"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.FirstName.Should().Be("John");
        result.LastName.Should().Be("Doe");
        result.FullName.Should().Be("John Doe");
        result.Email.Should().Be("john.doe@example.com");

        var savedPatient = await context.Patients.FirstOrDefaultAsync();
        savedPatient.Should().NotBeNull();
        savedPatient!.FirstName.Should().Be("John");
    }

    [Fact]
    public async Task Handler_ShouldCreatePatientWithAddress_WhenAddressProvided()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<PatientManagement.Infrastructure.Data.ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new PatientManagement.Infrastructure.Data.ApplicationDbContext(options);
        var handler = new CreatePatientCommandHandler(context);

        var command = new CreatePatientCommand
        {
            FirstName = "Jane",
            LastName = "Smith",
            DateOfBirth = new DateTime(1985, 5, 15),
            Gender = (int)Gender.Female,
            PhoneNumber = "9876543210",
            Address = new AddressDto
            {
                Street = "123 Main St",
                City = "Springfield",
                State = "IL",
                PostalCode = "62701",
                Country = "USA"
            }
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Address.Should().NotBeNull();
        result.Address!.Street.Should().Be("123 Main St");
        result.Address.City.Should().Be("Springfield");
    }
}

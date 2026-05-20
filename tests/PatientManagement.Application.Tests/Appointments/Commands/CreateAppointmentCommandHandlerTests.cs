using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PatientManagement.Application.Appointments.Commands.CreateAppointment;
using PatientManagement.Domain.Entities;
using PatientManagement.Domain.Enums;
using Xunit;

namespace PatientManagement.Application.Tests.Appointments.Commands;

public class CreateAppointmentCommandHandlerTests
{
    [Fact]
    public async Task Handler_ShouldCreateAppointment_WhenValidCommandProvided()
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

        var handler = new CreateAppointmentCommandHandler(context);

        var command = new CreateAppointmentCommand
        {
            PatientId = patient.Id,
            ScheduledDateTime = DateTime.UtcNow.AddDays(1),
            Reason = "Annual Checkup",
            Notes = "Patient prefers morning appointments"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.PatientId.Should().Be(patient.Id);
        result.Status.Should().Be(AppointmentStatus.Scheduled);
        result.Reason.Should().Be("Annual Checkup");
        result.Patient.Should().NotBeNull();
        result.Patient!.FullName.Should().Be("John Doe");

        var savedAppointment = await context.Appointments.FirstOrDefaultAsync();
        savedAppointment.Should().NotBeNull();
        savedAppointment!.PatientId.Should().Be(patient.Id);
    }

    [Fact]
    public async Task Handler_ShouldThrowException_WhenPatientNotFound()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<PatientManagement.Infrastructure.Data.ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new PatientManagement.Infrastructure.Data.ApplicationDbContext(options);
        var handler = new CreateAppointmentCommandHandler(context);

        var command = new CreateAppointmentCommand
        {
            PatientId = Guid.NewGuid(),
            ScheduledDateTime = DateTime.UtcNow.AddDays(1),
            Reason = "Annual Checkup"
        };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }
}

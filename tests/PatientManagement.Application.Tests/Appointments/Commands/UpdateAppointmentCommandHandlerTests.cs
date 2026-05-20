using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PatientManagement.Application.Appointments.Commands.UpdateAppointment;
using PatientManagement.Domain.Entities;
using PatientManagement.Domain.Enums;
using Xunit;

namespace PatientManagement.Application.Tests.Appointments.Commands;

public class UpdateAppointmentCommandHandlerTests
{
    [Fact]
    public async Task Handler_ShouldUpdateAppointment_WhenValidCommandProvided()
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
        
        var appointment = new Appointment
        {
            PatientId = patient.Id,
            ScheduledDateTime = DateTime.UtcNow.AddDays(1),
            Reason = "Initial reason",
            Status = AppointmentStatus.Scheduled
        };
        
        context.Patients.Add(patient);
        context.Appointments.Add(appointment);
        await context.SaveChangesAsync();

        var handler = new UpdateAppointmentCommandHandler(context);

        var command = new UpdateAppointmentCommand
        {
            Id = appointment.Id,
            ScheduledDateTime = DateTime.UtcNow.AddDays(2),
            Reason = "Updated reason",
            Notes = "Patient requested time change"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Reason.Should().Be("Updated reason");
        result.Notes.Should().Be("Patient requested time change");

        var updatedAppointment = await context.Appointments.FirstAsync(a => a.Id == appointment.Id);
        updatedAppointment.Reason.Should().Be("Updated reason");
        updatedAppointment.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Handler_ShouldThrowException_WhenAppointmentNotFound()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<PatientManagement.Infrastructure.Data.ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new PatientManagement.Infrastructure.Data.ApplicationDbContext(options);
        var handler = new UpdateAppointmentCommandHandler(context);

        var command = new UpdateAppointmentCommand
        {
            Id = Guid.NewGuid(),
            ScheduledDateTime = DateTime.UtcNow.AddDays(2),
            Reason = "Updated reason"
        };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }
}

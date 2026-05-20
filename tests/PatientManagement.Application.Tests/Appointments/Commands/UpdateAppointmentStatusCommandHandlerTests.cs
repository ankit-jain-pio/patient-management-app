using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PatientManagement.Application.Appointments.Commands.UpdateAppointmentStatus;
using PatientManagement.Domain.Entities;
using PatientManagement.Domain.Enums;
using Xunit;

namespace PatientManagement.Application.Tests.Appointments.Commands;

public class UpdateAppointmentStatusCommandHandlerTests
{
    [Fact]
    public async Task Handler_ShouldUpdateStatus_WhenValidCommandProvided()
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
            ScheduledDateTime = DateTime.UtcNow.AddHours(1),
            Status = AppointmentStatus.Scheduled
        };
        
        context.Patients.Add(patient);
        context.Appointments.Add(appointment);
        await context.SaveChangesAsync();

        var handler = new UpdateAppointmentStatusCommandHandler(context);

        var command = new UpdateAppointmentStatusCommand
        {
            Id = appointment.Id,
            Status = AppointmentStatus.Completed
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(AppointmentStatus.Completed);
        result.CompletedTime.Should().NotBeNull();

        var updatedAppointment = await context.Appointments.FirstAsync(a => a.Id == appointment.Id);
        updatedAppointment.Status.Should().Be(AppointmentStatus.Completed);
    }

    [Fact]
    public async Task Handler_ShouldSetCheckInTime_WhenStatusChangedToInProgress()
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
            ScheduledDateTime = DateTime.UtcNow.AddHours(1),
            Status = AppointmentStatus.Scheduled
        };
        
        context.Patients.Add(patient);
        context.Appointments.Add(appointment);
        await context.SaveChangesAsync();

        var handler = new UpdateAppointmentStatusCommandHandler(context);

        var command = new UpdateAppointmentStatusCommand
        {
            Id = appointment.Id,
            Status = AppointmentStatus.InProgress
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.Should().Be(AppointmentStatus.InProgress);
        result.CheckInTime.Should().NotBeNull();
    }

    [Fact]
    public async Task Handler_ShouldThrowException_WhenAppointmentNotFound()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<PatientManagement.Infrastructure.Data.ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new PatientManagement.Infrastructure.Data.ApplicationDbContext(options);
        var handler = new UpdateAppointmentStatusCommandHandler(context);

        var command = new UpdateAppointmentStatusCommand
        {
            Id = Guid.NewGuid(),
            Status = AppointmentStatus.Completed
        };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }
}

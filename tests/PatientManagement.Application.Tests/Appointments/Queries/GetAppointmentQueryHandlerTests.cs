using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PatientManagement.Application.Appointments.Queries.GetAppointment;
using PatientManagement.Domain.Entities;
using PatientManagement.Domain.Enums;
using Xunit;

namespace PatientManagement.Application.Tests.Appointments.Queries;

public class GetAppointmentQueryHandlerTests
{
    [Fact]
    public async Task Handler_ShouldReturnAppointment_WhenAppointmentExists()
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
            Reason = "Annual Checkup",
            Status = AppointmentStatus.Scheduled
        };
        
        context.Patients.Add(patient);
        context.Appointments.Add(appointment);
        await context.SaveChangesAsync();

        var handler = new GetAppointmentQueryHandler(context);
        var query = new GetAppointmentQuery { Id = appointment.Id };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(appointment.Id);
        result.PatientId.Should().Be(patient.Id);
        result.Reason.Should().Be("Annual Checkup");
        result.Status.Should().Be(AppointmentStatus.Scheduled);
        result.Patient.Should().NotBeNull();
        result.Patient!.FullName.Should().Be("John Doe");
    }

    [Fact]
    public async Task Handler_ShouldReturnNull_WhenAppointmentDoesNotExist()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<PatientManagement.Infrastructure.Data.ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new PatientManagement.Infrastructure.Data.ApplicationDbContext(options);
        var handler = new GetAppointmentQueryHandler(context);
        var query = new GetAppointmentQuery { Id = Guid.NewGuid() };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}

using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PatientManagement.Application.Appointments.Queries.GetPatientAppointments;
using PatientManagement.Domain.Entities;
using PatientManagement.Domain.Enums;
using Xunit;

namespace PatientManagement.Application.Tests.Appointments.Queries;

public class GetPatientAppointmentsQueryHandlerTests
{
    [Fact]
    public async Task Handler_ShouldReturnAllAppointmentsForPatient_WhenAppointmentsExist()
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
        
        // Appointments for patient1
        context.Appointments.AddRange(
            new Appointment
            {
                PatientId = patient1.Id,
                ScheduledDateTime = DateTime.UtcNow.AddDays(-7),
                Status = AppointmentStatus.Completed,
                Reason = "Past appointment"
            },
            new Appointment
            {
                PatientId = patient1.Id,
                ScheduledDateTime = DateTime.UtcNow.AddDays(7),
                Status = AppointmentStatus.Scheduled,
                Reason = "Future appointment"
            },
            new Appointment
            {
                PatientId = patient1.Id,
                ScheduledDateTime = DateTime.UtcNow.AddDays(14),
                Status = AppointmentStatus.Scheduled,
                Reason = "Another future appointment"
            }
        );
        
        // Appointment for patient2 (should not be included)
        context.Appointments.Add(new Appointment
        {
            PatientId = patient2.Id,
            ScheduledDateTime = DateTime.UtcNow.AddDays(1),
            Status = AppointmentStatus.Scheduled,
            Reason = "Different patient"
        });
        
        await context.SaveChangesAsync();

        var handler = new GetPatientAppointmentsQueryHandler(context);
        var query = new GetPatientAppointmentsQuery { PatientId = patient1.Id };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().BeInDescendingOrder(a => a.ScheduledDateTime);
        result.Should().OnlyContain(a => a.PatientId == patient1.Id);
    }

    [Fact]
    public async Task Handler_ShouldReturnEmptyList_WhenPatientHasNoAppointments()
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

        var handler = new GetPatientAppointmentsQueryHandler(context);
        var query = new GetPatientAppointmentsQuery { PatientId = patient.Id };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}

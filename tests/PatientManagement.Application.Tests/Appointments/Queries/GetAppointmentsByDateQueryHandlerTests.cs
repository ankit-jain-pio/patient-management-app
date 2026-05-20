using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PatientManagement.Application.Appointments.Queries.GetAppointmentsByDate;
using PatientManagement.Domain.Entities;
using PatientManagement.Domain.Enums;
using Xunit;

namespace PatientManagement.Application.Tests.Appointments.Queries;

public class GetAppointmentsByDateQueryHandlerTests
{
    [Fact]
    public async Task Handler_ShouldReturnAppointmentsForSpecificDate_WhenAppointmentsExist()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<PatientManagement.Infrastructure.Data.ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new PatientManagement.Infrastructure.Data.ApplicationDbContext(options);
        
        var targetDate = new DateTime(2026, 5, 20, 0, 0, 0, DateTimeKind.Utc);
        
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
        
        // Appointments on target date
        context.Appointments.AddRange(
            new Appointment
            {
                PatientId = patient1.Id,
                ScheduledDateTime = targetDate.AddHours(9),
                Status = AppointmentStatus.Scheduled,
                Reason = "Morning appointment"
            },
            new Appointment
            {
                PatientId = patient2.Id,
                ScheduledDateTime = targetDate.AddHours(14),
                Status = AppointmentStatus.Scheduled,
                Reason = "Afternoon appointment"
            }
        );
        
        // Appointment on different date
        context.Appointments.Add(new Appointment
        {
            PatientId = patient1.Id,
            ScheduledDateTime = targetDate.AddDays(1),
            Status = AppointmentStatus.Scheduled,
            Reason = "Next day appointment"
        });
        
        await context.SaveChangesAsync();

        var handler = new GetAppointmentsByDateQueryHandler(context);
        var query = new GetAppointmentsByDateQuery { Date = targetDate };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().BeInAscendingOrder(a => a.ScheduledDateTime);
        result[0].Reason.Should().Be("Morning appointment");
        result[1].Reason.Should().Be("Afternoon appointment");
    }

    [Fact]
    public async Task Handler_ShouldReturnEmptyList_WhenNoAppointmentsExistForDate()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<PatientManagement.Infrastructure.Data.ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new PatientManagement.Infrastructure.Data.ApplicationDbContext(options);
        
        var handler = new GetAppointmentsByDateQueryHandler(context);
        var query = new GetAppointmentsByDateQuery { Date = new DateTime(2026, 5, 20) };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}

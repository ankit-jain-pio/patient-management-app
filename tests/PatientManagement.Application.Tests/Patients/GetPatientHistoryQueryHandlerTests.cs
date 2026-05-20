using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PatientManagement.Application.Patients.Queries.GetPatientHistory;
using PatientManagement.Domain.Entities;
using PatientManagement.Domain.Enums;
using PatientManagement.Domain.ValueObjects;
using PatientManagement.Infrastructure.Data;

namespace PatientManagement.Application.Tests.Patients;

public class GetPatientHistoryQueryHandlerTests
{
    private readonly ApplicationDbContext _context;
    private readonly GetPatientHistoryQueryHandler _handler;

    public GetPatientHistoryQueryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _handler = new GetPatientHistoryQueryHandler(_context);
    }

    [Fact]
    public async Task Handle_ShouldReturnPatientHistory_WithAllData()
    {
        // Arrange
        var patient = new Patient
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(1980, 5, 15),
            Gender = Gender.Male,
            PhoneNumber = "1234567890",
            Address = new Address("123 Main St", "City", "State", "12345")
        };

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        var consultation1 = new Consultation
        {
            PatientId = patient.Id,
            ConsultationDate = DateTime.Now.AddDays(-10),
            ChiefComplaint = "Fever",
            Diagnosis = "Viral Fever",
            Vitals = new Vitals(37.5m, "120/80", 80, 70.0m, 175.0m, 98m, "16")
        };

        var consultation2 = new Consultation
        {
            PatientId = patient.Id,
            ConsultationDate = DateTime.Now.AddDays(-5),
            ChiefComplaint = "Headache",
            Diagnosis = "Migraine",
            Vitals = new Vitals(36.8m, "115/75", 75, 70.0m, 175.0m, 99m, "14")
        };

        _context.Consultations.AddRange(consultation1, consultation2);
        await _context.SaveChangesAsync();

        var prescription1 = new Prescription
        {
            ConsultationId = consultation1.Id,
            MedicationName = "Paracetamol",
            Dosage = "500mg",
            Frequency = "Twice daily",
            DurationInDays = 5
        };

        var prescription2 = new Prescription
        {
            ConsultationId = consultation2.Id,
            MedicationName = "Ibuprofen",
            Dosage = "400mg",
            Frequency = "Once daily",
            DurationInDays = 3
        };

        _context.Prescriptions.AddRange(prescription1, prescription2);
        await _context.SaveChangesAsync();

        var appointment = new Appointment
        {
            PatientId = patient.Id,
            ScheduledDateTime = DateTime.Now.AddDays(-7),
            Status = AppointmentStatus.Completed,
            Reason = "Follow-up"
        };

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        var query = new GetPatientHistoryQuery
        {
            PatientId = patient.Id
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.PatientId.Should().Be(patient.Id);
        result.PatientName.Should().Be("John Doe");
        result.Age.Should().BeGreaterThan(0);
        result.Consultations.Should().HaveCount(2);
        result.Appointments.Should().HaveCount(1);
        result.TotalConsultations.Should().Be(2);
        result.TotalAppointments.Should().Be(1);

        // Verify consultations are ordered by date descending
        result.Consultations[0].ConsultationDate.Should().BeAfter(result.Consultations[1].ConsultationDate);

        // Verify prescriptions are included
        result.Consultations[0].Prescriptions.Should().HaveCount(1);
        result.Consultations[1].Prescriptions.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_WithDateRange_ShouldFilterConsultations()
    {
        // Arrange
        var patient = new Patient
        {
            FirstName = "Jane",
            LastName = "Smith",
            DateOfBirth = new DateTime(1990, 3, 20),
            Gender = Gender.Female,
            PhoneNumber = "9876543210",
            Address = new Address("456 Oak Ave", "City", "State", "54321")
        };

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        var oldConsultation = new Consultation
        {
            PatientId = patient.Id,
            ConsultationDate = DateTime.Now.AddDays(-30),
            ChiefComplaint = "Old issue",
            Diagnosis = "Old diagnosis"
        };

        var recentConsultation = new Consultation
        {
            PatientId = patient.Id,
            ConsultationDate = DateTime.Now.AddDays(-5),
            ChiefComplaint = "Recent issue",
            Diagnosis = "Recent diagnosis"
        };

        _context.Consultations.AddRange(oldConsultation, recentConsultation);
        await _context.SaveChangesAsync();

        var query = new GetPatientHistoryQuery
        {
            PatientId = patient.Id,
            StartDate = DateTime.Now.AddDays(-10),
            EndDate = DateTime.Now
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Consultations.Should().HaveCount(1);
        result.Consultations[0].ChiefComplaint.Should().Be("Recent issue");
        result.TotalConsultations.Should().Be(1);
    }

    [Fact]
    public async Task Handle_WithNonExistentPatient_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var query = new GetPatientHistoryQuery
        {
            PatientId = Guid.NewGuid()
        };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithNoConsultations_ShouldReturnEmptyList()
    {
        // Arrange
        var patient = new Patient
        {
            FirstName = "Bob",
            LastName = "Johnson",
            DateOfBirth = new DateTime(1985, 7, 10),
            Gender = Gender.Male,
            PhoneNumber = "5555555555",
            Address = new Address("789 Pine St", "City", "State", "67890")
        };

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        var query = new GetPatientHistoryQuery
        {
            PatientId = patient.Id
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Consultations.Should().BeEmpty();
        result.Appointments.Should().BeEmpty();
        result.TotalConsultations.Should().Be(0);
        result.TotalAppointments.Should().Be(0);
    }

    [Fact]
    public async Task Handle_WithOnlyStartDate_ShouldFilterFromStartDate()
    {
        // Arrange
        var patient = new Patient
        {
            FirstName = "Alice",
            LastName = "Williams",
            DateOfBirth = new DateTime(1975, 11, 25),
            Gender = Gender.Female,
            PhoneNumber = "1112223333",
            Address = new Address("321 Elm St", "City", "State", "98765")
        };

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        var beforeStart = new Consultation
        {
            PatientId = patient.Id,
            ConsultationDate = DateTime.Now.AddDays(-20),
            ChiefComplaint = "Before start",
            Diagnosis = "Before"
        };

        var afterStart = new Consultation
        {
            PatientId = patient.Id,
            ConsultationDate = DateTime.Now.AddDays(-5),
            ChiefComplaint = "After start",
            Diagnosis = "After"
        };

        _context.Consultations.AddRange(beforeStart, afterStart);
        await _context.SaveChangesAsync();

        var query = new GetPatientHistoryQuery
        {
            PatientId = patient.Id,
            StartDate = DateTime.Now.AddDays(-10)
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Consultations.Should().HaveCount(1);
        result.Consultations[0].ChiefComplaint.Should().Be("After start");
    }
}

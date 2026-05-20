using FluentAssertions;
using PatientManagement.Application.DTOs.Appointments;
using PatientManagement.Application.DTOs.Consultations;
using PatientManagement.Application.DTOs.Patients;
using PatientManagement.Domain.Enums;
using PatientManagement.Infrastructure.Services;
using System.Text;
using Xunit;

namespace PatientManagement.Infrastructure.Tests.Services;

public class CsvExportServiceTests
{
    private readonly CsvExportService _service;

    public CsvExportServiceTests()
    {
        _service = new CsvExportService();
    }

    [Fact]
    public void ExportPatients_ShouldGenerateCsvWithAllFields()
    {
        // Arrange
        var patients = new List<PatientDto>
        {
            new PatientDto
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateTime(1980, 5, 15),
                Age = 43,
                Gender = Gender.Male,
                PhoneNumber = "1234567890",
                Email = "john.doe@example.com",
                Address = new AddressDto
                {
                    Street = "123 Main St",
                    City = "Springfield",
                    State = "IL",
                    PostalCode = "62701"
                },
                BloodGroup = "O+",
                Allergies = "None",
                MedicalHistory = "None",
                EmergencyContactName = "Jane Doe",
                EmergencyContactPhone = "9876543210",
                CreatedAt = DateTime.Now
            }
        };

        // Act
        var result = _service.ExportPatients(patients);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();

        var csvContent = Encoding.UTF8.GetString(result);
        csvContent.Should().Contain("John");
        csvContent.Should().Contain("Doe");
        csvContent.Should().Contain("1234567890");
        csvContent.Should().Contain("john.doe@example.com");
    }

    [Fact]
    public void ExportPatients_WithEmptyList_ShouldGenerateHeaderOnly()
    {
        // Arrange
        var patients = new List<PatientDto>();

        // Act
        var result = _service.ExportPatients(patients);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();

        var csvContent = Encoding.UTF8.GetString(result);
        csvContent.Should().Contain("Id");
        csvContent.Should().Contain("FirstName");
        csvContent.Should().Contain("LastName");
    }

    [Fact]
    public void ExportConsultations_ShouldGenerateCsvWithAllFields()
    {
        // Arrange
        var consultations = new List<ConsultationDto>
        {
            new ConsultationDto
            {
                Id = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                Patient = new PatientSummaryDto
                {
                    Id = Guid.NewGuid(),
                    FullName = "John Doe",
                    Age = 43,
                    PhoneNumber = "1234567890"
                },
                ConsultationDate = DateTime.Now,
                ChiefComplaint = "Fever",
                Symptoms = "High temperature, body ache",
                Diagnosis = "Viral Fever",
                TreatmentPlan = "Rest and medication",
                Vitals = new VitalsDto
                {
                    Temperature = 38.5m,
                    BloodPressure = "120/80",
                    PulseRate = 85,
                    Weight = 70.0m,
                    Height = 175.0m,
                    OxygenSaturation = 98m,
                    RespiratoryRate = "16",
                    BMI = 22.86m
                },
                Prescriptions = new List<PrescriptionDto>
                {
                    new PrescriptionDto
                    {
                        Id = Guid.NewGuid(),
                        MedicationName = "Paracetamol",
                        Dosage = "500mg",
                        Frequency = "Twice daily",
                        DurationInDays = 5
                    }
                },
                CreatedAt = DateTime.Now
            }
        };

        // Act
        var result = _service.ExportConsultations(consultations);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();

        var csvContent = Encoding.UTF8.GetString(result);
        csvContent.Should().Contain("John Doe");
        csvContent.Should().Contain("Fever");
        csvContent.Should().Contain("Viral Fever");
        csvContent.Should().Contain("38.5");
        csvContent.Should().Contain("120/80");
    }

    [Fact]
    public void ExportAppointments_ShouldGenerateCsvWithAllFields()
    {
        // Arrange
        var appointments = new List<AppointmentDto>
        {
            new AppointmentDto
            {
                Id = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                Patient = new PatientSummaryDto
                {
                    Id = Guid.NewGuid(),
                    FullName = "Jane Smith",
                    Age = 35,
                    PhoneNumber = "9876543210"
                },
                ScheduledDateTime = DateTime.Now.AddDays(1),
                Status = AppointmentStatus.Scheduled,
                Reason = "Follow-up checkup",
                Notes = "Annual checkup",
                CreatedAt = DateTime.Now
            }
        };

        // Act
        var result = _service.ExportAppointments(appointments);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();

        var csvContent = Encoding.UTF8.GetString(result);
        csvContent.Should().Contain("Jane Smith");
        csvContent.Should().Contain("9876543210");
        csvContent.Should().Contain("Scheduled");
        csvContent.Should().Contain("Follow-up checkup");
    }

    [Fact]
    public void ExportConsultations_WithNullVitals_ShouldHandleGracefully()
    {
        // Arrange
        var consultations = new List<ConsultationDto>
        {
            new ConsultationDto
            {
                Id = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                Patient = new PatientSummaryDto
                {
                    Id = Guid.NewGuid(),
                    FullName = "Bob Johnson",
                    Age = 50,
                    PhoneNumber = "5555555555"
                },
                ConsultationDate = DateTime.Now,
                ChiefComplaint = "Checkup",
                Diagnosis = "Healthy",
                Vitals = null,
                CreatedAt = DateTime.Now
            }
        };

        // Act
        var result = _service.ExportConsultations(consultations);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();

        var csvContent = Encoding.UTF8.GetString(result);
        csvContent.Should().Contain("Bob Johnson");
        csvContent.Should().Contain("Healthy");
    }

    [Fact]
    public void ExportAppointments_WithMultipleStatuses_ShouldIncludeAll()
    {
        // Arrange
        var appointments = new List<AppointmentDto>
        {
            new AppointmentDto
            {
                Id = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                Patient = new PatientSummaryDto
                {
                    Id = Guid.NewGuid(),
                    FullName = "Alice Williams",
                    Age = 28,
                    PhoneNumber = "1112223333"
                },
                ScheduledDateTime = DateTime.Now,
                Status = AppointmentStatus.Scheduled,
                Reason = "First visit",
                CreatedAt = DateTime.Now
            },
            new AppointmentDto
            {
                Id = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                Patient = new PatientSummaryDto
                {
                    Id = Guid.NewGuid(),
                    FullName = "Charlie Brown",
                    Age = 45,
                    PhoneNumber = "4445556666"
                },
                ScheduledDateTime = DateTime.Now.AddDays(-1),
                Status = AppointmentStatus.Completed,
                Reason = "Follow-up",
                CheckInTime = DateTime.Now.AddDays(-1).AddHours(-1),
                CompletedTime = DateTime.Now.AddDays(-1),
                CreatedAt = DateTime.Now.AddDays(-2)
            }
        };

        // Act
        var result = _service.ExportAppointments(appointments);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();

        var csvContent = Encoding.UTF8.GetString(result);
        csvContent.Should().Contain("Alice Williams");
        csvContent.Should().Contain("Charlie Brown");
        csvContent.Should().Contain("Scheduled");
        csvContent.Should().Contain("Completed");
    }
}

using FluentAssertions;
using Microsoft.Extensions.Configuration;
using PatientManagement.Application.DTOs.Appointments;
using PatientManagement.Application.DTOs.Consultations;
using PatientManagement.Domain.Enums;
using PatientManagement.Infrastructure.Services;
using Xunit;

namespace PatientManagement.Infrastructure.Tests.Services;

public class PdfExportServiceTests
{
    private readonly PdfExportService _service;

    public PdfExportServiceTests()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ClinicSettings:Name", "Test Clinic" },
                { "ClinicSettings:Address", "123 Test St" },
                { "ClinicSettings:Phone", "+1-234-567-8900" },
                { "ClinicSettings:Email", "test@clinic.com" }
            })
            .Build();

        _service = new PdfExportService(configuration);
    }

    [Fact]
    public void GeneratePrescriptionPdf_ShouldGenerateValidPdf()
    {
        // Arrange
        var consultation = new ConsultationDto
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
            Diagnosis = "Viral Fever",
            Prescriptions = new List<PrescriptionDto>
            {
                new PrescriptionDto
                {
                    Id = Guid.NewGuid(),
                    MedicationName = "Paracetamol",
                    Dosage = "500mg",
                    Frequency = "Twice daily",
                    DurationInDays = 5,
                    Instructions = "Take after meals"
                },
                new PrescriptionDto
                {
                    Id = Guid.NewGuid(),
                    MedicationName = "Vitamin C",
                    Dosage = "1000mg",
                    Frequency = "Once daily",
                    DurationInDays = 7,
                    Instructions = "Take in the morning"
                }
            },
            FollowUpInstructions = "Rest well and drink plenty of fluids",
            NextVisitDate = DateTime.Now.AddDays(7),
            CreatedAt = DateTime.Now
        };

        // Act
        var result = _service.GeneratePrescriptionPdf(consultation);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        
        // PDF should start with PDF header
        var pdfHeader = System.Text.Encoding.ASCII.GetString(result.Take(5).ToArray());
        pdfHeader.Should().Be("%PDF-");
    }

    [Fact]
    public void GeneratePrescriptionPdf_WithNoPrescriptions_ShouldStillGeneratePdf()
    {
        // Arrange
        var consultation = new ConsultationDto
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
            ConsultationDate = DateTime.Now,
            Diagnosis = "Regular checkup",
            Prescriptions = new List<PrescriptionDto>(),
            CreatedAt = DateTime.Now
        };

        // Act
        var result = _service.GeneratePrescriptionPdf(consultation);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        
        var pdfHeader = System.Text.Encoding.ASCII.GetString(result.Take(5).ToArray());
        pdfHeader.Should().Be("%PDF-");
    }

    [Fact]
    public void GenerateConsultationSummaryPdf_ShouldGenerateValidPdf()
    {
        // Arrange
        var consultation = new ConsultationDto
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
            ChiefComplaint = "Persistent cough",
            Symptoms = "Dry cough for 3 days, mild fever",
            Diagnosis = "Upper Respiratory Tract Infection",
            TreatmentPlan = "Antibiotics and rest",
            ClinicalNotes = "Patient appears stable, no complications",
            Vitals = new VitalsDto
            {
                Temperature = 38.2m,
                BloodPressure = "125/85",
                PulseRate = 82,
                Weight = 75.0m,
                Height = 180.0m,
                OxygenSaturation = 97m,
                RespiratoryRate = "18",
                BMI = 23.15m
            },
            Prescriptions = new List<PrescriptionDto>
            {
                new PrescriptionDto
                {
                    Id = Guid.NewGuid(),
                    MedicationName = "Amoxicillin",
                    Dosage = "500mg",
                    Frequency = "Three times daily",
                    DurationInDays = 7
                }
            },
            FollowUpInstructions = "Return if symptoms persist after 3 days",
            NextVisitDate = DateTime.Now.AddDays(3),
            CreatedAt = DateTime.Now
        };

        // Act
        var result = _service.GenerateConsultationSummaryPdf(consultation);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        
        var pdfHeader = System.Text.Encoding.ASCII.GetString(result.Take(5).ToArray());
        pdfHeader.Should().Be("%PDF-");
    }

    [Fact]
    public void GenerateConsultationSummaryPdf_WithMinimalData_ShouldGeneratePdf()
    {
        // Arrange
        var consultation = new ConsultationDto
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
            ConsultationDate = DateTime.Now,
            ChiefComplaint = "Routine checkup",
            Diagnosis = "Healthy",
            CreatedAt = DateTime.Now
        };

        // Act
        var result = _service.GenerateConsultationSummaryPdf(consultation);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        
        var pdfHeader = System.Text.Encoding.ASCII.GetString(result.Take(5).ToArray());
        pdfHeader.Should().Be("%PDF-");
    }

    [Fact]
    public void GeneratePrescriptionPdf_WithLongInstructions_ShouldHandleGracefully()
    {
        // Arrange
        var consultation = new ConsultationDto
        {
            Id = Guid.NewGuid(),
            PatientId = Guid.NewGuid(),
            Patient = new PatientSummaryDto
            {
                Id = Guid.NewGuid(),
                FullName = "Charlie Brown",
                Age = 62,
                PhoneNumber = "4445556666"
            },
            ConsultationDate = DateTime.Now,
            Diagnosis = "Chronic condition management",
            Prescriptions = new List<PrescriptionDto>
            {
                new PrescriptionDto
                {
                    Id = Guid.NewGuid(),
                    MedicationName = "Complex Medication",
                    Dosage = "Variable",
                    Frequency = "As directed",
                    DurationInDays = 30,
                    Instructions = "Take this medication with food. Avoid alcohol. " +
                                 "Do not drive or operate machinery. Monitor blood pressure daily. " +
                                 "Report any side effects immediately to your doctor. " +
                                 "Store in a cool, dry place away from direct sunlight."
                }
            },
            FollowUpInstructions = "Regular monitoring required. Follow strict dietary restrictions. " +
                                 "Maintain a health diary. Schedule monthly check-ups.",
            NextVisitDate = DateTime.Now.AddDays(30),
            CreatedAt = DateTime.Now
        };

        // Act
        var result = _service.GeneratePrescriptionPdf(consultation);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        
        var pdfHeader = System.Text.Encoding.ASCII.GetString(result.Take(5).ToArray());
        pdfHeader.Should().Be("%PDF-");
    }

    [Fact]
    public void GenerateConsultationSummaryPdf_WithNullVitals_ShouldGeneratePdf()
    {
        // Arrange
        var consultation = new ConsultationDto
        {
            Id = Guid.NewGuid(),
            PatientId = Guid.NewGuid(),
            Patient = new PatientSummaryDto
            {
                Id = Guid.NewGuid(),
                FullName = "David Lee",
                Age = 40,
                PhoneNumber = "7778889999"
            },
            ConsultationDate = DateTime.Now,
            ChiefComplaint = "Consultation only",
            Diagnosis = "No physical examination",
            Vitals = null,
            CreatedAt = DateTime.Now
        };

        // Act
        var result = _service.GenerateConsultationSummaryPdf(consultation);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        
        var pdfHeader = System.Text.Encoding.ASCII.GetString(result.Take(5).ToArray());
        pdfHeader.Should().Be("%PDF-");
    }
}

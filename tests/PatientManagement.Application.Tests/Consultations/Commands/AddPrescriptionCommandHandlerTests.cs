using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PatientManagement.Application.Consultations.Commands.AddPrescription;
using PatientManagement.Domain.Entities;
using PatientManagement.Domain.Enums;
using Xunit;

namespace PatientManagement.Application.Tests.Consultations.Commands;

public class AddPrescriptionCommandHandlerTests
{
    [Fact]
    public async Task Handler_ShouldAddPrescription_WhenValidCommandProvided()
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
        
        var consultation = new Consultation
        {
            PatientId = patient.Id,
            ChiefComplaint = "Test",
            ConsultationDate = DateTime.UtcNow
        };
        
        context.Patients.Add(patient);
        context.Consultations.Add(consultation);
        await context.SaveChangesAsync();

        var handler = new AddPrescriptionCommandHandler(context);

        var command = new AddPrescriptionCommand
        {
            ConsultationId = consultation.Id,
            MedicationName = "Paracetamol",
            Dosage = "500mg",
            Frequency = "Twice daily",
            DurationInDays = 5,
            Instructions = "Take after meals"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.ConsultationId.Should().Be(consultation.Id);
        result.MedicationName.Should().Be("Paracetamol");
        result.Dosage.Should().Be("500mg");
        result.Frequency.Should().Be("Twice daily");
        result.DurationInDays.Should().Be(5);

        var savedPrescription = await context.Prescriptions.FirstOrDefaultAsync();
        savedPrescription.Should().NotBeNull();
        savedPrescription!.ConsultationId.Should().Be(consultation.Id);
    }

    [Fact]
    public async Task Handler_ShouldThrowException_WhenConsultationNotFound()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<PatientManagement.Infrastructure.Data.ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new PatientManagement.Infrastructure.Data.ApplicationDbContext(options);
        var handler = new AddPrescriptionCommandHandler(context);

        var command = new AddPrescriptionCommand
        {
            ConsultationId = Guid.NewGuid(),
            MedicationName = "Test",
            Dosage = "100mg",
            Frequency = "Once daily",
            DurationInDays = 7
        };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }
}

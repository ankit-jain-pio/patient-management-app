using MediatR;
using Microsoft.EntityFrameworkCore;
using PatientManagement.Application.Common.Interfaces;
using PatientManagement.Application.DTOs.Consultations;
using PatientManagement.Domain.Entities;

namespace PatientManagement.Application.Consultations.Commands.AddPrescription;

public class AddPrescriptionCommandHandler : IRequestHandler<AddPrescriptionCommand, PrescriptionDto>
{
    private readonly IApplicationDbContext _context;

    public AddPrescriptionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PrescriptionDto> Handle(AddPrescriptionCommand request, CancellationToken cancellationToken)
    {
        // Verify consultation exists
        var consultationExists = await _context.Consultations
            .AnyAsync(c => c.Id == request.ConsultationId, cancellationToken);

        if (!consultationExists)
        {
            throw new KeyNotFoundException($"Consultation with ID {request.ConsultationId} not found");
        }

        // Create prescription
        var prescription = new Prescription
        {
            ConsultationId = request.ConsultationId,
            MedicationName = request.MedicationName,
            Dosage = request.Dosage,
            Frequency = request.Frequency,
            DurationInDays = request.DurationInDays,
            Instructions = request.Instructions,
            PrescribedDate = DateTime.UtcNow
        };

        _context.Prescriptions.Add(prescription);
        await _context.SaveChangesAsync(cancellationToken);

        return new PrescriptionDto
        {
            Id = prescription.Id,
            ConsultationId = prescription.ConsultationId,
            MedicationName = prescription.MedicationName,
            Dosage = prescription.Dosage,
            Frequency = prescription.Frequency,
            DurationInDays = prescription.DurationInDays,
            Instructions = prescription.Instructions,
            PrescribedDate = prescription.PrescribedDate
        };
    }
}

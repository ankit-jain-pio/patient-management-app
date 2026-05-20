using MediatR;
using Microsoft.EntityFrameworkCore;
using PatientManagement.Application.Common.Interfaces;
using PatientManagement.Application.DTOs.Patients;
using PatientManagement.Domain.Enums;
using PatientManagement.Domain.ValueObjects;

namespace PatientManagement.Application.Patients.Commands.UpdatePatient;

public class UpdatePatientCommandHandler : IRequestHandler<UpdatePatientCommand, PatientDto>
{
    private readonly IApplicationDbContext _context;

    public UpdatePatientCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PatientDto> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
    {
        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (patient == null)
        {
            throw new KeyNotFoundException($"Patient with ID {request.Id} not found");
        }

        // Update address value object if provided
        Address? address = null;
        if (request.Address != null)
        {
            address = new Address(
                request.Address.Street,
                request.Address.City,
                request.Address.State,
                request.Address.PostalCode,
                request.Address.Country
            );
        }

        // Update patient properties
        patient.FirstName = request.FirstName;
        patient.LastName = request.LastName;
        patient.DateOfBirth = request.DateOfBirth;
        patient.Gender = (Gender)request.Gender;
        patient.PhoneNumber = request.PhoneNumber;
        patient.Email = request.Email;
        patient.Address = address;
        patient.EmergencyContactName = request.EmergencyContactName;
        patient.EmergencyContactPhone = request.EmergencyContactPhone;
        patient.BloodGroup = request.BloodGroup;
        patient.Allergies = request.Allergies;
        patient.MedicalHistory = request.MedicalHistory;

        await _context.SaveChangesAsync(cancellationToken);

        // Map to DTO
        return new PatientDto
        {
            Id = patient.Id,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            FullName = patient.FullName,
            DateOfBirth = patient.DateOfBirth,
            Age = patient.Age,
            Gender = patient.Gender,
            PhoneNumber = patient.PhoneNumber,
            Email = patient.Email,
            Address = patient.Address != null ? new AddressDto
            {
                Street = patient.Address.Street,
                City = patient.Address.City,
                State = patient.Address.State,
                PostalCode = patient.Address.PostalCode,
                Country = patient.Address.Country
            } : null,
            EmergencyContactName = patient.EmergencyContactName,
            EmergencyContactPhone = patient.EmergencyContactPhone,
            BloodGroup = patient.BloodGroup,
            Allergies = patient.Allergies,
            MedicalHistory = patient.MedicalHistory,
            CreatedAt = patient.CreatedAt,
            UpdatedAt = patient.UpdatedAt
        };
    }
}

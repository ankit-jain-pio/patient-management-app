using MediatR;
using Microsoft.EntityFrameworkCore;
using PatientManagement.Application.Common.Interfaces;
using PatientManagement.Application.DTOs.Patients;

namespace PatientManagement.Application.Patients.Queries.GetAllPatients;

public class GetAllPatientsQuery : IRequest<List<PatientDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

public class GetAllPatientsQueryHandler : IRequestHandler<GetAllPatientsQuery, List<PatientDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllPatientsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PatientDto>> Handle(GetAllPatientsQuery request, CancellationToken cancellationToken)
    {
        var patients = await _context.Patients
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return patients.Select(patient => new PatientDto
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
        }).ToList();
    }
}

using MediatR;
using Microsoft.EntityFrameworkCore;
using PatientManagement.Application.Common.Interfaces;
using PatientManagement.Application.DTOs.Patients;

namespace PatientManagement.Application.Patients.Queries.SearchPatients;

public class SearchPatientsQuery : IRequest<List<PatientSearchResultDto>>
{
    public string? SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class SearchPatientsQueryHandler : IRequestHandler<SearchPatientsQuery, List<PatientSearchResultDto>>
{
    private readonly IApplicationDbContext _context;

    public SearchPatientsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PatientSearchResultDto>> Handle(SearchPatientsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Patients.AsNoTracking();

        // Apply search filter if search term is provided
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower().Trim();
            query = query.Where(p =>
                p.FirstName.ToLower().Contains(searchTerm) ||
                p.LastName.ToLower().Contains(searchTerm) ||
                p.PhoneNumber.Contains(searchTerm) ||
                (p.Email != null && p.Email.ToLower().Contains(searchTerm))
            );
        }

        var patients = await query
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new
            {
                p.Id,
                p.FirstName,
                p.LastName,
                p.Age,
                p.Gender,
                p.PhoneNumber,
                p.CreatedAt,
                ConsultationCount = p.Consultations.Count,
                LastConsultationDate = p.Consultations
                    .OrderByDescending(c => c.ConsultationDate)
                    .Select(c => c.ConsultationDate)
                    .FirstOrDefault()
            })
            .ToListAsync(cancellationToken);

        return patients.Select(p => new PatientSearchResultDto
        {
            Id = p.Id,
            FirstName = p.FirstName,
            LastName = p.LastName,
            FullName = $"{p.FirstName} {p.LastName}",
            Age = p.Age,
            Gender = p.Gender,
            PhoneNumber = p.PhoneNumber,
            LastVisit = p.LastConsultationDate != default ? p.LastConsultationDate : p.CreatedAt,
            TotalVisits = p.ConsultationCount
        }).ToList();
    }
}

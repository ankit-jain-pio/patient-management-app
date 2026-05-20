using FluentValidation;

namespace PatientManagement.Application.Patients.Queries.SearchPatients;

public class SearchPatientsQueryValidator : AbstractValidator<SearchPatientsQuery>
{
    public SearchPatientsQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Page size must not exceed 100");

        RuleFor(x => x.SearchTerm)
            .MinimumLength(2).WithMessage("Search term must be at least 2 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.SearchTerm));
    }
}

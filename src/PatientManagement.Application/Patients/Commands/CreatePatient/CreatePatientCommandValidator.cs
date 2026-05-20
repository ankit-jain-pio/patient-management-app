using FluentValidation;

namespace PatientManagement.Application.Patients.Commands.CreatePatient;

public class CreatePatientCommandValidator : AbstractValidator<CreatePatientCommand>
{
    public CreatePatientCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(100).WithMessage("First name must not exceed 100 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(100).WithMessage("Last name must not exceed 100 characters");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required")
            .LessThan(DateTime.UtcNow).WithMessage("Date of birth must be in the past")
            .GreaterThan(DateTime.UtcNow.AddYears(-150)).WithMessage("Date of birth is not valid");

        RuleFor(x => x.Gender)
            .Must(g => Enum.IsDefined(typeof(Domain.Enums.Gender), g))
            .WithMessage("Invalid gender value");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required")
            .MaximumLength(20).WithMessage("Phone number must not exceed 20 characters")
            .Matches(@"^[\d\s\-\+\(\)]+$").WithMessage("Phone number contains invalid characters");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Invalid email format")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.BloodGroup)
            .MaximumLength(10).WithMessage("Blood group must not exceed 10 characters")
            .When(x => !string.IsNullOrEmpty(x.BloodGroup));

        RuleFor(x => x.EmergencyContactPhone)
            .MaximumLength(20).WithMessage("Emergency contact phone must not exceed 20 characters")
            .Matches(@"^[\d\s\-\+\(\)]+$").WithMessage("Emergency contact phone contains invalid characters")
            .When(x => !string.IsNullOrEmpty(x.EmergencyContactPhone));

        // Address validation
        When(x => x.Address != null, () =>
        {
            RuleFor(x => x.Address!.Street)
                .NotEmpty().WithMessage("Street is required when address is provided");

            RuleFor(x => x.Address!.City)
                .NotEmpty().WithMessage("City is required when address is provided");

            RuleFor(x => x.Address!.State)
                .NotEmpty().WithMessage("State is required when address is provided");

            RuleFor(x => x.Address!.PostalCode)
                .NotEmpty().WithMessage("Postal code is required when address is provided");
        });
    }
}

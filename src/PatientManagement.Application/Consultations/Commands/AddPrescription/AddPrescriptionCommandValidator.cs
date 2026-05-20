using FluentValidation;

namespace PatientManagement.Application.Consultations.Commands.AddPrescription;

public class AddPrescriptionCommandValidator : AbstractValidator<AddPrescriptionCommand>
{
    public AddPrescriptionCommandValidator()
    {
        RuleFor(x => x.ConsultationId)
            .NotEmpty().WithMessage("Consultation ID is required");

        RuleFor(x => x.MedicationName)
            .NotEmpty().WithMessage("Medication name is required")
            .MaximumLength(200).WithMessage("Medication name must not exceed 200 characters");

        RuleFor(x => x.Dosage)
            .NotEmpty().WithMessage("Dosage is required")
            .MaximumLength(100).WithMessage("Dosage must not exceed 100 characters");

        RuleFor(x => x.Frequency)
            .NotEmpty().WithMessage("Frequency is required")
            .MaximumLength(100).WithMessage("Frequency must not exceed 100 characters");

        RuleFor(x => x.DurationInDays)
            .GreaterThan(0).WithMessage("Duration must be greater than 0 days")
            .LessThanOrEqualTo(365).WithMessage("Duration must not exceed 365 days");

        RuleFor(x => x.Instructions)
            .MaximumLength(500).WithMessage("Instructions must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Instructions));
    }
}

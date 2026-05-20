using FluentValidation;

namespace PatientManagement.Application.Consultations.Commands.CreateConsultation;

public class CreateConsultationCommandValidator : AbstractValidator<CreateConsultationCommand>
{
    public CreateConsultationCommandValidator()
    {
        RuleFor(x => x.PatientId)
            .NotEmpty().WithMessage("Patient ID is required");

        // Vitals validation if provided
        When(x => x.Vitals != null, () =>
        {
            RuleFor(x => x.Vitals!.Temperature)
                .Must(temp => !temp.HasValue || (temp >= 35 && temp <= 42))
                .WithMessage("Temperature must be between 35°C and 42°C");

            RuleFor(x => x.Vitals!.PulseRate)
                .Must(pulse => !pulse.HasValue || (pulse >= 30 && pulse <= 200))
                .WithMessage("Pulse rate must be between 30 and 200 bpm");

            RuleFor(x => x.Vitals!.OxygenSaturation)
                .Must(o2 => !o2.HasValue || (o2 >= 0 && o2 <= 100))
                .WithMessage("Oxygen saturation must be between 0 and 100%");

            RuleFor(x => x.Vitals!.Weight)
                .Must(weight => !weight.HasValue || weight > 0)
                .WithMessage("Weight must be greater than 0");

            RuleFor(x => x.Vitals!.Height)
                .Must(height => !height.HasValue || height > 0)
                .WithMessage("Height must be greater than 0");
        });

        RuleFor(x => x.ChiefComplaint)
            .MaximumLength(1000).WithMessage("Chief complaint must not exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.ChiefComplaint));

        RuleFor(x => x.Symptoms)
            .MaximumLength(2000).WithMessage("Symptoms must not exceed 2000 characters")
            .When(x => !string.IsNullOrEmpty(x.Symptoms));
    }
}

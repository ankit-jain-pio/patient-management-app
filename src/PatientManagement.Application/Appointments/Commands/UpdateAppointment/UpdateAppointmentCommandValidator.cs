using FluentValidation;

namespace PatientManagement.Application.Appointments.Commands.UpdateAppointment;

public class UpdateAppointmentCommandValidator : AbstractValidator<UpdateAppointmentCommand>
{
    public UpdateAppointmentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Appointment ID is required");

        RuleFor(x => x.ScheduledDateTime)
            .NotEmpty().WithMessage("Scheduled date and time is required")
            .GreaterThan(DateTime.UtcNow.AddMinutes(-5))
            .WithMessage("Appointment must be scheduled for current time or future");

        RuleFor(x => x.Reason)
            .MaximumLength(500).WithMessage("Reason must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Reason));

        RuleFor(x => x.Notes)
            .MaximumLength(2000).WithMessage("Notes must not exceed 2000 characters")
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}

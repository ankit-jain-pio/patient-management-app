using FluentValidation;

namespace PatientManagement.Application.Appointments.Commands.UpdateAppointmentStatus;

public class UpdateAppointmentStatusCommandValidator : AbstractValidator<UpdateAppointmentStatusCommand>
{
    public UpdateAppointmentStatusCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Appointment ID is required");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid appointment status value");
    }
}

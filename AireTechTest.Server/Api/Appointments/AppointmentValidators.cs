using AireTechTest.Server.Domain;

using FluentValidation;

using Vogen;

namespace AireTechTest.Server.Api.Appointments;

public sealed class CreateAppointmentRequestValidator : AbstractValidator<CreateAppointmentRequest>
{
    public CreateAppointmentRequestValidator()
    {
        RuleFor(x => x.PatientNhsNumber)
            .NotEmpty()
            .WithMessage("Patient NHS Number is required")
            .Must(BeValidNhsNumber)
            .WithMessage("Patient NHS Number is invalid");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Status is invalid");

        RuleFor(x => x.Time)
            .NotEmpty()
            .WithMessage("Time is required");

        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0)
            .WithMessage("Duration must be greater than 0 minutes")
            .LessThanOrEqualTo(480)
            .WithMessage("Duration must not exceed 8 hours");

        RuleFor(x => x.Clinician)
            .NotEmpty()
            .WithMessage("Clinician is required")
            .MaximumLength(200)
            .WithMessage("Clinician must not exceed 200 characters");

        RuleFor(x => x.Department)
            .IsInEnum()
            .WithMessage("Department is invalid");

        RuleFor(x => x.Location)
            .NotEmpty()
            .WithMessage("Location is required")
            .Must(BeValidPostcode)
            .WithMessage("Location postcode is invalid");
    }

    private static bool BeValidNhsNumber(string? nhsNumber)
    {
        if (string.IsNullOrWhiteSpace(nhsNumber))
        {
            return false;
        }

        try
        {
            NhsNumber.From(nhsNumber);
            return true;
        }
        catch (ValueObjectValidationException)
        {
            return false;
        }
    }

    private static bool BeValidPostcode(string? postcode)
    {
        if (string.IsNullOrWhiteSpace(postcode))
        {
            return false;
        }

        try
        {
            Postcode.From(postcode);
            return true;
        }
        catch (ValueObjectValidationException)
        {
            return false;
        }
    }
}

public sealed class UpdateAppointmentRequestValidator : AbstractValidator<UpdateAppointmentRequest>
{
    public UpdateAppointmentRequestValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Status is invalid");

        RuleFor(x => x.Time)
            .NotEmpty()
            .WithMessage("Time is required");

        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0)
            .WithMessage("Duration must be greater than 0 minutes")
            .LessThanOrEqualTo(480)
            .WithMessage("Duration must not exceed 8 hours");

        RuleFor(x => x.Clinician)
            .NotEmpty()
            .WithMessage("Clinician is required")
            .MaximumLength(200)
            .WithMessage("Clinician must not exceed 200 characters");

        RuleFor(x => x.Department)
            .IsInEnum()
            .WithMessage("Department is invalid");

        RuleFor(x => x.Location)
            .NotEmpty()
            .WithMessage("Location is required")
            .Must(BeValidPostcode)
            .WithMessage("Location postcode is invalid");
    }

    private static bool BeValidPostcode(string? postcode)
    {
        if (string.IsNullOrWhiteSpace(postcode))
        {
            return false;
        }

        try
        {
            Postcode.From(postcode);
            return true;
        }
        catch (ValueObjectValidationException)
        {
            return false;
        }
    }
}
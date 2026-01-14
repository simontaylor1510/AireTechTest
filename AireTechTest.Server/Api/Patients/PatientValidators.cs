using AireTechTest.Server.Domain;
using FluentValidation;
using Vogen;

namespace AireTechTest.Server.Api.Patients;

public sealed class CreatePatientRequestValidator : AbstractValidator<CreatePatientRequest>
{
    public CreatePatientRequestValidator()
    {
        RuleFor(x => x.NhsNumber)
            .NotEmpty()
            .WithMessage("NHS Number is required")
            .Must(BeValidNhsNumber)
            .WithMessage("NHS Number is invalid");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(200)
            .WithMessage("Name must not exceed 200 characters");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty()
            .WithMessage("Date of birth is required")
            .Must(NotBeInFuture)
            .WithMessage("Date of birth cannot be in the future")
            .Must(NotBeUnreasonablyOld)
            .WithMessage("Date of birth is invalid");

        RuleFor(x => x.Postcode)
            .NotEmpty()
            .WithMessage("Postcode is required")
            .Must(BeValidPostcode)
            .WithMessage("Postcode is invalid");
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

    private static bool NotBeInFuture(DateOnly dateOfBirth)
    {
        return dateOfBirth <= DateOnly.FromDateTime(DateTime.Today);
    }

    private static bool NotBeUnreasonablyOld(DateOnly dateOfBirth)
    {
        // Oldest verified person was 122 years old
        DateOnly minDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-130));
        return dateOfBirth >= minDate;
    }
}

public sealed class UpdatePatientRequestValidator : AbstractValidator<UpdatePatientRequest>
{
    public UpdatePatientRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(200)
            .WithMessage("Name must not exceed 200 characters");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty()
            .WithMessage("Date of birth is required")
            .Must(NotBeInFuture)
            .WithMessage("Date of birth cannot be in the future")
            .Must(NotBeUnreasonablyOld)
            .WithMessage("Date of birth is invalid");

        RuleFor(x => x.Postcode)
            .NotEmpty()
            .WithMessage("Postcode is required")
            .Must(BeValidPostcode)
            .WithMessage("Postcode is invalid");
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

    private static bool NotBeInFuture(DateOnly dateOfBirth)
    {
        return dateOfBirth <= DateOnly.FromDateTime(DateTime.Today);
    }

    private static bool NotBeUnreasonablyOld(DateOnly dateOfBirth)
    {
        DateOnly minDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-130));
        return dateOfBirth >= minDate;
    }
}
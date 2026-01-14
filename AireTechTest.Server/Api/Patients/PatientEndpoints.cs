using AireTechTest.Server.Data;
using AireTechTest.Server.Domain;

using FluentValidation;
using FluentValidation.Results;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AireTechTest.Server.Api.Patients;

public static class PatientEndpoints
{
    public static RouteGroupBuilder MapPatientEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("", GetAllPatients)
            .WithName("GetAllPatients")
            .WithSummary("Get all patients");

        group.MapGet("{nhsNumber}", GetPatient)
            .WithName("GetPatient")
            .WithSummary("Get a patient by NHS number");

        group.MapPost("", CreatePatient)
            .WithName("CreatePatient")
            .WithSummary("Create a new patient");

        group.MapPut("{nhsNumber}", UpdatePatient)
            .WithName("UpdatePatient")
            .WithSummary("Update an existing patient");

        group.MapDelete("{nhsNumber}", DeletePatient)
            .WithName("DeletePatient")
            .WithSummary("Delete a patient");

        return group;
    }

    private static async Task<Ok<List<PatientResponse>>> GetAllPatients(
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        List<Patient> patients = await dbContext.Patients
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        List<PatientResponse> response = patients.Select(PatientResponse.FromPatient).ToList();
        return TypedResults.Ok(response);
    }

    private static async Task<Results<Ok<PatientResponse>, NotFound<ProblemDetails>>> GetPatient(
        string nhsNumber,
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        NhsNumber parsedNhsNumber;
        try
        {
            parsedNhsNumber = NhsNumber.From(nhsNumber);
        }
        catch
        {
            return TypedResults.NotFound(new ProblemDetails
            {
                Title = "Patient not found",
                Detail = $"No patient found with NHS number '{nhsNumber}'",
                Status = StatusCodes.Status404NotFound
            });
        }

        Patient? patient = await dbContext.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.NhsNumber == parsedNhsNumber, cancellationToken);

        if (patient is null)
        {
            return TypedResults.NotFound(new ProblemDetails
            {
                Title = "Patient not found",
                Detail = $"No patient found with NHS number '{nhsNumber}'",
                Status = StatusCodes.Status404NotFound
            });
        }

        return TypedResults.Ok(PatientResponse.FromPatient(patient));
    }

    private static async Task<Results<Created<PatientResponse>, Conflict<ProblemDetails>, ValidationProblem>>
        CreatePatient(
            CreatePatientRequest request,
            IValidator<CreatePatientRequest> validator,
            ApplicationDbContext dbContext,
            CancellationToken cancellationToken)
    {
        ValidationResult validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        NhsNumber nhsNumber = NhsNumber.From(request.NhsNumber);

        bool exists = await dbContext.Patients
            .AnyAsync(p => p.NhsNumber == nhsNumber, cancellationToken);

        if (exists)
        {
            return TypedResults.Conflict(new ProblemDetails
            {
                Title = "Patient already exists",
                Detail = $"A patient with NHS number '{request.NhsNumber}' already exists",
                Status = StatusCodes.Status409Conflict
            });
        }

        Patient patient = new()
        {
            NhsNumber = nhsNumber,
            Name = request.Name,
            DateOfBirth = request.DateOfBirth,
            Postcode = Postcode.From(request.Postcode)
        };

        dbContext.Patients.Add(patient);
        await dbContext.SaveChangesAsync(cancellationToken);

        PatientResponse response = PatientResponse.FromPatient(patient);
        return TypedResults.Created($"/api/patients/{patient.NhsNumber.Value}", response);
    }

    private static async Task<Results<Ok<PatientResponse>, NotFound<ProblemDetails>, ValidationProblem>> UpdatePatient(
        string nhsNumber,
        UpdatePatientRequest request,
        IValidator<UpdatePatientRequest> validator,
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        ValidationResult validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        NhsNumber parsedNhsNumber;
        try
        {
            parsedNhsNumber = NhsNumber.From(nhsNumber);
        }
        catch
        {
            return TypedResults.NotFound(new ProblemDetails
            {
                Title = "Patient not found",
                Detail = $"No patient found with NHS number '{nhsNumber}'",
                Status = StatusCodes.Status404NotFound
            });
        }

        Patient? patient = await dbContext.Patients
            .FirstOrDefaultAsync(p => p.NhsNumber == parsedNhsNumber, cancellationToken);

        if (patient is null)
        {
            return TypedResults.NotFound(new ProblemDetails
            {
                Title = "Patient not found",
                Detail = $"No patient found with NHS number '{nhsNumber}'",
                Status = StatusCodes.Status404NotFound
            });
        }

        // Update properties using record with expression
        Patient updatedPatient = patient with
        {
            Name = request.Name, DateOfBirth = request.DateOfBirth, Postcode = Postcode.From(request.Postcode)
        };

        dbContext.Entry(patient).CurrentValues.SetValues(updatedPatient);
        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(PatientResponse.FromPatient(updatedPatient));
    }

    private static async Task<Results<NoContent, NotFound<ProblemDetails>>> DeletePatient(
        string nhsNumber,
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        NhsNumber parsedNhsNumber;
        try
        {
            parsedNhsNumber = NhsNumber.From(nhsNumber);
        }
        catch
        {
            return TypedResults.NotFound(new ProblemDetails
            {
                Title = "Patient not found",
                Detail = $"No patient found with NHS number '{nhsNumber}'",
                Status = StatusCodes.Status404NotFound
            });
        }

        Patient? patient = await dbContext.Patients
            .FirstOrDefaultAsync(p => p.NhsNumber == parsedNhsNumber, cancellationToken);

        if (patient is null)
        {
            return TypedResults.NotFound(new ProblemDetails
            {
                Title = "Patient not found",
                Detail = $"No patient found with NHS number '{nhsNumber}'",
                Status = StatusCodes.Status404NotFound
            });
        }

        dbContext.Patients.Remove(patient);
        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.NoContent();
    }
}

/// <summary>
/// Response DTO for patient data.
/// </summary>
public sealed record PatientResponse
{
    public required string NhsNumber { get; init; }
    public required string Name { get; init; }
    public required DateOnly DateOfBirth { get; init; }
    public required string Postcode { get; init; }
    public required int Age { get; init; }

    public static PatientResponse FromPatient(Patient patient) => new()
    {
        NhsNumber = patient.NhsNumber.Value,
        Name = patient.Name,
        DateOfBirth = patient.DateOfBirth,
        Postcode = patient.Postcode.Value,
        Age = patient.Age
    };
}
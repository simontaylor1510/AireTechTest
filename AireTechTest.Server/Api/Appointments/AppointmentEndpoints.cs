using AireTechTest.Server.Data;
using AireTechTest.Server.Domain;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AireTechTest.Server.Api.Appointments;

public static class AppointmentEndpoints
{
    public static RouteGroupBuilder MapAppointmentEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("", GetAllAppointments)
            .WithName("GetAllAppointments")
            .WithSummary("Get all appointments");

        group.MapGet("{id:guid}", GetAppointment)
            .WithName("GetAppointment")
            .WithSummary("Get an appointment by ID");

        group.MapPost("", CreateAppointment)
            .WithName("CreateAppointment")
            .WithSummary("Create a new appointment");

        group.MapPut("{id:guid}", UpdateAppointment)
            .WithName("UpdateAppointment")
            .WithSummary("Update an existing appointment");

        group.MapDelete("{id:guid}", DeleteAppointment)
            .WithName("DeleteAppointment")
            .WithSummary("Delete an appointment");

        return group;
    }

    private static async Task<Ok<List<AppointmentResponse>>> GetAllAppointments(
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        List<Appointment> appointments = await dbContext.Appointments
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        List<AppointmentResponse> response = appointments.Select(AppointmentResponse.FromAppointment).ToList();
        return TypedResults.Ok(response);
    }

    private static async Task<Results<Ok<AppointmentResponse>, NotFound<ProblemDetails>>> GetAppointment(
        Guid id,
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            return TypedResults.NotFound(new ProblemDetails
            {
                Title = "Appointment not found",
                Detail = "Appointment ID cannot be empty",
                Status = StatusCodes.Status404NotFound
            });
        }

        AppointmentId appointmentId = AppointmentId.From(id);

        Appointment? appointment = await dbContext.Appointments
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == appointmentId, cancellationToken);

        if (appointment is null)
        {
            return TypedResults.NotFound(new ProblemDetails
            {
                Title = "Appointment not found",
                Detail = $"No appointment found with ID '{id}'",
                Status = StatusCodes.Status404NotFound
            });
        }

        return TypedResults.Ok(AppointmentResponse.FromAppointment(appointment));
    }

    private static async Task<Results<Created<AppointmentResponse>, NotFound<ProblemDetails>, ValidationProblem>> CreateAppointment(
        CreateAppointmentRequest request,
        IValidator<CreateAppointmentRequest> validator,
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        ValidationResult validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        NhsNumber patientNhsNumber = NhsNumber.From(request.PatientNhsNumber);

        // Verify patient exists
        bool patientExists = await dbContext.Patients
            .AnyAsync(p => p.NhsNumber == patientNhsNumber, cancellationToken);

        if (!patientExists)
        {
            return TypedResults.NotFound(new ProblemDetails
            {
                Title = "Patient not found",
                Detail = $"No patient found with NHS number '{request.PatientNhsNumber}'",
                Status = StatusCodes.Status404NotFound
            });
        }

        Appointment appointment = new()
        {
            Id = AppointmentId.New(),
            PatientNhsNumber = patientNhsNumber,
            Status = request.Status,
            Time = request.Time,
            Duration = TimeSpan.FromMinutes(request.DurationMinutes),
            Clinician = request.Clinician,
            Department = request.Department,
            Location = Postcode.From(request.Location)
        };

        dbContext.Appointments.Add(appointment);
        await dbContext.SaveChangesAsync(cancellationToken);

        AppointmentResponse response = AppointmentResponse.FromAppointment(appointment);
        return TypedResults.Created($"/api/appointments/{appointment.Id.Value}", response);
    }

    private static async Task<Results<Ok<AppointmentResponse>, NotFound<ProblemDetails>, ValidationProblem>> UpdateAppointment(
        Guid id,
        UpdateAppointmentRequest request,
        IValidator<UpdateAppointmentRequest> validator,
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        ValidationResult validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        if (id == Guid.Empty)
        {
            return TypedResults.NotFound(new ProblemDetails
            {
                Title = "Appointment not found",
                Detail = "Appointment ID cannot be empty",
                Status = StatusCodes.Status404NotFound
            });
        }

        AppointmentId appointmentId = AppointmentId.From(id);

        Appointment? appointment = await dbContext.Appointments
            .FirstOrDefaultAsync(a => a.Id == appointmentId, cancellationToken);

        if (appointment is null)
        {
            return TypedResults.NotFound(new ProblemDetails
            {
                Title = "Appointment not found",
                Detail = $"No appointment found with ID '{id}'",
                Status = StatusCodes.Status404NotFound
            });
        }

        Appointment updatedAppointment = appointment with
        {
            Status = request.Status,
            Time = request.Time,
            Duration = TimeSpan.FromMinutes(request.DurationMinutes),
            Clinician = request.Clinician,
            Department = request.Department,
            Location = Postcode.From(request.Location)
        };

        dbContext.Entry(appointment).CurrentValues.SetValues(updatedAppointment);
        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(AppointmentResponse.FromAppointment(updatedAppointment));
    }

    private static async Task<Results<NoContent, NotFound<ProblemDetails>>> DeleteAppointment(
        Guid id,
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            return TypedResults.NotFound(new ProblemDetails
            {
                Title = "Appointment not found",
                Detail = "Appointment ID cannot be empty",
                Status = StatusCodes.Status404NotFound
            });
        }

        AppointmentId appointmentId = AppointmentId.From(id);

        Appointment? appointment = await dbContext.Appointments
            .FirstOrDefaultAsync(a => a.Id == appointmentId, cancellationToken);

        if (appointment is null)
        {
            return TypedResults.NotFound(new ProblemDetails
            {
                Title = "Appointment not found",
                Detail = $"No appointment found with ID '{id}'",
                Status = StatusCodes.Status404NotFound
            });
        }

        dbContext.Appointments.Remove(appointment);
        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.NoContent();
    }
}

/// <summary>
/// Response DTO for appointment data.
/// </summary>
public sealed record AppointmentResponse
{
    public required Guid Id { get; init; }
    public required string PatientNhsNumber { get; init; }
    public required AppointmentStatus Status { get; init; }
    public required DateTimeOffset Time { get; init; }
    public required int DurationMinutes { get; init; }
    public required string Clinician { get; init; }
    public required Department Department { get; init; }
    public required string Location { get; init; }
    public required DateTimeOffset EndTime { get; init; }
    public required bool IsPast { get; init; }
    public required bool IsFuture { get; init; }

    public static AppointmentResponse FromAppointment(Appointment appointment) => new()
    {
        Id = appointment.Id.Value,
        PatientNhsNumber = appointment.PatientNhsNumber.Value,
        Status = appointment.Status,
        Time = appointment.Time,
        DurationMinutes = (int)appointment.Duration.TotalMinutes,
        Clinician = appointment.Clinician,
        Department = appointment.Department,
        Location = appointment.Location.Value,
        EndTime = appointment.EndTime,
        IsPast = appointment.IsPast,
        IsFuture = appointment.IsFuture
    };
}
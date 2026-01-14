using AireTechTest.Server.Domain;

namespace AireTechTest.Server.Api.Appointments;

/// <summary>
/// Request to create a new appointment.
/// </summary>
public sealed record CreateAppointmentRequest
{
    /// <summary>
    /// The NHS number of the patient.
    /// </summary>
    public required string PatientNhsNumber { get; init; }

    /// <summary>
    /// The status of the appointment.
    /// </summary>
    public required AppointmentStatus Status { get; init; }

    /// <summary>
    /// The scheduled date and time of the appointment.
    /// </summary>
    public required DateTimeOffset Time { get; init; }

    /// <summary>
    /// The duration of the appointment in minutes.
    /// </summary>
    public required int DurationMinutes { get; init; }

    /// <summary>
    /// The name of the clinician.
    /// </summary>
    public required string Clinician { get; init; }

    /// <summary>
    /// The department where the appointment takes place.
    /// </summary>
    public required Department Department { get; init; }

    /// <summary>
    /// The postcode of the appointment location.
    /// </summary>
    public required string Location { get; init; }
}

/// <summary>
/// Request to update an existing appointment.
/// </summary>
public sealed record UpdateAppointmentRequest
{
    /// <summary>
    /// The status of the appointment.
    /// </summary>
    public required AppointmentStatus Status { get; init; }

    /// <summary>
    /// The scheduled date and time of the appointment.
    /// </summary>
    public required DateTimeOffset Time { get; init; }

    /// <summary>
    /// The duration of the appointment in minutes.
    /// </summary>
    public required int DurationMinutes { get; init; }

    /// <summary>
    /// The name of the clinician.
    /// </summary>
    public required string Clinician { get; init; }

    /// <summary>
    /// The department where the appointment takes place.
    /// </summary>
    public required Department Department { get; init; }

    /// <summary>
    /// The postcode of the appointment location.
    /// </summary>
    public required string Location { get; init; }
}
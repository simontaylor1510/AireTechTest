namespace AireTechTest.Server.Domain;

/// <summary>
/// Represents a medical appointment.
/// </summary>
public sealed record Appointment
{
    /// <summary>
    /// Gets the unique identifier for this appointment.
    /// </summary>
    public required AppointmentId Id { get; init; }

    /// <summary>
    /// Gets the NHS number of the patient for this appointment.
    /// </summary>
    public required NhsNumber PatientNhsNumber { get; init; }

    /// <summary>
    /// Gets the status of the appointment.
    /// </summary>
    public required AppointmentStatus Status { get; init; }

    /// <summary>
    /// Gets the scheduled date and time of the appointment.
    /// </summary>
    public required DateTimeOffset Time { get; init; }

    /// <summary>
    /// Gets the duration of the appointment.
    /// </summary>
    public required TimeSpan Duration { get; init; }

    /// <summary>
    /// Gets the name of the clinician for this appointment.
    /// </summary>
    public required string Clinician { get; init; }

    /// <summary>
    /// Gets the department where the appointment takes place.
    /// </summary>
    public required Department Department { get; init; }

    /// <summary>
    /// Gets the postcode of the appointment location.
    /// </summary>
    public required Postcode Location { get; init; }

    /// <summary>
    /// Gets the scheduled end time of the appointment.
    /// </summary>
    public DateTimeOffset EndTime => Time.Add(Duration);

    /// <summary>
    /// Gets whether the appointment is in the past.
    /// </summary>
    public bool IsPast => Time < DateTimeOffset.Now;

    /// <summary>
    /// Gets whether the appointment is in the future.
    /// </summary>
    public bool IsFuture => Time > DateTimeOffset.Now;
}
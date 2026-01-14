namespace AireTechTest.Server.Domain;

/// <summary>
/// Status of an appointment.
/// </summary>
public enum AppointmentStatus
{
    /// <summary>
    /// Appointment is currently active/scheduled.
    /// </summary>
    Active,

    /// <summary>
    /// Patient attended the appointment.
    /// </summary>
    Attended,

    /// <summary>
    /// Patient missed the appointment (did not attend).
    /// </summary>
    Missed,

    /// <summary>
    /// Appointment was cancelled.
    /// </summary>
    Cancelled
}
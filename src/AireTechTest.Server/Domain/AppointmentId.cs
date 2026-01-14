using Vogen;

namespace AireTechTest.Server.Domain;

/// <summary>
/// Strongly-typed identifier for an appointment.
/// </summary>
[ValueObject<Guid>]
public readonly partial struct AppointmentId
{
    private static Validation Validate(Guid input)
    {
        return input == Guid.Empty ? Validation.Invalid("AppointmentId cannot be empty") : Validation.Ok;
    }

    /// <summary>
    /// Creates a new unique AppointmentId.
    /// </summary>
    public static AppointmentId New() => From(Guid.NewGuid());

    private static Guid NormalizeInput(Guid input)
    {
        return input;
    }
}
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
        if (input == Guid.Empty)
        {
            return Validation.Invalid("AppointmentId cannot be empty");
        }

        return Validation.Ok;
    }

    /// <summary>
    /// Creates a new unique AppointmentId.
    /// </summary>
    public static AppointmentId New() => From(Guid.NewGuid());
}
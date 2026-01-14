using Vogen;

namespace AireTechTest.Server.Domain;

/// <summary>
/// Strongly-typed identifier for an appointment.
/// </summary>
[ValueObject<Guid>]
#pragma warning disable AddNormalizeInputMethod
#pragma warning disable CA1036
public readonly partial struct AppointmentId
#pragma warning restore CA1036
#pragma warning restore AddNormalizeInputMethod
{
    private static Validation Validate(Guid input)
    {
        return input == Guid.Empty ? Validation.Invalid("AppointmentId cannot be empty") : Validation.Ok;
    }

    /// <summary>
    /// Creates a new unique AppointmentId.
    /// </summary>
    public static AppointmentId New() => From(Guid.NewGuid());
}
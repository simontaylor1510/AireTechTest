namespace AireTechTest.Server.Domain;

/// <summary>
/// Represents a patient in the healthcare system.
/// </summary>
public sealed record Patient
{
    /// <summary>
    /// Gets the patient's NHS number (unique identifier).
    /// </summary>
    public required NhsNumber NhsNumber { get; init; }

    /// <summary>
    /// Gets the patient's full name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the patient's date of birth.
    /// </summary>
    public required DateOnly DateOfBirth { get; init; }

    /// <summary>
    /// Gets the patient's postcode.
    /// </summary>
    public required Postcode Postcode { get; init; }

    /// <summary>
    /// Calculates the patient's age as of today.
    /// </summary>
    public int Age
    {
        get
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            int age = today.Year - DateOfBirth.Year;

            if (DateOfBirth > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }
    }
}
namespace AireTechTest.Server.Api.Patients;

/// <summary>
/// Request to create a new patient.
/// </summary>
public sealed record CreatePatientRequest
{
    /// <summary>
    /// The patient's NHS number (10 digits).
    /// </summary>
    public required string NhsNumber { get; init; }

    /// <summary>
    /// The patient's full name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// The patient's date of birth.
    /// </summary>
    public required DateOnly DateOfBirth { get; init; }

    /// <summary>
    /// The patient's postcode.
    /// </summary>
    public required string Postcode { get; init; }
}

/// <summary>
/// Request to update an existing patient.
/// </summary>
public sealed record UpdatePatientRequest
{
    /// <summary>
    /// The patient's full name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// The patient's date of birth.
    /// </summary>
    public required DateOnly DateOfBirth { get; init; }

    /// <summary>
    /// The patient's postcode.
    /// </summary>
    public required string Postcode { get; init; }
}
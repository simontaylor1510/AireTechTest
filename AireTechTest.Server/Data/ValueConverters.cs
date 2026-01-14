using AireTechTest.Server.Domain;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AireTechTest.Server.Data;

/// <summary>
/// EF Core value converter for NhsNumber.
/// </summary>
public class NhsNumberConverter : ValueConverter<NhsNumber, string>
{
    public NhsNumberConverter()
        : base(
            nhsNumber => nhsNumber.Value,
            value => NhsNumber.From(value))
    {
    }
}

/// <summary>
/// EF Core value converter for Postcode.
/// </summary>
public class PostcodeConverter : ValueConverter<Postcode, string>
{
    public PostcodeConverter()
        : base(
            postcode => postcode.Value,
            value => Postcode.From(value))
    {
    }
}

/// <summary>
/// EF Core value converter for AppointmentId.
/// </summary>
public class AppointmentIdConverter : ValueConverter<AppointmentId, Guid>
{
    public AppointmentIdConverter()
        : base(
            appointmentId => appointmentId.Value,
            value => AppointmentId.From(value))
    {
    }
}
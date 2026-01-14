using AireTechTest.Server.Api.Appointments;
using AireTechTest.Server.Domain;

using FluentValidation.Results;

namespace AireTechTest.Server.Tests.Api.Appointments;

public class UpdateAppointmentRequestValidatorTests
{
    private readonly UpdateAppointmentRequestValidator _validator = new();

    [Test]
    public async Task ValidRequest_ShouldPass()
    {
        UpdateAppointmentRequest request = new()
        {
            Status = AppointmentStatus.Active,
            Time = DateTimeOffset.Now.AddDays(1),
            DurationMinutes = 30,
            Clinician = "Dr. Smith",
            Department = Department.Oncology,
            Location = "SW1A 1AA"
        };

        ValidationResult result = await _validator.ValidateAsync(request);

        await Assert.That(result.IsValid).IsTrue();
    }

    [Test]
    public async Task ZeroDuration_ShouldFail()
    {
        UpdateAppointmentRequest request = new()
        {
            Status = AppointmentStatus.Active,
            Time = DateTimeOffset.Now.AddDays(1),
            DurationMinutes = 0,
            Clinician = "Dr. Smith",
            Department = Department.Oncology,
            Location = "SW1A 1AA"
        };

        ValidationResult result = await _validator.ValidateAsync(request);

        await Assert.That(result.IsValid).IsFalse();
        await Assert.That(result.Errors.Any(e => e.PropertyName == "DurationMinutes")).IsTrue();
    }

    [Test]
    public async Task NegativeDuration_ShouldFail()
    {
        UpdateAppointmentRequest request = new()
        {
            Status = AppointmentStatus.Active,
            Time = DateTimeOffset.Now.AddDays(1),
            DurationMinutes = -30,
            Clinician = "Dr. Smith",
            Department = Department.Oncology,
            Location = "SW1A 1AA"
        };

        ValidationResult result = await _validator.ValidateAsync(request);

        await Assert.That(result.IsValid).IsFalse();
        await Assert.That(result.Errors.Any(e => e.PropertyName == "DurationMinutes")).IsTrue();
    }

    [Test]
    public async Task DurationExceeds8Hours_ShouldFail()
    {
        UpdateAppointmentRequest request = new()
        {
            Status = AppointmentStatus.Active,
            Time = DateTimeOffset.Now.AddDays(1),
            DurationMinutes = 481,
            Clinician = "Dr. Smith",
            Department = Department.Oncology,
            Location = "SW1A 1AA"
        };

        ValidationResult result = await _validator.ValidateAsync(request);

        await Assert.That(result.IsValid).IsFalse();
        await Assert.That(result.Errors.Any(e => e.PropertyName == "DurationMinutes")).IsTrue();
    }

    [Test]
    public async Task EmptyClinician_ShouldFail()
    {
        UpdateAppointmentRequest request = new()
        {
            Status = AppointmentStatus.Active,
            Time = DateTimeOffset.Now.AddDays(1),
            DurationMinutes = 30,
            Clinician = "",
            Department = Department.Oncology,
            Location = "SW1A 1AA"
        };

        ValidationResult result = await _validator.ValidateAsync(request);

        await Assert.That(result.IsValid).IsFalse();
        await Assert.That(result.Errors.Any(e => e.PropertyName == "Clinician")).IsTrue();
    }

    [Test]
    public async Task ClinicianTooLong_ShouldFail()
    {
        UpdateAppointmentRequest request = new()
        {
            Status = AppointmentStatus.Active,
            Time = DateTimeOffset.Now.AddDays(1),
            DurationMinutes = 30,
            Clinician = new string('A', 201),
            Department = Department.Oncology,
            Location = "SW1A 1AA"
        };

        ValidationResult result = await _validator.ValidateAsync(request);

        await Assert.That(result.IsValid).IsFalse();
        await Assert.That(result.Errors.Any(e => e.PropertyName == "Clinician")).IsTrue();
    }

    [Test]
    public async Task EmptyLocation_ShouldFail()
    {
        UpdateAppointmentRequest request = new()
        {
            Status = AppointmentStatus.Active,
            Time = DateTimeOffset.Now.AddDays(1),
            DurationMinutes = 30,
            Clinician = "Dr. Smith",
            Department = Department.Oncology,
            Location = ""
        };

        ValidationResult result = await _validator.ValidateAsync(request);

        await Assert.That(result.IsValid).IsFalse();
        await Assert.That(result.Errors.Any(e => e.PropertyName == "Location")).IsTrue();
    }

    [Test]
    public async Task InvalidLocation_ShouldFail()
    {
        UpdateAppointmentRequest request = new()
        {
            Status = AppointmentStatus.Active,
            Time = DateTimeOffset.Now.AddDays(1),
            DurationMinutes = 30,
            Clinician = "Dr. Smith",
            Department = Department.Oncology,
            Location = "INVALID"
        };

        ValidationResult result = await _validator.ValidateAsync(request);

        await Assert.That(result.IsValid).IsFalse();
        await Assert.That(result.Errors.Any(e => e.PropertyName == "Location")).IsTrue();
    }
}
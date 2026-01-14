using AireTechTest.Server.Api.Patients;
using FluentValidation.Results;

namespace AireTechTest.Server.Tests.Api.Patients;

public class UpdatePatientRequestValidatorTests
{
    private readonly UpdatePatientRequestValidator _validator = new();

    [Test]
    public async Task ValidRequest_ShouldPass()
    {
        UpdatePatientRequest request = new()
        {
            Name = "John Smith",
            DateOfBirth = new DateOnly(1990, 1, 15),
            Postcode = "SW1A 1AA"
        };

        ValidationResult result = await _validator.ValidateAsync(request);

        await Assert.That(result.IsValid).IsTrue();
    }

    [Test]
    public async Task EmptyName_ShouldFail()
    {
        UpdatePatientRequest request = new()
        {
            Name = "",
            DateOfBirth = new DateOnly(1990, 1, 15),
            Postcode = "SW1A 1AA"
        };

        ValidationResult result = await _validator.ValidateAsync(request);

        await Assert.That(result.IsValid).IsFalse();
        await Assert.That(result.Errors.Any(e => e.PropertyName == "Name")).IsTrue();
    }

    [Test]
    public async Task NameTooLong_ShouldFail()
    {
        UpdatePatientRequest request = new()
        {
            Name = new string('A', 201),
            DateOfBirth = new DateOnly(1990, 1, 15),
            Postcode = "SW1A 1AA"
        };

        ValidationResult result = await _validator.ValidateAsync(request);

        await Assert.That(result.IsValid).IsFalse();
        await Assert.That(result.Errors.Any(e => e.PropertyName == "Name")).IsTrue();
    }

    [Test]
    public async Task FutureDateOfBirth_ShouldFail()
    {
        UpdatePatientRequest request = new()
        {
            Name = "John Smith",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            Postcode = "SW1A 1AA"
        };

        ValidationResult result = await _validator.ValidateAsync(request);

        await Assert.That(result.IsValid).IsFalse();
        await Assert.That(result.Errors.Any(e => e.PropertyName == "DateOfBirth")).IsTrue();
    }

    [Test]
    public async Task UnreasonablyOldDateOfBirth_ShouldFail()
    {
        UpdatePatientRequest request = new()
        {
            Name = "John Smith",
            DateOfBirth = new DateOnly(1800, 1, 1),
            Postcode = "SW1A 1AA"
        };

        ValidationResult result = await _validator.ValidateAsync(request);

        await Assert.That(result.IsValid).IsFalse();
        await Assert.That(result.Errors.Any(e => e.PropertyName == "DateOfBirth")).IsTrue();
    }

    [Test]
    public async Task EmptyPostcode_ShouldFail()
    {
        UpdatePatientRequest request = new()
        {
            Name = "John Smith",
            DateOfBirth = new DateOnly(1990, 1, 15),
            Postcode = ""
        };

        ValidationResult result = await _validator.ValidateAsync(request);

        await Assert.That(result.IsValid).IsFalse();
        await Assert.That(result.Errors.Any(e => e.PropertyName == "Postcode")).IsTrue();
    }

    [Test]
    public async Task InvalidPostcode_ShouldFail()
    {
        UpdatePatientRequest request = new()
        {
            Name = "John Smith",
            DateOfBirth = new DateOnly(1990, 1, 15),
            Postcode = "INVALID"
        };

        ValidationResult result = await _validator.ValidateAsync(request);

        await Assert.That(result.IsValid).IsFalse();
        await Assert.That(result.Errors.Any(e => e.PropertyName == "Postcode")).IsTrue();
    }
}
using AireTechTest.Server.Api.Patients;

using FluentValidation.Results;

namespace AireTechTest.Server.Tests.Api.Patients;

public class CreatePatientRequestValidatorTests
{
    private readonly CreatePatientRequestValidator _validator = new();

    [Test]
    public async Task ValidRequest_ShouldPass()
    {
        CreatePatientRequest request = new()
        {
            NhsNumber = "9434765919",
            Name = "John Smith",
            DateOfBirth = new DateOnly(1990, 1, 15),
            Postcode = "SW1A 1AA"
        };

        ValidationResult result = await _validator.ValidateAsync(request);

        await Assert.That(result.IsValid).IsTrue();
    }

    [Test]
    public async Task EmptyNhsNumber_ShouldFail()
    {
        CreatePatientRequest request = new()
        {
            NhsNumber = "", Name = "John Smith", DateOfBirth = new DateOnly(1990, 1, 15), Postcode = "SW1A 1AA"
        };

        ValidationResult result = await _validator.ValidateAsync(request);

        await Assert.That(result.IsValid).IsFalse();
        await Assert.That(result.Errors.Any(e => e.PropertyName == "NhsNumber")).IsTrue();
    }

    [Test]
    public async Task InvalidNhsNumber_ShouldFail()
    {
        CreatePatientRequest request = new()
        {
            NhsNumber = "1234567890",
            Name = "John Smith",
            DateOfBirth = new DateOnly(1990, 1, 15),
            Postcode = "SW1A 1AA"
        };

        ValidationResult result = await _validator.ValidateAsync(request);

        await Assert.That(result.IsValid).IsFalse();
        await Assert.That(result.Errors.Any(e => e.PropertyName == "NhsNumber")).IsTrue();
    }

    [Test]
    public async Task EmptyName_ShouldFail()
    {
        CreatePatientRequest request = new()
        {
            NhsNumber = "9434765919", Name = "", DateOfBirth = new DateOnly(1990, 1, 15), Postcode = "SW1A 1AA"
        };

        ValidationResult result = await _validator.ValidateAsync(request);

        await Assert.That(result.IsValid).IsFalse();
        await Assert.That(result.Errors.Any(e => e.PropertyName == "Name")).IsTrue();
    }

    [Test]
    public async Task NameTooLong_ShouldFail()
    {
        CreatePatientRequest request = new()
        {
            NhsNumber = "9434765919",
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
        CreatePatientRequest request = new()
        {
            NhsNumber = "9434765919",
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
        CreatePatientRequest request = new()
        {
            NhsNumber = "9434765919",
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
        CreatePatientRequest request = new()
        {
            NhsNumber = "9434765919", Name = "John Smith", DateOfBirth = new DateOnly(1990, 1, 15), Postcode = ""
        };

        ValidationResult result = await _validator.ValidateAsync(request);

        await Assert.That(result.IsValid).IsFalse();
        await Assert.That(result.Errors.Any(e => e.PropertyName == "Postcode")).IsTrue();
    }

    [Test]
    public async Task InvalidPostcode_ShouldFail()
    {
        CreatePatientRequest request = new()
        {
            NhsNumber = "9434765919",
            Name = "John Smith",
            DateOfBirth = new DateOnly(1990, 1, 15),
            Postcode = "INVALID"
        };

        ValidationResult result = await _validator.ValidateAsync(request);

        await Assert.That(result.IsValid).IsFalse();
        await Assert.That(result.Errors.Any(e => e.PropertyName == "Postcode")).IsTrue();
    }

    [Test]
    public async Task MultipleErrors_ShouldReturnAllErrors()
    {
        CreatePatientRequest request = new()
        {
            NhsNumber = "", Name = "", DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddDays(1)), Postcode = ""
        };

        ValidationResult result = await _validator.ValidateAsync(request);

        await Assert.That(result.IsValid).IsFalse();
        await Assert.That(result.Errors.Count).IsGreaterThanOrEqualTo(4);
    }
}
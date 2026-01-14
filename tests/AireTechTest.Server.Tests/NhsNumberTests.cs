using System.Diagnostics.CodeAnalysis;

using AireTechTest.Server.Domain;

using Vogen;

namespace AireTechTest.Server.Tests;

[SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global")]
public class NhsNumberTests
{
    [Test]
    public async Task NhsNumber_ValidNumber_CreatesSuccessfully()
    {
        // Arrange & Act - Using example from NHS Data Dictionary
        NhsNumber nhsNumber = NhsNumber.From("9434765919");

        // Assert
        await Assert.That(nhsNumber.Value).IsEqualTo("9434765919");
    }

    [Test]
    public async Task NhsNumber_ValidNumberWithSpaces_CreatesSuccessfully()
    {
        // Arrange & Act - Should normalize to digits only
        NhsNumber nhsNumber = NhsNumber.From("943 476 5919");

        // Assert
        await Assert.That(nhsNumber.Value).IsEqualTo("9434765919");
    }

    [Test]
    public async Task NhsNumber_ValidNumberWithHyphens_CreatesSuccessfully()
    {
        // Arrange & Act
        NhsNumber nhsNumber = NhsNumber.From("943-476-5919");

        // Assert
        await Assert.That(nhsNumber.Value).IsEqualTo("9434765919");
    }

    [Test]
    public async Task NhsNumber_CheckDigitZero_CreatesSuccessfully()
    {
        // Arrange & Act - A number where check digit calculation results in 11 (stored as 0)
        NhsNumber nhsNumber = NhsNumber.From("4505577104");

        // Assert
        await Assert.That(nhsNumber.Value).IsEqualTo("4505577104");
    }

    [Test]
    public async Task NhsNumber_TooShort_ThrowsValueObjectValidationException()
    {
        // Arrange & Act & Assert
        await Assert.That(() => { _ = NhsNumber.From("123456789"); }).Throws<ValueObjectValidationException>();
    }

    [Test]
    public async Task NhsNumber_TooLong_ThrowsValueObjectValidationException()
    {
        // Arrange & Act & Assert
        await Assert.That(() => { _ = NhsNumber.From("12345678901"); }).Throws<ValueObjectValidationException>();
    }

    [Test]
    public async Task NhsNumber_AllSameDigits_ThrowsValueObjectValidationException()
    {
        // Arrange & Act & Assert
        await Assert.That(() => { _ = NhsNumber.From("0000000000"); }).Throws<ValueObjectValidationException>();
    }

    [Test]
    public async Task NhsNumber_InvalidCheckDigit_ThrowsValueObjectValidationException()
    {
        // Arrange & Act & Assert - Changed last digit from 9 to 8
        await Assert.That(() => { _ = NhsNumber.From("9434765918"); }).Throws<ValueObjectValidationException>();
    }

    [Test]
    public async Task NhsNumber_Empty_ThrowsValueObjectValidationException()
    {
        // Arrange & Act & Assert
        await Assert.That(() => { _ = NhsNumber.From(""); }).Throws<ValueObjectValidationException>();
    }

    [Test]
    public async Task NhsNumber_NonNumeric_ThrowsValueObjectValidationException()
    {
        // Arrange & Act & Assert
        await Assert.That(() => { _ = NhsNumber.From("abcdefghij"); }).Throws<ValueObjectValidationException>();
    }

    [Test]
    public async Task NhsNumber_Equality_WorksCorrectly()
    {
        // Arrange
        NhsNumber nhsNumber1 = NhsNumber.From("9434765919");
        NhsNumber nhsNumber2 = NhsNumber.From("943 476 5919");

        // Act & Assert
        await Assert.That(nhsNumber1).IsEqualTo(nhsNumber2);
    }
}
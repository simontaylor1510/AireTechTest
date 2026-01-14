using System.Diagnostics.CodeAnalysis;

using AireTechTest.Server.Domain;

using TUnit.Assertions.AssertConditions.Throws;

using Vogen;

namespace AireTechTest.Server.Tests;

[SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global")]
public class PostcodeTests
{
    // Valid format tests - all 6 standard formats

    [Test]
    public async Task Postcode_FormatA9_9AA_CreatesSuccessfully()
    {
        // Format: A9 9AA (e.g., M1 1AE)
        Postcode postcode = Postcode.From("M1 1AE");

        await Assert.That(postcode.Value).IsEqualTo("M1 1AE");
    }

    [Test]
    public async Task Postcode_FormatA99_9AA_CreatesSuccessfully()
    {
        // Format: A99 9AA (e.g., M60 1NW)
        Postcode postcode = Postcode.From("M60 1NW");

        await Assert.That(postcode.Value).IsEqualTo("M60 1NW");
    }

    [Test]
    public async Task Postcode_FormatA9A_9AA_CreatesSuccessfully()
    {
        // Format: A9A 9AA (e.g., W1A 0AX)
        Postcode postcode = Postcode.From("W1A 0AX");

        await Assert.That(postcode.Value).IsEqualTo("W1A 0AX");
    }

    [Test]
    public async Task Postcode_FormatAA9_9AA_CreatesSuccessfully()
    {
        // Format: AA9 9AA (e.g., CR2 6XH)
        Postcode postcode = Postcode.From("CR2 6XH");

        await Assert.That(postcode.Value).IsEqualTo("CR2 6XH");
    }

    [Test]
    public async Task Postcode_FormatAA99_9AA_CreatesSuccessfully()
    {
        // Format: AA99 9AA (e.g., DN55 1PT)
        Postcode postcode = Postcode.From("DN55 1PT");

        await Assert.That(postcode.Value).IsEqualTo("DN55 1PT");
    }

    [Test]
    public async Task Postcode_FormatAA9A_9AA_CreatesSuccessfully()
    {
        // Format: AA9A 9AA (e.g., EC1A 1BB)
        Postcode postcode = Postcode.From("EC1A 1BB");

        await Assert.That(postcode.Value).IsEqualTo("EC1A 1BB");
    }

    [Test]
    public async Task Postcode_SpecialGIR0AA_CreatesSuccessfully()
    {
        // Special case: GIR 0AA (old Girobank postcode)
        Postcode postcode = Postcode.From("GIR 0AA");

        await Assert.That(postcode.Value).IsEqualTo("GIR 0AA");
    }

    // Normalization tests

    [Test]
    public async Task Postcode_LowercaseInput_NormalizesToUppercase()
    {
        Postcode postcode = Postcode.From("sw1a 1aa");

        await Assert.That(postcode.Value).IsEqualTo("SW1A 1AA");
    }

    [Test]
    public async Task Postcode_NoSpace_NormalizesWithSpace()
    {
        Postcode postcode = Postcode.From("SW1A1AA");

        await Assert.That(postcode.Value).IsEqualTo("SW1A 1AA");
    }

    [Test]
    public async Task Postcode_ExtraSpaces_NormalizesToSingleSpace()
    {
        Postcode postcode = Postcode.From("SW1A   1AA");

        await Assert.That(postcode.Value).IsEqualTo("SW1A 1AA");
    }

    [Test]
    public async Task Postcode_LeadingTrailingSpaces_Trimmed()
    {
        Postcode postcode = Postcode.From("  SW1A 1AA  ");

        await Assert.That(postcode.Value).IsEqualTo("SW1A 1AA");
    }

    // Property tests

    [Test]
    public async Task Postcode_OutwardCode_ReturnsCorrectValue()
    {
        Postcode postcode = Postcode.From("SW1A 1AA");

        await Assert.That(postcode.OutwardCode).IsEqualTo("SW1A");
    }

    [Test]
    public async Task Postcode_InwardCode_ReturnsCorrectValue()
    {
        Postcode postcode = Postcode.From("SW1A 1AA");

        await Assert.That(postcode.InwardCode).IsEqualTo("1AA");
    }

    [Test]
    public async Task Postcode_Area_ReturnsCorrectValue()
    {
        Postcode postcode = Postcode.From("SW1A 1AA");

        await Assert.That(postcode.Area).IsEqualTo("SW");
    }

    [Test]
    public async Task Postcode_Area_SingleLetter_ReturnsCorrectValue()
    {
        Postcode postcode = Postcode.From("M1 1AE");

        await Assert.That(postcode.Area).IsEqualTo("M");
    }

    [Test]
    public async Task Postcode_District_WithSubDistrict_ReturnsWithoutSubDistrict()
    {
        Postcode postcode = Postcode.From("SW1A 1AA");

        await Assert.That(postcode.District).IsEqualTo("SW1");
    }

    [Test]
    public async Task Postcode_District_WithoutSubDistrict_ReturnsFullOutward()
    {
        Postcode postcode = Postcode.From("M60 1NW");

        await Assert.That(postcode.District).IsEqualTo("M60");
    }

    [Test]
    public async Task Postcode_Sector_ReturnsCorrectValue()
    {
        Postcode postcode = Postcode.From("SW1A 1AA");

        await Assert.That(postcode.Sector).IsEqualTo("SW1A 1");
    }

    // Invalid postcode tests

    [Test]
    public async Task Postcode_Empty_ThrowsException()
    {
        await Assert.That(() => { _ = Postcode.From(""); }).Throws<ValueObjectValidationException>();
    }

    [Test]
    public async Task Postcode_TooShort_ThrowsException()
    {
        await Assert.That(() => { _ = Postcode.From("M1 1A"); }).Throws<ValueObjectValidationException>();
    }

    [Test]
    public async Task Postcode_TooLong_ThrowsException()
    {
        await Assert.That(() => { _ = Postcode.From("SW1AA 1AAA"); }).Throws<ValueObjectValidationException>();
    }

    [Test]
    public async Task Postcode_InvalidFirstLetter_Q_ThrowsException()
    {
        // Q is not valid in first position
        await Assert.That(() => { _ = Postcode.From("Q1 1AA"); }).Throws<ValueObjectValidationException>();
    }

    [Test]
    public async Task Postcode_InvalidFirstLetter_V_ThrowsException()
    {
        // V is not valid in first position
        await Assert.That(() => { _ = Postcode.From("V1 1AA"); }).Throws<ValueObjectValidationException>();
    }

    [Test]
    public async Task Postcode_InvalidFirstLetter_X_ThrowsException()
    {
        // X is not valid in first position
        await Assert.That(() => { _ = Postcode.From("X1 1AA"); }).Throws<ValueObjectValidationException>();
    }

    [Test]
    public async Task Postcode_InvalidInwardLetter_C_ThrowsException()
    {
        // C is not valid in inward code
        await Assert.That(() => { _ = Postcode.From("M1 1AC"); }).Throws<ValueObjectValidationException>();
    }

    [Test]
    public async Task Postcode_InvalidInwardLetter_I_ThrowsException()
    {
        // I is not valid in inward code
        await Assert.That(() => { _ = Postcode.From("M1 1AI"); }).Throws<ValueObjectValidationException>();
    }

    [Test]
    public async Task Postcode_InvalidInwardLetter_K_ThrowsException()
    {
        // K is not valid in inward code
        await Assert.That(() => { _ = Postcode.From("M1 1AK"); }).Throws<ValueObjectValidationException>();
    }

    [Test]
    public async Task Postcode_InvalidInwardLetter_M_ThrowsException()
    {
        // M is not valid in inward code
        await Assert.That(() => { _ = Postcode.From("M1 1AM"); }).Throws<ValueObjectValidationException>();
    }

    [Test]
    public async Task Postcode_InvalidInwardLetter_O_ThrowsException()
    {
        // O is not valid in inward code
        await Assert.That(() => { _ = Postcode.From("M1 1AO"); }).Throws<ValueObjectValidationException>();
    }

    [Test]
    public async Task Postcode_InvalidInwardLetter_V_ThrowsException()
    {
        // V is not valid in inward code
        await Assert.That(() => { _ = Postcode.From("M1 1AV"); }).Throws<ValueObjectValidationException>();
    }

    [Test]
    public async Task Postcode_NumericOnly_ThrowsException()
    {
        await Assert.That(() => { _ = Postcode.From("12345"); }).Throws<ValueObjectValidationException>();
    }

    [Test]
    public async Task Postcode_LettersOnly_ThrowsException()
    {
        await Assert.That(() => { _ = Postcode.From("ABCDEF"); }).Throws<ValueObjectValidationException>();
    }

    // Equality tests

    [Test]
    public async Task Postcode_Equality_SamePostcode_AreEqual()
    {
        Postcode postcode1 = Postcode.From("SW1A 1AA");
        Postcode postcode2 = Postcode.From("sw1a1aa");

        await Assert.That(postcode1).IsEqualTo(postcode2);
    }

    [Test]
    public async Task Postcode_Equality_DifferentPostcodes_AreNotEqual()
    {
        Postcode postcode1 = Postcode.From("SW1A 1AA");
        Postcode postcode2 = Postcode.From("M1 1AE");

        await Assert.That(postcode1).IsNotEqualTo(postcode2);
    }
}
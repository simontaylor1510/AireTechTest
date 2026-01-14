using System.Text.RegularExpressions;

using Vogen;

namespace AireTechTest.Server.Domain;

/// <summary>
/// UK Postcode value object with validation per BS7666 standard.
/// </summary>
[ValueObject<string>]
public readonly partial struct Postcode
{
    // Regex pattern following BS7666 standard
    // Matches: A9 9AA, A99 9AA, A9A 9AA, AA9 9AA, AA99 9AA, AA9A 9AA, GIR 0AA
    private static readonly Regex PostcodeRegex = GeneratedPostcodeRegex();

    private static Validation Validate(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return Validation.Invalid("Postcode cannot be empty");
        }

        // Normalize: trim and convert to uppercase for validation
        string normalized = input.Trim().ToUpperInvariant();

        // Remove extra spaces and ensure single space between parts
        normalized = Regex.Replace(normalized, @"\s+", " ");

        // Check length (5-8 characters including optional space)
        string withoutSpace = normalized.Replace(" ", "");
        if (withoutSpace.Length is < 5 or > 7)
        {
            return Validation.Invalid("Postcode must be between 5 and 7 characters (excluding space)");
        }

        // Validate against regex pattern
        return !PostcodeRegex.IsMatch(normalized) ? Validation.Invalid("Postcode format is invalid") : Validation.Ok;
    }

    private static string NormalizeInput(string input)
    {
        // Normalize to uppercase with single space separator
        string normalized = input.Trim().ToUpperInvariant();
        normalized = Regex.Replace(normalized, @"\s+", "");

        // Insert space before inward code (last 3 characters)
        if (normalized.Length >= 5)
        {
            normalized = normalized[..^3] + " " + normalized[^3..];
        }

        return normalized;
    }

    /// <summary>
    /// Gets the outward code (first part before the space).
    /// </summary>
    public string OutwardCode => Value.Split(' ')[0];

    /// <summary>
    /// Gets the inward code (second part after the space).
    /// </summary>
    public string InwardCode => Value.Split(' ')[1];

    /// <summary>
    /// Gets the postcode area (1-2 letter prefix).
    /// </summary>
    public string Area
    {
        get
        {
            string outward = OutwardCode;
            int letterCount = outward.TakeWhile(char.IsLetter).Count();
            return outward[..letterCount];
        }
    }

    /// <summary>
    /// Gets the postcode district (outward code without sub-district letter).
    /// </summary>
    public string District
    {
        get
        {
            string outward = OutwardCode;
            // Remove trailing letter if present (sub-district indicator)
            if (outward.Length >= 3 && char.IsLetter(outward[^1]) && char.IsDigit(outward[^2]))
            {
                return outward[..^1];
            }

            return outward;
        }
    }

    /// <summary>
    /// Gets the postcode sector (district + first digit of inward code).
    /// </summary>
    public string Sector => $"{OutwardCode} {InwardCode[0]}";

    [GeneratedRegex(
        @"^(GIR\s*0AA|[A-PR-UWYZ]([0-9][0-9]?|[A-HK-Y][0-9][0-9]?|[0-9][A-HJKSTUW]|[A-HK-Y][0-9][ABEHMNPRVWXY])\s*[0-9][ABD-HJLNP-UW-Z]{2})$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-GB")]
    private static partial Regex GeneratedPostcodeRegex();
}
using Vogen;

namespace AireTechTest.Server.Domain;

[ValueObject<string>]
public readonly partial struct NhsNumber
{
    private static Validation Validate(string input)
    {
        // Remove any spaces/hyphens for flexibility
        string digits = new(input.Where(char.IsDigit).ToArray());

        // Must be exactly 10 digits
        if (digits.Length != 10)
        {
            return Validation.Invalid("NHS Number must be exactly 10 digits");
        }

        // Reject all-same-digit numbers
        if (digits.Distinct().Count() == 1)
        {
            return Validation.Invalid("NHS Number cannot have all identical digits");
        }

        // Modulus 11 check digit validation
        int sum = 0;
        for (int i = 0; i < 9; i++)
        {
            sum += (digits[i] - '0') * (10 - i);
        }

        int remainder = sum % 11;
        int expectedCheckDigit = 11 - remainder;

        // Check digit of 11 means the actual digit should be 0
        if (expectedCheckDigit == 11)
        {
            expectedCheckDigit = 0;
        }

        // Check digit of 10 means this combination is invalid
        if (expectedCheckDigit == 10)
        {
            return Validation.Invalid("NHS Number has invalid check digit (modulus 10)");
        }

        int actualCheckDigit = digits[9] - '0';
        if (expectedCheckDigit != actualCheckDigit)
        {
            return Validation.Invalid($"NHS Number check digit is invalid. Expected {expectedCheckDigit}, got {actualCheckDigit}");
        }

        return Validation.Ok;
    }

    private static string NormalizeInput(string input)
    {
        // Store as digits only (no spaces/hyphens)
        return new string(input.Where(char.IsDigit).ToArray());
    }
}
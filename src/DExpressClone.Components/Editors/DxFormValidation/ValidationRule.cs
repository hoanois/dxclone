using System.Text.RegularExpressions;

namespace DExpressClone.Components.Editors.DxFormValidation;

/// <summary>
/// Base class for form validation rules.
/// </summary>
public abstract class ValidationRule
{
    /// <summary>
    /// Error message to display when validation fails.
    /// </summary>
    public string ErrorMessage { get; set; } = "This field is invalid.";

    /// <summary>
    /// Validates the given value.
    /// </summary>
    public abstract bool Validate(object? value);
}

/// <summary>
/// Validates that a value is not null or empty.
/// </summary>
public class RequiredRule : ValidationRule
{
    public RequiredRule() => ErrorMessage = "This field is required.";

    public override bool Validate(object? value) => value switch
    {
        null => false,
        string s => !string.IsNullOrWhiteSpace(s),
        _ => true
    };
}

/// <summary>
/// Validates that a string value matches a valid email format.
/// </summary>
public class EmailRule : ValidationRule
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public EmailRule() => ErrorMessage = "Please enter a valid email address.";

    public override bool Validate(object? value)
    {
        if (value is not string s || string.IsNullOrWhiteSpace(s)) return true; // empty handled by RequiredRule
        return EmailRegex.IsMatch(s);
    }
}

/// <summary>
/// Validates that a string value matches a custom regex pattern.
/// </summary>
public class RegexRule : ValidationRule
{
    public string Pattern { get; set; } = "";

    public RegexRule() => ErrorMessage = "The value does not match the required pattern.";

    public override bool Validate(object? value)
    {
        if (value is not string s || string.IsNullOrWhiteSpace(s)) return true;
        return Regex.IsMatch(s, Pattern);
    }
}

/// <summary>
/// Validates that a value matches another value (e.g., confirm password).
/// </summary>
public class MatchRule : ValidationRule
{
    public Func<object?> MatchValue { get; set; } = () => null;

    public MatchRule() => ErrorMessage = "The values do not match.";

    public override bool Validate(object? value)
    {
        var other = MatchValue();
        if (value is null && other is null) return true;
        return Equals(value, other);
    }
}

/// <summary>
/// Validates that a string has a minimum length.
/// </summary>
public class MinLengthRule : ValidationRule
{
    public int Length { get; set; }

    public MinLengthRule() => ErrorMessage = "The value is too short.";

    public override bool Validate(object? value)
    {
        if (value is not string s || string.IsNullOrEmpty(s)) return true;
        return s.Length >= Length;
    }
}

/// <summary>
/// Validates that a string does not exceed a maximum length.
/// </summary>
public class MaxLengthRule : ValidationRule
{
    public int Length { get; set; }

    public MaxLengthRule() => ErrorMessage = "The value is too long.";

    public override bool Validate(object? value)
    {
        if (value is not string s) return true;
        return s.Length <= Length;
    }
}

/// <summary>
/// Validates that a numeric value falls within a specified range.
/// </summary>
public class RangeRule : ValidationRule
{
    public double Min { get; set; } = double.MinValue;
    public double Max { get; set; } = double.MaxValue;

    public RangeRule() => ErrorMessage = "The value is out of range.";

    public override bool Validate(object? value)
    {
        if (value is null) return true;
        try
        {
            var num = Convert.ToDouble(value);
            return num >= Min && num <= Max;
        }
        catch
        {
            return false;
        }
    }
}

/// <summary>
/// Validates using a custom function.
/// </summary>
public class CustomRule : ValidationRule
{
    public Func<object?, bool> ValidateFunc { get; set; } = _ => true;

    public override bool Validate(object? value) => ValidateFunc(value);
}

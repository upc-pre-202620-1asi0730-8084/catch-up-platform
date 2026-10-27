using System.Text.RegularExpressions;

namespace CatchUpPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions;

/// <summary>
///     Provides extension methods for string transformations in ASP.NET routing configuration.
/// </summary>
public static partial class StringExtensions
{
    [GeneratedRegex("(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])", RegexOptions.Compiled)]
    private static partial Regex KebabCaseRegex();

    /// <summary>
    ///     Converts a PascalCase string into lowercase kebab-case.
    /// </summary>
    /// <param name="text">The string to convert.</param>
    /// <returns>The kebab-case formatted string.</returns>
    public static string ToKebabCase(this string text)
    {
        return string.IsNullOrEmpty(text) 
            ? text 
            : KebabCaseRegex().Replace(text, "-$1").Trim().ToLower();
    }
    
}
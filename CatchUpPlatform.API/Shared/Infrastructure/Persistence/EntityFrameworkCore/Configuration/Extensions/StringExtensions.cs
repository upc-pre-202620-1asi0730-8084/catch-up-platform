using Humanizer;

namespace CatchUpPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

/// <summary>
///     Provides extension methods for string manipulation used in database naming conventions.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    ///     Converts a PascalCase or camelCase string into a snake_case string.
    /// </summary>
    /// <param name="text">The string to convert.</param>
    /// <returns>The snake_case formatted string.</returns>
    public static string ToSnakeCase(this string text)
    {
        return new string(Convert(text.GetEnumerator()).ToArray());
        
        static IEnumerable<char> Convert(CharEnumerator e)
        {
            if (!e.MoveNext()) yield break;
            yield return char.ToLower(e.Current);
            while (e.MoveNext())
                if (char.IsUpper(e.Current))
                {
                    yield return '_';
                    yield return char.ToLower(e.Current);
                }
                else
                {
                    yield return e.Current;
                }
        }
    }

    /// <summary>
    ///     Pluralizes the specified singular noun string using Humanizer.
    /// </summary>
    /// <param name="text">The string to pluralize.</param>
    /// <returns>The pluralized string.</returns>
    public static string ToPlural(this string text)
    {
        return text.Pluralize(false);
    }
}
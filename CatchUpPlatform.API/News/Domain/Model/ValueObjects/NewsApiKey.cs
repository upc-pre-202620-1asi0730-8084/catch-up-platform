namespace CatchUpPlatform.API.News.Domain.Model.ValueObjects;

/// <summary>
///     Value object representing a News API key identifier used to authenticate requests with the news provider.
/// </summary>
public sealed record NewsApiKey
{
    private const int MaxLength = 255;
    
    /// <summary>
    ///     Gets the underlying string value of the News API key.
    /// </summary>
    public string Value { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="NewsApiKey"/> value object.
    /// </summary>
    /// <param name="value">The raw News API key string.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is null, empty, whitespace, or exceeds maximum length.</exception>
    public NewsApiKey(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("NewsApiKey cannot be null, empty, or whitespace.", nameof(value));
        if (value.Length > MaxLength)
            throw new ArgumentException($"NewsApiKey cannot exceed {MaxLength} characters.", nameof(value));
        Value = value;
    }
    
    /// <summary>
    ///     Returns the underlying string value of the News API key.
    /// </summary>
    /// <returns>The API key string.</returns>
    public override string ToString() => Value;

    /// <summary>
    ///     Masks a raw News API key string for safe diagnostic logging, retaining only the last 4 characters.
    /// </summary>
    /// <param name="value">The raw API key string to mask.</param>
    /// <returns>The masked API key string.</returns>
    public static string MaskedForLogging(string? value) => string.IsNullOrEmpty(value)
    ? string.Empty
    : value.Length <= 4
        ? new string('*', value.Length)
        : new string('*', value.Length - 4) + value[^4..];
    
    /// <summary>
    ///     Returns a masked representation of this News API key instance for safe diagnostic logging.
    /// </summary>
    /// <returns>The masked API key string.</returns>
    public string MaskedForLogging() => MaskedForLogging(Value);
    
}
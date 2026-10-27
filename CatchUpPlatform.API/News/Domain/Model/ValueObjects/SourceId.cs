namespace CatchUpPlatform.API.News.Domain.Model.ValueObjects;

/// <summary>
///     Value object representing a unique news source identifier provided by the news service.
/// </summary>
public sealed record SourceId
{
    private const int MaxLength = 255;
    
    /// <summary>
    ///     Gets the underlying string value of the source identifier.
    /// </summary>
    public string Value { get; }
    
    /// <summary>
    ///     Initializes a new instance of the <see cref="SourceId"/> value object.
    /// </summary>
    /// <param name="value">The raw source identifier string.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is null, empty, whitespace, or exceeds maximum length.</exception>
    public SourceId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("SourceId cannot be null, empty, or whitespace.", nameof(value));
        if (value.Length > MaxLength)
            throw new ArgumentException($"SourceId cannot exceed {MaxLength} characters.", nameof(value));
        Value = value;
    }
    
    /// <summary>
    ///     Returns the underlying string value of the source identifier.
    /// </summary>
    /// <returns>The source identifier string.</returns>
    public override string ToString() => Value;
}
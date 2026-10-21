namespace CatchUpPlatform.API.News.Domain.Model.ValueObjects;

public sealed record NewsApiKey
{
    private const int MaxLength = 255;
    
    public string Value { get; }

    public NewsApiKey(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("NewsApiKey cannot be null, empty, or whitespace.", nameof(value));
        if (value.Length > MaxLength)
            throw new ArgumentException($"NewsApiKey cannot exceed {MaxLength} characters.", nameof(value));
        Value = value;
    }
    
    public override string ToString() => Value;

    public static string MaskedForLogging(string? value) => string.IsNullOrEmpty(value)
    ? string.Empty
    : value.Length <= 4
        ? new string('*', value.Length)
        : new string('*', value.Length - 4) + value[^4..];
    
    public string MaskedForLogging() => MaskedForLogging(Value);
    
}
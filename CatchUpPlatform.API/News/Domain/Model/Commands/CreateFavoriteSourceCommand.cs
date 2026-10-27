using CatchUpPlatform.API.News.Domain.Model.ValueObjects;

namespace CatchUpPlatform.API.News.Domain.Model.Commands;

/// <summary>
///     Domain command to create a new favorite news source association.
/// </summary>
/// <param name="NewsApiKey">The validated News API key value object identifying the consumer.</param>
/// <param name="SourceId">The validated source identifier value object representing the news source.</param>
public record CreateFavoriteSourceCommand(NewsApiKey NewsApiKey, SourceId SourceId);
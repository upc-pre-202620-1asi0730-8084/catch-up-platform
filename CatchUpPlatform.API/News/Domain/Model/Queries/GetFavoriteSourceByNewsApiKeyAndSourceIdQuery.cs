using CatchUpPlatform.API.News.Domain.Model.ValueObjects;

namespace CatchUpPlatform.API.News.Domain.Model.Queries;

/// <summary>
///     Domain query to retrieve a specific favorite source by the composite pair of News API key and source identifier.
/// </summary>
/// <param name="NewsApiKey">The validated News API key value object identifying the consumer.</param>
/// <param name="SourceId">The validated source identifier value object representing the news source.</param>
public record GetFavoriteSourceByNewsApiKeyAndSourceIdQuery(NewsApiKey NewsApiKey, SourceId SourceId);
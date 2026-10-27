namespace CatchUpPlatform.API.News.Interfaces.Rest.Resources;

/// <summary>
///     Resource representing a favorite news source association returned to clients.
/// </summary>
/// <param name="Id">The unique identifier of the favorite source.</param>
/// <param name="NewsApiKey">The News API key identifying the consumer.</param>
/// <param name="SourceId">The news source identifier from the news provider.</param>
public record FavoriteSourceResource(
    int Id,
    string NewsApiKey,
    string SourceId);
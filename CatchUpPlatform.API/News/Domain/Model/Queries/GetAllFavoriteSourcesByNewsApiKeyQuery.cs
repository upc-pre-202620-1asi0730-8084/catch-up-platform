using CatchUpPlatform.API.News.Domain.Model.ValueObjects;

namespace CatchUpPlatform.API.News.Domain.Model.Queries;

/// <summary>
///     Domain query to retrieve all favorite sources associated with a given News API key.
/// </summary>
/// <param name="NewsApiKey">The validated News API key value object identifying the consumer.</param>
public record GetAllFavoriteSourcesByNewsApiKeyQuery(NewsApiKey NewsApiKey);
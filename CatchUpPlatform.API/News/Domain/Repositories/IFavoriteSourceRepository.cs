using CatchUpPlatform.API.News.Domain.Model.Aggregates;
using CatchUpPlatform.API.News.Domain.Model.ValueObjects;
using CatchUpPlatform.API.Shared.Domain.Repositories;

namespace CatchUpPlatform.API.News.Domain.Repositories;

/// <summary>
///     Domain repository interface for managing <see cref="FavoriteSource"/> aggregates.
/// </summary>
public interface IFavoriteSourceRepository : IBaseRepository<FavoriteSource>
{
    /// <summary>
    ///     Finds all favorite sources associated with the given News API key asynchronously.
    /// </summary>
    /// <param name="newsApiKey">The News API key identifying the consumer.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task containing the collection of favorite sources matching the API key.</returns>
    Task<IEnumerable<FavoriteSource>> FindByNewsApiKeyAsync(NewsApiKey newsApiKey, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Finds a specific favorite source by the composite pair of News API key and source identifier asynchronously.
    /// </summary>
    /// <param name="newsApiKey">The News API key identifying the consumer.</param>
    /// <param name="sourceId">The news source identifier.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task containing the matching favorite source if found; otherwise, <see langword="null"/>.</returns>
    Task<FavoriteSource?> FindByNewsApiKeyAndSourceIdAsync(NewsApiKey newsApiKey, SourceId sourceId, CancellationToken cancellationToken = default);
}
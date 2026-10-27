using CatchUpPlatform.API.News.Domain.Model.Aggregates;
using CatchUpPlatform.API.News.Domain.Model.Queries;

namespace CatchUpPlatform.API.News.Application.Services;

/// <summary>
///     Application query service interface for handling favorite source read operations.
/// </summary>
public interface IFavoriteSourceQueryService
{
    /// <summary>
    ///     Retrieves all favorite sources associated with a News API key asynchronously.
    /// </summary>
    /// <param name="query">The query containing the News API key.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task containing the collection of matching favorite sources.</returns>
    Task<IEnumerable<FavoriteSource>> Handle(GetAllFavoriteSourcesByNewsApiKeyQuery query, CancellationToken cancellationToken);

    /// <summary>
    ///     Retrieves a favorite source by its unique identifier asynchronously.
    /// </summary>
    /// <param name="query">The query containing the favorite source ID.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task containing the favorite source if found; otherwise, <see langword="null"/>.</returns>
    Task<FavoriteSource?> Handle(GetFavoriteSourceByIdQuery query, CancellationToken cancellationToken);

    /// <summary>
    ///     Retrieves a favorite source by the composite pair of News API key and source identifier asynchronously.
    /// </summary>
    /// <param name="query">The query containing the News API key and source identifier.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task containing the matching favorite source if found; otherwise, <see langword="null"/>.</returns>
    Task<FavoriteSource?> Handle(GetFavoriteSourceByNewsApiKeyAndSourceIdQuery query, CancellationToken cancellationToken);
}
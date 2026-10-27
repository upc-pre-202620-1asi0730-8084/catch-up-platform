using CatchUpPlatform.API.News.Application.Services;
using CatchUpPlatform.API.News.Domain.Model.Aggregates;
using CatchUpPlatform.API.News.Domain.Model.Queries;
using CatchUpPlatform.API.News.Domain.Repositories;

namespace CatchUpPlatform.API.News.Application.Internal.QueryServices;

/// <summary>
///     Application query service that implements <see cref="IFavoriteSourceQueryService"/> to coordinate favorite source queries.
/// </summary>
/// <param name="favoriteSourceRepository">The favorite source repository.</param>
public class FavoriteSourceQueryService(IFavoriteSourceRepository favoriteSourceRepository) : IFavoriteSourceQueryService
{
    /// <inheritdoc />
    public async Task<IEnumerable<FavoriteSource>> Handle(GetAllFavoriteSourcesByNewsApiKeyQuery query, CancellationToken cancellationToken)
    {
        return await favoriteSourceRepository.FindByNewsApiKeyAsync(query.NewsApiKey, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<FavoriteSource?> Handle(GetFavoriteSourceByIdQuery query, CancellationToken cancellationToken)
    {
        return await favoriteSourceRepository.FindByIdAsync(query.Id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<FavoriteSource?> Handle(GetFavoriteSourceByNewsApiKeyAndSourceIdQuery query, CancellationToken cancellationToken)
    {
        return await favoriteSourceRepository.FindByNewsApiKeyAndSourceIdAsync(query.NewsApiKey, query.SourceId, cancellationToken);
    }
}
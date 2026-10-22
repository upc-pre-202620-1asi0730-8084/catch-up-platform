using CatchUpPlatform.API.News.Application.Services;
using CatchUpPlatform.API.News.Domain.Model.Aggregate;
using CatchUpPlatform.API.News.Domain.Model.Queries;
using CatchUpPlatform.API.News.Domain.Repositories;

namespace CatchUpPlatform.API.News.Application.Internal.QueryServices;

public class FavoriteSourceQueryService(IFavoriteSourceRepository favoriteSourceRepository) : IFavoriteSourceQueryService
{
    public async Task<IEnumerable<FavoriteSource>> Handle(GetAllFavoriteSourcesByNewsApiKeyQuery query, CancellationToken cancellationToken)
    {
        return await favoriteSourceRepository.FindByNewsApiKeyAsync(query.NewsApiKey, cancellationToken);
    }

    public async Task<FavoriteSource?> Handle(GetFavoriteSourceByIdQuery query, CancellationToken cancellationToken)
    {
        return await favoriteSourceRepository.FindByIdAsync(query.Id, cancellationToken);
    }

    public async Task<FavoriteSource?> Handle(GetFavoriteSourceByNewsApiKeyAndSourceIdQuery query, CancellationToken cancellationToken)
    {
        return await favoriteSourceRepository.FindByNewsApiKeyAndSourceIdAsync(query.NewsApiKey, query.SourceId, cancellationToken);
    }
}
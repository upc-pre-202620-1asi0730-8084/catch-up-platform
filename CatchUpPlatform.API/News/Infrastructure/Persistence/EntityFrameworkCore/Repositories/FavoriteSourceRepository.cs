using CatchUpPlatform.API.News.Domain.Model.Aggregates;
using CatchUpPlatform.API.News.Domain.Model.ValueObjects;
using CatchUpPlatform.API.News.Domain.Repositories;
using CatchUpPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using CatchUpPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CatchUpPlatform.API.News.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     Repository implementation for <see cref="FavoriteSource"/> aggregates using Entity Framework Core.
/// </summary>
/// <param name="context">The application database context.</param>
public class FavoriteSourceRepository(AppDbContext context)
    : BaseRepository<FavoriteSource>(context), IFavoriteSourceRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<FavoriteSource>> FindByNewsApiKeyAsync(NewsApiKey newsApiKey,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<FavoriteSource>()
            .Where(f => f.NewsApiKey == newsApiKey)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<FavoriteSource?> FindByNewsApiKeyAndSourceIdAsync(NewsApiKey newsApiKey, SourceId sourceId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<FavoriteSource>()
            .FirstOrDefaultAsync(f => f.NewsApiKey == newsApiKey && f.SourceId == sourceId, cancellationToken);
    }
}
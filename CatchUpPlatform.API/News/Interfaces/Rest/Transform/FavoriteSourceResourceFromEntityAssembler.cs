using CatchUpPlatform.API.News.Domain.Model.Aggregates;
using CatchUpPlatform.API.News.Interfaces.Rest.Resources;

namespace CatchUpPlatform.API.News.Interfaces.Rest.Transform;

/// <summary>
///     Assembles HTTP response resources from <see cref="FavoriteSource"/> domain entities.
/// </summary>
public static class FavoriteSourceResourceFromEntityAssembler
{
    /// <summary>
    ///     Transforms a <see cref="FavoriteSource"/> entity into a <see cref="FavoriteSourceResource"/>.
    /// </summary>
    /// <param name="entity">The favorite source aggregate root entity.</param>
    /// <returns>A resource representation suitable for HTTP responses.</returns>
    public static FavoriteSourceResource ToResourceFromEntity(FavoriteSource entity)
    {
        return new FavoriteSourceResource(
            entity.Id,
            entity.NewsApiKey.Value,
            entity.SourceId.Value
        );
    }
}
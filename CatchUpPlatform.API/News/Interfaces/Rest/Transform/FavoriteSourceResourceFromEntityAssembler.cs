using CatchUpPlatform.API.News.Domain.Model.Aggregates;
using CatchUpPlatform.API.News.Interfaces.Rest.Resources;

namespace CatchUpPlatform.API.News.Interfaces.Rest.Transform;

public static class FavoriteSourceResourceFromEntityAssembler
{
    public static FavoriteSourceResource ToResourceFromEntity(FavoriteSource entity)
    {
        return new FavoriteSourceResource(
            entity.Id,
            entity.NewsApiKey.Value,
            entity.SourceId.Value
        );
    }
}
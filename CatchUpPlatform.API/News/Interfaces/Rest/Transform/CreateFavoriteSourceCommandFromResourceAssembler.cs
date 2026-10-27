using CatchUpPlatform.API.News.Domain.Model.Commands;
using CatchUpPlatform.API.News.Domain.Model.ValueObjects;
using CatchUpPlatform.API.News.Interfaces.Rest.Resources;

namespace CatchUpPlatform.API.News.Interfaces.Rest.Transform;

/// <summary>
///     Assembles domain commands from REST creation resources.
/// </summary>
public static class CreateFavoriteSourceCommandFromResourceAssembler
{
    /// <summary>
    ///     Transforms a <see cref="CreateFavoriteSourceResource"/> into a <see cref="CreateFavoriteSourceCommand"/>.
    /// </summary>
    /// <param name="resource">The HTTP resource containing creation parameters.</param>
    /// <returns>The domain command instance with validated value objects.</returns>
    public static CreateFavoriteSourceCommand ToCommandFromResource(CreateFavoriteSourceResource resource)
    {
        return new CreateFavoriteSourceCommand(new NewsApiKey(resource.NewsApiKey), new SourceId(resource.SourceId));
    }
}
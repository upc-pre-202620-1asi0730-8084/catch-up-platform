using CatchUpPlatform.API.News.Domain.Model.Commands;
using CatchUpPlatform.API.News.Domain.Model.ValueObjects;

namespace CatchUpPlatform.API.News.Domain.Model.Aggregates;

/// <summary>
///     Aggregate root representing a user's favorite news source association.
/// </summary>
public partial class FavoriteSource
{
    /// <summary>
    ///     Gets the unique identifier of the favorite source aggregate.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    ///     Gets the News API key associated with this favorite source.
    /// </summary>
    public NewsApiKey NewsApiKey { get; private set; }

    /// <summary>
    ///     Gets the source identifier of the news provider.
    /// </summary>
    public SourceId SourceId { get; private set; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="FavoriteSource"/> class for Entity Framework Core.
    /// </summary>
    public FavoriteSource()
    {
        NewsApiKey = null!;
        SourceId = null!;
    }
    
    /// <summary>
    ///     Initializes a new instance of the <see cref="FavoriteSource"/> aggregate from a creation command.
    /// </summary>
    /// <param name="command">The command containing the News API key and source identifier.</param>
    public FavoriteSource(CreateFavoriteSourceCommand command)
    {
        NewsApiKey = command.NewsApiKey;
        SourceId = command.SourceId;
    }
}
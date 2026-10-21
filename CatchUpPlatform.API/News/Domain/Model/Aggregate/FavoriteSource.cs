using CatchUpPlatform.API.News.Domain.Model.Commands;
using CatchUpPlatform.API.News.Domain.Model.ValueObjects;

namespace CatchUpPlatform.API.News.Domain.Model.Aggregate;

public partial class FavoriteSource
{
    public int Id { get; private set; }
    public NewsApiKey NewsApiKey { get; private set; }
    public SourceId SourceId { get; private set; }

    public FavoriteSource()
    {
        NewsApiKey = null!;
        SourceId = null!;
    }
    
    public FavoriteSource(CreateFavoriteSourceCommand command)
    {
        NewsApiKey = command.NewsApiKey;
        SourceId = command.SourceId;
    }
}
namespace CatchUpPlatform.API.News.Interfaces.Rest.Resources;

public record FavoriteSourceResource(
    int Id,
    string NewsApiKey,
    string SourceId);
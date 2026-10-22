namespace CatchUpPlatform.API.News.Applications.Errors;

public enum ECreateFavoriteSourceError
{
    InvalidNewsApiKey,
    InvalidSourceId,
    DuplicateFavoriteSource,
    UnexpectedError
}
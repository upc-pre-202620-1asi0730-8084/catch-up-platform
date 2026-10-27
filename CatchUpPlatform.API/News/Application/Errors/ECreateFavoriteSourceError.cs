namespace CatchUpPlatform.API.News.Application.Errors;

/// <summary>
///     Represents domain and application error states that may occur during favorite source creation.
/// </summary>
public enum ECreateFavoriteSourceError
{
    /// <summary>
    ///     The provided News API key is invalid or malformed.
    /// </summary>
    InvalidNewsApiKey,

    /// <summary>
    ///     The provided source identifier is invalid or malformed.
    /// </summary>
    InvalidSourceId,

    /// <summary>
    ///     A favorite source with the same News API key and source identifier already exists.
    /// </summary>
    DuplicateFavoriteSource,

    /// <summary>
    ///     An unexpected persistence or system error occurred while processing the request.
    /// </summary>
    UnexpectedError
}
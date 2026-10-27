namespace CatchUpPlatform.API.News.Domain.Model.Queries;

/// <summary>
///     Domain query to retrieve a favorite source by its unique database identifier.
/// </summary>
/// <param name="Id">The unique identifier of the favorite source.</param>
public record GetFavoriteSourceByIdQuery(int Id);
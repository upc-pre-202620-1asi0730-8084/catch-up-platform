using CatchUpPlatform.API.News.Application.Errors;
using CatchUpPlatform.API.News.Domain.Model.Aggregates;
using CatchUpPlatform.API.News.Domain.Model.Commands;
using CatchUpPlatform.API.Shared.Application.Patterns;

namespace CatchUpPlatform.API.News.Application.Services;

/// <summary>
///     Application command service interface for handling favorite source write operations.
/// </summary>
public interface IFavoriteSourceCommandService
{
    /// <summary>
    ///     Handles the creation of a new favorite source association asynchronously.
    /// </summary>
    /// <param name="command">The command containing creation parameters.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A result containing the created aggregate root on success, or an error enum on failure.</returns>
    Task<Result<FavoriteSource, ECreateFavoriteSourceError>> Handle(CreateFavoriteSourceCommand command, CancellationToken cancellationToken);
}
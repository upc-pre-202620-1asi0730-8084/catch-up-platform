namespace CatchUpPlatform.API.Shared.Domain.Repositories;

/// <summary>
///     Defines the contract for the Unit of Work pattern to atomically commit changes across repositories.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    ///     Persists all pending changes to the underlying database asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous save operation.</returns>
    Task CompleteAsync(CancellationToken cancellationToken = default);
}
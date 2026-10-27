using CatchUpPlatform.API.News.Application.Errors;
using CatchUpPlatform.API.News.Application.Services;
using CatchUpPlatform.API.News.Domain.Model.Aggregates;
using CatchUpPlatform.API.News.Domain.Model.Commands;
using CatchUpPlatform.API.News.Domain.Repositories;
using CatchUpPlatform.API.Shared.Application.Patterns;
using CatchUpPlatform.API.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CatchUpPlatform.API.News.Application.Internal.CommandServices;

public class FavoriteSourceCommandService(
    IFavoriteSourceRepository favoriteSourceRepository,
    IUnitOfWork unitOfWork,
    ILogger<FavoriteSourceCommandService> logger) : IFavoriteSourceCommandService
{

    private static bool IsDuplicateKeyViolation(DbUpdateException exception)
    {
        for (Exception? current = exception; current is not null; current = current.InnerException)
        {
            if(!string.Equals(current.GetType().Name, "MySqlException", StringComparison.Ordinal)) continue;
            var numberProperty = current.GetType().GetProperty("Number");
            if (numberProperty?.PropertyType == typeof(int) &&
                numberProperty.GetValue(current) is int errorCode && errorCode == 1062) return true;
        }
        return false;
    }
    
    public async Task<Result<FavoriteSource, ECreateFavoriteSourceError>> Handle(CreateFavoriteSourceCommand command, CancellationToken cancellationToken)
    {
        var existingSource = await favoriteSourceRepository.FindByNewsApiKeyAndSourceIdAsync(command.NewsApiKey, command.SourceId, cancellationToken);
        if (existingSource is not null)
        {
            logger.LogWarning("Duplicate favorite source rejected for NewsApiKey {NewsApiKey} and SourceId {SourceId}", command.NewsApiKey.MaskedForLogging(), command.SourceId);
            return new Result<FavoriteSource, ECreateFavoriteSourceError>.Failure(ECreateFavoriteSourceError.DuplicateFavoriteSource);
        }

        try
        {
            var favoriteSource = new FavoriteSource(command);
            await favoriteSourceRepository.AddAsync(favoriteSource, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<FavoriteSource, ECreateFavoriteSourceError>.Success(favoriteSource);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex,
                "Invalid arguments while creating favorite source for NewsApiKey {NewsApiKey} and SourceId {SourceId}",
                command.NewsApiKey.MaskedForLogging(),
                command.SourceId);
            return new Result<FavoriteSource, ECreateFavoriteSourceError>.Failure(ECreateFavoriteSourceError
                .UnexpectedError);
        }
        catch (DbUpdateException ex) when (IsDuplicateKeyViolation(ex))
        {
            logger.LogWarning(ex,
                "Duplicate key violation creating favorite source for NewsApiKey {NewsApiKey} and SourceId {SourceId}",
                command.NewsApiKey.MaskedForLogging(),
                command.SourceId);
            return new Result<FavoriteSource, ECreateFavoriteSourceError>.Failure(ECreateFavoriteSourceError
                .DuplicateFavoriteSource);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex,
                "Database update failed creating favorite source for NewsApiKey {NewsApiKey} and SourceId {SourceId}",
                command.NewsApiKey.MaskedForLogging(),
                command.SourceId);
            return new Result<FavoriteSource, ECreateFavoriteSourceError>.Failure(ECreateFavoriteSourceError
                .UnexpectedError);
        }

        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Unexpected error creating favorite source for NewsApiKey {NewsApiKey} and SourceId {SourceId}",
                command.NewsApiKey.MaskedForLogging(),
                command.SourceId);
            return new Result<FavoriteSource, ECreateFavoriteSourceError>.Failure(ECreateFavoriteSourceError
                .UnexpectedError);
        }
    }
}
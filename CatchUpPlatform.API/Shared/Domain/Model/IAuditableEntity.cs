namespace CatchUpPlatform.API.Shared.Domain.Model;

/// <summary>
///     Defines the contract for domain entities that track audit timestamps.
/// </summary>
public interface IAuditableEntity
{
    /// <summary>
    ///     Gets or sets the timestamp when the entity was created.
    /// </summary>
    DateTimeOffset? CreatedAt { get; set; }

    /// <summary>
    ///     Gets or sets the timestamp when the entity was last updated.
    /// </summary>
    DateTimeOffset? UpdatedAt { get; set; }
}
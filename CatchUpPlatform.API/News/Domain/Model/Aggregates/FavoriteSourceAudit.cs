using CatchUpPlatform.API.Shared.Domain.Model;

namespace CatchUpPlatform.API.News.Domain.Model.Aggregates;

public partial class FavoriteSource : IAuditableEntity
{
    /// <inheritdoc />
    public DateTimeOffset? CreatedAt { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? UpdatedAt { get; set; }
}
using CatchUpPlatform.API.Shared.Domain.Model;

namespace CatchUpPlatform.API.News.Domain.Model.Aggregate;

public partial class FavoriteSource : IAuditableEntity
{
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace CatchUpPlatform.API.News.Interfaces.Rest.Resources;

public record CreateFavoriteSourceResource(
    [Required(ErrorMessage = "RequiredNewsApiKey")]
    string NewsApiKey,
    [Required(ErrorMessage = "RequiredSourceId")]
    string SourceId);
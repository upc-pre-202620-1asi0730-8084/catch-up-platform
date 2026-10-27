using System.ComponentModel.DataAnnotations;

namespace CatchUpPlatform.API.News.Interfaces.Rest.Resources;

/// <summary>
///     Resource representing the payload to create a new favorite news source.
/// </summary>
/// <param name="NewsApiKey">The News API key identifying the consumer.</param>
/// <param name="SourceId">The news source identifier from the news provider.</param>
public record CreateFavoriteSourceResource(
    [Required(ErrorMessage = "RequiredNewsApiKey")]
    string NewsApiKey,
    [Required(ErrorMessage = "RequiredSourceId")]
    string SourceId);
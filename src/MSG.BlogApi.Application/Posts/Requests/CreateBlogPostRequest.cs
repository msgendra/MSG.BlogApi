using System.ComponentModel.DataAnnotations;

namespace MSG.BlogApi.Application.Posts.Requests;

public sealed class CreateBlogPostRequest
{
    [Required, MaxLength(200)]
    public string Title { get; init; } = string.Empty;

    [Required, MaxLength(10000)]
    public string Content { get; init; } = string.Empty;
}

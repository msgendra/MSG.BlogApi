using System.ComponentModel.DataAnnotations;

namespace MSG.BlogApi.Application.Posts.Requests;

public sealed class CreateCommentRequest
{
    [MaxLength(100)]
    public string? Author { get; init; }

    [Required, MaxLength(2000)]
    public string Content { get; init; } = string.Empty;
}

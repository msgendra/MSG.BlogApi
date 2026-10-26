namespace MSG.BlogApi.Application.Posts.Responses;

public sealed class CommentDto
{
    public int Id { get; init; }
    public string? Author { get; init; }
    public string Content { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}

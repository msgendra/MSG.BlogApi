namespace MSG.BlogApi.Application.Posts.Responses;

public sealed class BlogPostDto
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public IReadOnlyList<CommentDto> Comments { get; init; } = [];
}

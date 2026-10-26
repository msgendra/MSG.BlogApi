namespace MSG.BlogApi.Application.Posts.Responses;

public sealed class BlogPostListItemDto
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public int CommentCount { get; init; }
}

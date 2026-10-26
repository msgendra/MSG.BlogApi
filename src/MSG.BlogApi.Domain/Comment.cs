namespace MSG.BlogApi.Domain;

public class Comment
{
    public int Id { get; set; }
    public int BlogPostId { get; set; }
    public string? Author { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

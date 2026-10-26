using Microsoft.Extensions.Logging;
using MSG.BlogApi.Application.Posts.Requests;
using MSG.BlogApi.Application.Posts.Responses;
using MSG.BlogApi.Domain;

namespace MSG.BlogApi.Application.Posts;

public sealed class BlogPostService(
    IBlogPostRepository repository,
    TimeProvider timeProvider,
    ILogger<BlogPostService> logger)
{
    public Task<IReadOnlyList<BlogPostListItemDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return repository.GetListAsync(cancellationToken);
    }

    public async Task<BlogPostDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var post = await repository.GetByIdAsync(id, cancellationToken);
        return post is null ? null : ToDto(post);
    }

    public async Task<BlogPostDto> CreateAsync(CreateBlogPostRequest request, CancellationToken cancellationToken = default)
    {
        var post = new BlogPost
        {
            Title = request.Title.Trim(),
            Content = request.Content.Trim(),
            CreatedAt = timeProvider.GetUtcNow().UtcDateTime
        };

        await repository.AddAsync(post, cancellationToken);
        logger.LogInformation("Created blog post {PostId}", post.Id);

        return ToDto(post);
    }

    public async Task<CommentDto?> AddCommentAsync(int postId, CreateCommentRequest request, CancellationToken cancellationToken = default)
    {
        if (!await repository.ExistsAsync(postId, cancellationToken))
        {
            return null;
        }

        var comment = new Comment
        {
            BlogPostId = postId,
            Author = string.IsNullOrWhiteSpace(request.Author) ? null : request.Author.Trim(),
            Content = request.Content.Trim(),
            CreatedAt = timeProvider.GetUtcNow().UtcDateTime
        };

        await repository.AddCommentAsync(comment, cancellationToken);
        logger.LogInformation("Added comment {CommentId} to blog post {PostId}", comment.Id, postId);

        return ToDto(comment);
    }



    //NOTE: In a real-world application, I will probably use a mapping library like AutoMapper
    //      to handle the conversion between domain models and DTOs. 

    private static BlogPostDto ToDto(BlogPost post) => new()
    {
        Id = post.Id,
        Title = post.Title,
        Content = post.Content,
        CreatedAt = post.CreatedAt,
        Comments = post.Comments.Select(ToDto).ToList()
    };

    private static CommentDto ToDto(Comment comment)
    {
        return new()
        {
            Id = comment.Id,
            Author = comment.Author,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt
        };
    }
}

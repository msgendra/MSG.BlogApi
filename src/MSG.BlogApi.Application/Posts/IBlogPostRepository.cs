using MSG.BlogApi.Application.Posts.Responses;
using MSG.BlogApi.Domain;

namespace MSG.BlogApi.Application.Posts;

public interface IBlogPostRepository
{
    Task<IReadOnlyList<BlogPostListItemDto>> GetListAsync(CancellationToken cancellationToken);
    Task<BlogPost?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken);
    Task AddAsync(BlogPost post, CancellationToken cancellationToken);
    Task AddCommentAsync(Comment comment, CancellationToken cancellationToken);
}

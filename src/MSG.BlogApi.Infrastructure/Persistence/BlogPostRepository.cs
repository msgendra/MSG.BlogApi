using Microsoft.EntityFrameworkCore;
using MSG.BlogApi.Application.Posts;
using MSG.BlogApi.Application.Posts.Responses;
using MSG.BlogApi.Domain;

namespace MSG.BlogApi.Infrastructure.Persistence;

internal sealed class BlogPostRepository(BlogDbContext db) : IBlogPostRepository
{
    public async Task<IReadOnlyList<BlogPostListItemDto>> GetListAsync(CancellationToken cancellationToken) =>
        await db.BlogPosts
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new BlogPostListItemDto
            {
                Id = p.Id,
                Title = p.Title,
                CommentCount = p.Comments.Count
            })
            .ToListAsync(cancellationToken);

    public Task<BlogPost?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        db.BlogPosts
            .AsNoTracking()
            .Include(p => p.Comments.OrderBy(c => c.CreatedAt))
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken) =>
        db.BlogPosts.AnyAsync(p => p.Id == id, cancellationToken);

    public async Task AddAsync(BlogPost post, CancellationToken cancellationToken)
    {
        db.BlogPosts.Add(post);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task AddCommentAsync(Comment comment, CancellationToken cancellationToken)
    {
        db.Comments.Add(comment);
        await db.SaveChangesAsync(cancellationToken);
    }
}

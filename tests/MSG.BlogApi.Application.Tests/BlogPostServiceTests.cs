using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MSG.BlogApi.Application.Posts;
using MSG.BlogApi.Application.Posts.Requests;
using MSG.BlogApi.Domain;

namespace MSG.BlogApi.Application.Tests;

public class BlogPostServiceTests
{
    private static readonly DateTimeOffset Now = new(2026, 1, 15, 10, 30, 0, TimeSpan.Zero);

    private readonly Mock<IBlogPostRepository> _repository = new();
    private readonly BlogPostService _service;

    public BlogPostServiceTests()
    {
        var timeProvider = new Mock<TimeProvider>();
        timeProvider.Setup(t => t.GetUtcNow()).Returns(Now);
        _service = new BlogPostService(_repository.Object, timeProvider.Object, NullLogger<BlogPostService>.Instance);
    }

    [Fact]
    public async Task CreateAsync_SetsCreationDate()
    {
        var request = new CreateBlogPostRequest { Title = "Post Test", Content = "This is a post example for testing" };

        var result = await _service.CreateAsync(request);

        Assert.Equal("Post Test", result.Title);
        Assert.Equal("This is a post example for testing", result.Content);
        Assert.Equal(Now.UtcDateTime, result.CreatedAt);
        Assert.Empty(result.Comments);
        _repository.Verify(r => r.AddAsync(
            It.Is<BlogPost>(p => p.Title == "Post Test"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenPostDoesNotExist()
    {
        _repository.Setup(r => r.GetByIdAsync(42, It.IsAny<CancellationToken>()))
            .ReturnsAsync((BlogPost?)null);

        var result = await _service.GetByIdAsync(42);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsPostWithComments()
    {
        var post = new BlogPost
        {
            Id = 1,
            Title = "Title",
            Content = "Content",
            Comments =
            [
                new Comment { Id = 10, BlogPostId = 1, Author = "Mariano", Content = "My comment" },
                new Comment { Id = 11, BlogPostId = 1, Content = "Agreed" }
            ]
        };
        _repository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(post);

        var result = await _service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Title", result.Title);
        Assert.Equal(2, result.Comments.Count);
        Assert.Equal("Mariano", result.Comments[0].Author);
        Assert.Null(result.Comments[1].Author);
    }

    [Fact]
    public async Task AddCommentAsync_ReturnsNullAndDoesNotSave_WhenPostDoesNotExist()
    {
        _repository.Setup(r => r.ExistsAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await _service.AddCommentAsync(99, new CreateCommentRequest { Content = "A Comment" });

        Assert.Null(result);
        _repository.Verify(r => r.AddCommentAsync(It.IsAny<Comment>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AddCommentAsync_SavesComment_WhenPostExists()
    {
        _repository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _service.AddCommentAsync(1, new CreateCommentRequest { Author = "  ", Content = "A Comment" });

        Assert.NotNull(result);
        Assert.Null(result.Author);
        Assert.Equal("A Comment", result.Content);
        Assert.Equal(Now.UtcDateTime, result.CreatedAt);
        _repository.Verify(r => r.AddCommentAsync(
            It.Is<Comment>(c => c.BlogPostId == 1 && c.Content == "A Comment"), It.IsAny<CancellationToken>()), Times.Once);
    }
}

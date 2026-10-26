using Microsoft.AspNetCore.Mvc;
using MSG.BlogApi.Application.Posts;
using MSG.BlogApi.Application.Posts.Requests;
using MSG.BlogApi.Application.Posts.Responses;

namespace MSG.BlogApi.Api.Controllers;

[ApiController]
[Route("api/posts")]
[Produces("application/json")]
public class PostsController(BlogPostService postService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<BlogPostListItemDto>>(StatusCodes.Status200OK)]
    public Task<IReadOnlyList<BlogPostListItemDto>> GetAll(CancellationToken cancellationToken) =>
        postService.GetAllAsync(cancellationToken);

    [HttpGet("{id:int}")]
    [ProducesResponseType<BlogPostDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BlogPostDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var post = await postService.GetByIdAsync(id, cancellationToken);
        return post is null ? NotFound() : post;
    }

    [HttpPost]
    [ProducesResponseType<BlogPostDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BlogPostDto>> Create(CreateBlogPostRequest request, CancellationToken cancellationToken)
    {
        var post = await postService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = post.Id }, post);
    }

    [HttpPost("{id:int}/comments")]
    [ProducesResponseType<CommentDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CommentDto>> AddComment(int id, CreateCommentRequest request, CancellationToken cancellationToken)
    {
        var comment = await postService.AddCommentAsync(id, request, cancellationToken);
        return comment is null ? NotFound() : CreatedAtAction(nameof(GetById), new { id }, comment);
    }
}

using backend.Data;
using backend.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

using backend.Dtos.PostDto;
using backend.Dtos.UserDto;

namespace backend.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PostsController : ControllerBase
{
    private readonly AppDbContext db;

    public PostsController(AppDbContext db)
    {
        this.db = db;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Post>>> Index()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var posts = await db.Posts
            .Where(p => p.UserId == userId)
            .Select(p => new PostResponse(
                p.Id,
                p.Title,
                p.Content,
                p.CreatedAt,
                p.UpdatedAt,
                new UserJoinedResponse(p.User.Id, p.User.Name, p.User.Email)
            ))
            .ToListAsync();

        return Ok(posts);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<PostResponse>> Show(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var post = await db.Posts
            .Where(p => p.Id == id && p.UserId == userId)
            .Select(p => new PostResponse(
                p.Id,
                p.Title,
                p.Content,
                p.CreatedAt,
                p.UpdatedAt,
                new UserJoinedResponse(p.User.Id, p.User.Name, p.User.Email)
            ))
            .FirstOrDefaultAsync();

        if (post == null) return NotFound();

        return Ok(post);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<PostResponse>> Store(PostRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var newPost = new Post
        {
            Title = request.Title,
            Content = request.Content,
            UserId = userId
        };

        db.Posts.Add(newPost);
        await db.SaveChangesAsync();

        var user = await db.Users.FindAsync(userId);

        var response = new PostResponse(
            newPost.Id,
            newPost.Title,
            newPost.Content,
            newPost.CreatedAt,
            newPost.UpdatedAt,
            new UserJoinedResponse(user!.Id, user!.Name, user!.Email)
        );

        return CreatedAtAction(nameof(Show), new { id = newPost.Id }, response);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<PostResponse>> Update(int id, PostRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var postInDb = await db.Posts.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

        if (postInDb == null) return NotFound();

        db.Entry(postInDb).CurrentValues.SetValues(request);
        await db.SaveChangesAsync();

        var response = new PostResponse(
            postInDb.Id,
            postInDb.Title,
            postInDb.Content,
            postInDb.CreatedAt,
            postInDb.UpdatedAt,
            new UserJoinedResponse(postInDb.User.Id, postInDb.User.Name, postInDb.User.Email)
        );

        return Ok(response);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Destroy(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var postInDb = await db.Posts.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

        if (postInDb == null) return NotFound();

        db.Posts.Remove(postInDb);
        await db.SaveChangesAsync();

        return NoContent();
    }
}
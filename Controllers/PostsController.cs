using backend.Data;
using backend.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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
    public async Task<ActionResult<IEnumerable<Post>>> Index()
    {
        return await db.Posts.Include(p => p.User).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Post>> Show(int id)
    {
        var post = await db.Posts.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == id);
        if (post == null)
        {
            return NotFound();
        }
        return post;
    }

    [HttpPost]
    public async Task<ActionResult<Post>> Store(Post post)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null) return Unauthorized();

        var newPost = new Post
        {
            Title = post.Title,
            Content = post.Content,
            UserId = int.Parse(userId)
        };

        db.Posts.Add(newPost);
        await db.SaveChangesAsync();

        return Ok(newPost);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Post>> Update(int id, Post post)
    {
        if (id != post.Id) return BadRequest();

        var existingPost = await db.Posts.FindAsync(id);
        if (existingPost == null) return NotFound();

        existingPost.Title = post.Title;
        existingPost.Content = post.Content;

        await db.SaveChangesAsync();

        return Ok(existingPost);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Destroy(int id)
    {
        var post = await db.Posts.FindAsync(id);
        if (post == null) return NotFound();

        db.Posts.Remove(post);
        await db.SaveChangesAsync();

        return NoContent();
    }
}
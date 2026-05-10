using backend.Data;
using backend.Entities;
using backend.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly AppDbContext db;

    public UsersController(AppDbContext db)
    {
        this.db = db;
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<User>> Show(int id)
    {
        var user = await db.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return user;
    }

    [HttpPost]
    public async Task<ActionResult<User>> Store(User user)
    {
        bool emailExists = await db.Users.AnyAsync(u => u.Email == user.Email);

        if (emailExists)
        {
            ModelState.AddModelError("Email", "E-mail já está em uso.");

            return Conflict(ModelState);
        }

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(Show), new { id = user.Id }, user);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<User>> Update(int id, User user)
    {
        if (id != user.Id) return BadRequest();

        db.Entry(user).State = EntityState.Modified;

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!db.Users.Any(u => u.Id == id)) return NotFound();
            throw;
        }

        return Ok(user);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Destroy(int id)
    {
        var user = await db.Users.FindAsync(id);
        if (user == null) return NotFound();

        db.Users.Remove(user);
        await db.SaveChangesAsync();

        return NoContent();
    }
}
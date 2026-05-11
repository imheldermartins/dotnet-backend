using backend.Data;
using backend.Entities;
using backend.Dtos.UserDto;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using BCrypt.Net;

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
    public async Task<ActionResult<UserResponse>> Show(int id)
    {
        var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (loggedInUserId != id.ToString())
        {
            return Forbid();
        }

        var user = await db.Users
            .Where(u => u.Id == id)
            .Select(u => new UserResponse(u.Id, u.Name, u.Email, u.CreatedAt, u.UpdatedAt))
            .FirstOrDefaultAsync();

        if (user == null) return NotFound();

        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<User>> Store(UserRequest request)
    {
        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        bool emailExists = await db.Users.AnyAsync(u => u.Email == user.Email);

        if (emailExists)
        {
            ModelState.AddModelError("Email", "E-mail já está em uso.");

            return Conflict(ModelState);
        }

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return CreatedAtAction(
            nameof(Show),
            new { id = user.Id },
            new UserResponse(user.Id, user.Name, user.Email, user.CreatedAt, user.UpdatedAt));
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<UserResponse>> Update(int id, UserUpdateRequest request)
    {
        var userInDb = await db.Users.FindAsync(id);

        if (userInDb == null) return NotFound();

        db.Entry(userInDb).CurrentValues.SetValues(request);

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!db.Users.Any(u => u.Id == id)) return NotFound();
            throw;
        }

        var response = new UserResponse(
            userInDb.Id,
            userInDb.Name,
            userInDb.Email,
            userInDb.CreatedAt,
            userInDb.UpdatedAt
        );

        return Ok(response);
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
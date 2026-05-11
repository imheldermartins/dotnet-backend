using Vertrau.Data;
using Vertrau.Entities;
using Vertrau.Dtos.UserDto;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using BCrypt.Net;

namespace Vertrau.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly AppDbContext db;

    public UsersController(AppDbContext db)
    {
        this.db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponse>>> Index(
        [FromQuery] string? firstName,
        [FromQuery] string? lastName,
        [FromQuery] string? email,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = db.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(firstName))
            query = query.Where(u => u.FirstName.Contains(firstName));

        if (!string.IsNullOrWhiteSpace(lastName))
            query = query.Where(u => u.LastName.Contains(lastName));

        if (!string.IsNullOrWhiteSpace(email))
            query = query.Where(u => u.Email.Contains(email));

        // Paginação
        var users = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new UserResponse(u.Id, u.FirstName, u.LastName, u.Email, u.Gender, u.BirthDate, u.CreatedAt, u.UpdatedAt))
            .ToListAsync();

        return Ok(users);
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
            .Select(u => new UserResponse(u.Id, u.FirstName, u.LastName, u.Email, u.Gender, u.BirthDate, u.CreatedAt, u.UpdatedAt))
            .FirstOrDefaultAsync();

        if (user == null) return NotFound();

        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<User>> Store(UserRequest request)
    {
        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Gender = request.Gender!.Value,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            BirthDate = request.BirthDate
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
            new UserResponse(user.Id, user.FirstName, user.LastName, user.Email, user.Gender, user.BirthDate, user.CreatedAt, user.UpdatedAt));
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
            userInDb.FirstName,
            userInDb.LastName,
            userInDb.Email,
            userInDb.Gender,
            userInDb.BirthDate,
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
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

    [HttpPost("login")]
    public async Task<ActionResult<User>> Login(AuthRequest request, [FromServices] IConfiguration config)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == request.Email && u.Password == request.Password);

        if (user == null)
        {
            ModelState.AddModelError("Authentication", "Credenciais inválidas.");
            return Unauthorized(ModelState);
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        var tokenHandler = new JwtSecurityTokenHandler();

        return Ok(new { userId = user.Id, token = tokenHandler.WriteToken(token) });
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
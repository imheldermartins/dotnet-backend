using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vertrau.Data;
using Vertrau.Entities;
using Vertrau.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;

using BCrypt.Net;

namespace Vertrau.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppDbContext db;
    private readonly IConfiguration config;

    public AuthController(AppDbContext db, IConfiguration config)
    {
        this.db = db;
        this.config = config;
    }

    [HttpPost("login")]
    public async Task<ActionResult<User>> Login(AuthRequest request, [FromServices] IConfiguration config)
    {
        var user = await db.Users.FirstOrDefaultAsync(u =>
            u.Email == request.Email
        );

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        {
            ModelState.AddModelError("Authentication", "Credenciais inválidas.");
            return Unauthorized(ModelState);
        }

        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await db.SaveChangesAsync();

        return Ok(new
        {
            accessToken,
            refreshToken,
            userId = user.Id
        });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken);

        if (
            user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow
        )
            return Unauthorized(new { message = "Sessão expirada. Faça login novamente." });

        var newAccessToken = GenerateAccessToken(user);
        var newRefreshToken = GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await db.SaveChangesAsync();

        return Ok(new
        {
            accessToken = newAccessToken,
            refreshToken = newRefreshToken
        });
    }

    private string GenerateAccessToken(Entities.User user)
    {
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
            expires: DateTime.UtcNow.AddHours(1), // Access Token de vida curta!
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        // Gera uma string aleatória muito forte em Base64
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}

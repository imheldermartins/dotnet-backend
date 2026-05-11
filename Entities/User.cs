using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Vertrau.Entities;

public enum Gender
{
    Male = 1,
    Female = 2,
    Other = 3
}

[Index(nameof(Email), IsUnique = true)]
public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email é obrigatório.")]
    [EmailAddress(ErrorMessage = "Email inválido.")]
    [StringLength(255, ErrorMessage = "Email deve ter no máximo 255 caracteres.")]
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (BirthDate.HasValue && BirthDate.Value > DateTime.Today)
        {
            yield return new ValidationResult(
                "A data de nascimento não pode ser uma data futura.",
                new[] { nameof(BirthDate) }
            );
        }
    }

    public List<Post> Posts { get; set; } = new List<Post>();
}

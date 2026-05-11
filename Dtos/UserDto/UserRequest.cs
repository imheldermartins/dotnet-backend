using System;
using System.ComponentModel.DataAnnotations;
using Vertrau.Entities;

namespace Vertrau.Dtos.UserDto;

public record UserRequest(
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres.")]
    string FirstName,

    [Required(ErrorMessage = "O sobrenome é obrigatório.")]
    [StringLength(100, ErrorMessage = "O sobrenome não pode exceder 100 caracteres.")]
    string LastName,

    [Required(ErrorMessage = "O email é obrigatório.")]
    [EmailAddress(ErrorMessage = "Formato de email inválido.")]
    string Email,

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "A senha deve ter entre 8 e 100 caracteres.")]
    string Password,

    [Required(ErrorMessage = "O gênero é obrigatório.")]
    Gender? Gender,

    [DataType(DataType.Date)]
    DateTime? BirthDate
) : IValidatableObject // 2. Assinando a interface de validação
{
    // 3. Regra de Negócio: Data não pode ser no futuro
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (BirthDate.HasValue && BirthDate.Value.Date > DateTime.UtcNow.Date)
        {
            yield return new ValidationResult(
                "A data de nascimento não pode ser no futuro.",
                new[] { "Data de Nascimento" }
            );
        }
    }
}
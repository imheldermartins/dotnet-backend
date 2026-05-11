using System;
using System.ComponentModel.DataAnnotations;
using Vertrau.Entities;

namespace Vertrau.Dtos.UserDto;

public record UserUpdateRequest(
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres.")]
    string FirstName,

    [Required(ErrorMessage = "O email é obrigatório.")]
    [EmailAddress(ErrorMessage = "Formato de email inválido.")]
    string Email,

    [Required(ErrorMessage = "O gênero é obrigatório.")]
    Gender Gender,

    [DataType(DataType.Date, ErrorMessage = "Formato de data inválido.")]
    DateTime? BirthDate
);
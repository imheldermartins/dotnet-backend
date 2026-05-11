using System;
using System.ComponentModel.DataAnnotations;
using Vertrau.Entities;

namespace Vertrau.Dtos.UserDto;

public record UserResponse(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    Gender Gender,
    DateTime? BirthDate,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
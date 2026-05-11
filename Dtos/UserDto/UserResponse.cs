using System;
using System.ComponentModel.DataAnnotations;

namespace backend.Dtos.UserDto;

public record UserResponse(
    int Id,
    string Name,
    string Email,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
using System;
using System.ComponentModel.DataAnnotations;

namespace backend.Dtos.UserDto;

public record UserJoinedResponse(
    int Id,
    string Name,
    string Email
);

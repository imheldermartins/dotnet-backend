using System;
using System.ComponentModel.DataAnnotations;

namespace Vertrau.Dtos.UserDto;

public record UserJoinedResponse(
    int Id,
    string Name,
    string Email
);

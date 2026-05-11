using System;
using Vertrau.Dtos.UserDto;

namespace Vertrau.Dtos.PostDto;

public record PostResponse(
    int Id,
    string Title,
    string Content,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    UserJoinedResponse User
);

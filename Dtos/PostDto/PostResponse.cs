using System;
using backend.Dtos.UserDto;

namespace backend.Dtos.PostDto;

public record PostResponse(
    int Id,
    string Title,
    string Content,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    UserJoinedResponse User
);

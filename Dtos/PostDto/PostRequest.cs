using System;

namespace backend.Dtos.PostDto;

public record PostRequest(
    string Title,
    string Content
);

using System;

namespace Vertrau.Dtos.PostDto;

public record PostRequest(
    string Title,
    string Content
);

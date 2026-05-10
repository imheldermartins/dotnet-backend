using System;

namespace backend.Dtos;

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}

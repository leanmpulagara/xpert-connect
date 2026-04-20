using System.ComponentModel.DataAnnotations;

namespace XpertConnect.Application.Features.Auth.DTOs;

public class RefreshTokenRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}

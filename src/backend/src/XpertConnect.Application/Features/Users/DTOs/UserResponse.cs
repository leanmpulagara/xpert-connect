using XpertConnect.Domain.Enums;

namespace XpertConnect.Application.Features.Users.DTOs;

public class UserResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string? ProfilePhotoUrl { get; set; }
    public UserType UserType { get; set; }
    public string UserTypeName => UserType.ToString();
    public VerificationStatus VerificationStatus { get; set; }
    public string VerificationStatusName => VerificationStatus.ToString();
    public bool IsActive { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

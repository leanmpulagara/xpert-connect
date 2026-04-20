using XpertConnect.Domain.Enums;

namespace XpertConnect.Application.Features.Users.DTOs;

public class UserListResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public UserType UserType { get; set; }
    public string UserTypeName => UserType.ToString();
    public VerificationStatus VerificationStatus { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

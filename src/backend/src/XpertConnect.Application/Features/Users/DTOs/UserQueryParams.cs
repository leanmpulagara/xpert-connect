using XpertConnect.Domain.Enums;

namespace XpertConnect.Application.Features.Users.DTOs;

public class UserQueryParams
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public UserType? UserType { get; set; }
    public VerificationStatus? VerificationStatus { get; set; }
    public bool? IsActive { get; set; }
    public string? SortBy { get; set; } = "CreatedAt";
    public bool SortDescending { get; set; } = true;
}

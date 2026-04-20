namespace XpertConnect.Application.Features.Users.DTOs;

public class UpdateUserRequest
{
    public string? Phone { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ProfilePhotoUrl { get; set; }
}

namespace XpertConnect.Application.Features.Seekers.DTOs;

/// <summary>
/// Request to create a seeker profile
/// </summary>
public class CreateSeekerProfileRequest
{
    public string? Company { get; set; }
    public string? JobTitle { get; set; }
}

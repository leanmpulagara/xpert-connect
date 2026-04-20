namespace XpertConnect.Application.Features.Seekers.DTOs;

/// <summary>
/// Request to update seeker profile
/// </summary>
public class UpdateSeekerProfileRequest
{
    public string? Company { get; set; }
    public string? JobTitle { get; set; }
}

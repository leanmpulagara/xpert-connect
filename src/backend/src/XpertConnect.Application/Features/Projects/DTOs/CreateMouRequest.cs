namespace XpertConnect.Application.Features.Projects.DTOs;

/// <summary>
/// Request to create a Memorandum of Understanding
/// </summary>
public class CreateMouRequest
{
    public string? Scope { get; set; }
    public string? Timeline { get; set; }
}

/// <summary>
/// Request to sign a Memorandum of Understanding
/// </summary>
public class SignMouRequest
{
    public string? Signature { get; set; }
}

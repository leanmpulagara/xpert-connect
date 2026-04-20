namespace XpertConnect.Application.Features.Projects.DTOs;

/// <summary>
/// Memorandum of Understanding response
/// </summary>
public class MouResponse
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectTitle { get; set; } = string.Empty;
    public string? Scope { get; set; }
    public string? Timeline { get; set; }
    public DateTime? SignedAt { get; set; }
    public bool IsSigned => SignedAt.HasValue;
    public string? DocumentUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}

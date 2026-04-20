namespace XpertConnect.Application.Features.Feedback.DTOs;

/// <summary>
/// Feedback response
/// </summary>
public class FeedbackResponse
{
    public Guid Id { get; set; }
    public Guid ConsultationId { get; set; }
    public Guid SeekerId { get; set; }
    public string SeekerName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string? Comments { get; set; }
    public DateTime CreatedAt { get; set; }
}

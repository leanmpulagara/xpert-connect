namespace XpertConnect.Application.Features.Feedback.DTOs;

/// <summary>
/// Request to create feedback for a consultation
/// </summary>
public class CreateFeedbackRequest
{
    public int Rating { get; set; }
    public string? Comments { get; set; }
}

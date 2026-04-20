namespace XpertConnect.Application.Features.Feedback.DTOs;

/// <summary>
/// Summary of feedback for an expert
/// </summary>
public class ExpertFeedbackSummary
{
    public Guid ExpertId { get; set; }
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public int FiveStarCount { get; set; }
    public int FourStarCount { get; set; }
    public int ThreeStarCount { get; set; }
    public int TwoStarCount { get; set; }
    public int OneStarCount { get; set; }
    public IList<FeedbackResponse> RecentFeedback { get; set; } = new List<FeedbackResponse>();
}

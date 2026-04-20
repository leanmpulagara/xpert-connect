namespace XpertConnect.Application.Features.Notifications.DTOs;

/// <summary>
/// Base notification message
/// </summary>
public class NotificationMessage
{
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object>? Data { get; set; }
}

/// <summary>
/// Notification for auction bid updates
/// </summary>
public class BidNotification
{
    public Guid AuctionId { get; set; }
    public Guid BidId { get; set; }
    public decimal Amount { get; set; }
    public string BidderInitials { get; set; } = string.Empty;
    public bool IsProxyBid { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public int TotalBids { get; set; }
}

/// <summary>
/// Notification when user is outbid
/// </summary>
public class OutbidNotification
{
    public Guid AuctionId { get; set; }
    public string AuctionTitle { get; set; } = string.Empty;
    public decimal YourBidAmount { get; set; }
    public decimal NewHighBidAmount { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Auction status change notification
/// </summary>
public class AuctionStatusNotification
{
    public Guid AuctionId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? WinnerName { get; set; }
    public decimal? WinningAmount { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Consultation reminder notification
/// </summary>
public class ConsultationReminderNotification
{
    public Guid ConsultationId { get; set; }
    public string ExpertName { get; set; } = string.Empty;
    public string SeekerName { get; set; } = string.Empty;
    public DateTime ScheduledAt { get; set; }
    public int MinutesUntilStart { get; set; }
    public string? MeetingLink { get; set; }
}

/// <summary>
/// Consultation status change notification
/// </summary>
public class ConsultationStatusNotification
{
    public Guid ConsultationId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string ExpertName { get; set; } = string.Empty;
    public string SeekerName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Payment notification
/// </summary>
public class PaymentNotification
{
    public Guid PaymentId { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Project application notification (for non-profits)
/// </summary>
public class ProjectApplicationNotification
{
    public Guid ProjectId { get; set; }
    public string ProjectTitle { get; set; } = string.Empty;
    public Guid ExpertId { get; set; }
    public string ExpertName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

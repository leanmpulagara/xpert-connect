using XpertConnect.Application.Features.Notifications.DTOs;

namespace XpertConnect.Application.Common.Interfaces;

/// <summary>
/// Service for sending real-time notifications via SignalR
/// </summary>
public interface INotificationService
{
    // Auction notifications
    Task NotifyNewBidAsync(Guid auctionId, BidNotification notification, CancellationToken cancellationToken = default);
    Task NotifyOutbidAsync(Guid userId, OutbidNotification notification, CancellationToken cancellationToken = default);
    Task NotifyAuctionStatusChangeAsync(Guid auctionId, AuctionStatusNotification notification, CancellationToken cancellationToken = default);
    Task NotifyAuctionWinnerAsync(Guid userId, AuctionStatusNotification notification, CancellationToken cancellationToken = default);

    // Consultation notifications
    Task NotifyConsultationReminderAsync(Guid userId, ConsultationReminderNotification notification, CancellationToken cancellationToken = default);
    Task NotifyConsultationStatusChangeAsync(Guid userId, ConsultationStatusNotification notification, CancellationToken cancellationToken = default);

    // Payment notifications
    Task NotifyPaymentStatusAsync(Guid userId, PaymentNotification notification, CancellationToken cancellationToken = default);

    // Project notifications
    Task NotifyProjectApplicationAsync(Guid orgId, ProjectApplicationNotification notification, CancellationToken cancellationToken = default);

    // Generic user notification
    Task NotifyUserAsync(Guid userId, NotificationMessage notification, CancellationToken cancellationToken = default);

    // Broadcast to all connected users (admin announcements)
    Task BroadcastAsync(NotificationMessage notification, CancellationToken cancellationToken = default);
}

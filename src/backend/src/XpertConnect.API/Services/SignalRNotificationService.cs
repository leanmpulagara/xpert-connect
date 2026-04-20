using Microsoft.AspNetCore.SignalR;
using XpertConnect.API.Hubs;
using XpertConnect.Application.Common.Interfaces;
using XpertConnect.Application.Features.Notifications.DTOs;

namespace XpertConnect.API.Services;

/// <summary>
/// SignalR-based implementation of the notification service
/// </summary>
public class SignalRNotificationService : INotificationService
{
    private readonly IHubContext<AuctionHub> _auctionHub;
    private readonly IHubContext<NotificationHub> _notificationHub;
    private readonly ILogger<SignalRNotificationService> _logger;

    public SignalRNotificationService(
        IHubContext<AuctionHub> auctionHub,
        IHubContext<NotificationHub> notificationHub,
        ILogger<SignalRNotificationService> logger)
    {
        _auctionHub = auctionHub;
        _notificationHub = notificationHub;
        _logger = logger;
    }

    /// <summary>
    /// Notify all clients watching an auction about a new bid
    /// </summary>
    public async Task NotifyNewBidAsync(Guid auctionId, BidNotification notification, CancellationToken cancellationToken = default)
    {
        var groupName = AuctionHub.GetAuctionGroupName(auctionId);

        _logger.LogInformation("Broadcasting new bid notification to auction {AuctionId}: Amount={Amount}",
            auctionId, notification.Amount);

        await _auctionHub.Clients.Group(groupName).SendAsync("NewBid", notification, cancellationToken);
    }

    /// <summary>
    /// Notify a user that they've been outbid
    /// </summary>
    public async Task NotifyOutbidAsync(Guid userId, OutbidNotification notification, CancellationToken cancellationToken = default)
    {
        var groupName = NotificationHub.GetUserGroupName(userId);

        _logger.LogInformation("Sending outbid notification to user {UserId} for auction {AuctionId}",
            userId, notification.AuctionId);

        await _notificationHub.Clients.Group(groupName).SendAsync("Outbid", notification, cancellationToken);
    }

    /// <summary>
    /// Notify all clients watching an auction about status change
    /// </summary>
    public async Task NotifyAuctionStatusChangeAsync(Guid auctionId, AuctionStatusNotification notification, CancellationToken cancellationToken = default)
    {
        var groupName = AuctionHub.GetAuctionGroupName(auctionId);

        _logger.LogInformation("Broadcasting auction status change to {AuctionId}: Status={Status}",
            auctionId, notification.Status);

        await _auctionHub.Clients.Group(groupName).SendAsync("AuctionStatusChanged", notification, cancellationToken);
    }

    /// <summary>
    /// Notify the auction winner
    /// </summary>
    public async Task NotifyAuctionWinnerAsync(Guid userId, AuctionStatusNotification notification, CancellationToken cancellationToken = default)
    {
        var groupName = NotificationHub.GetUserGroupName(userId);

        _logger.LogInformation("Sending auction winner notification to user {UserId} for auction {AuctionId}",
            userId, notification.AuctionId);

        await _notificationHub.Clients.Group(groupName).SendAsync("AuctionWon", notification, cancellationToken);
    }

    /// <summary>
    /// Send consultation reminder to a user
    /// </summary>
    public async Task NotifyConsultationReminderAsync(Guid userId, ConsultationReminderNotification notification, CancellationToken cancellationToken = default)
    {
        var groupName = NotificationHub.GetUserGroupName(userId);

        _logger.LogInformation("Sending consultation reminder to user {UserId} for consultation {ConsultationId}",
            userId, notification.ConsultationId);

        await _notificationHub.Clients.Group(groupName).SendAsync("ConsultationReminder", notification, cancellationToken);
    }

    /// <summary>
    /// Notify user about consultation status change
    /// </summary>
    public async Task NotifyConsultationStatusChangeAsync(Guid userId, ConsultationStatusNotification notification, CancellationToken cancellationToken = default)
    {
        var groupName = NotificationHub.GetUserGroupName(userId);

        _logger.LogInformation("Sending consultation status notification to user {UserId}: Status={Status}",
            userId, notification.Status);

        await _notificationHub.Clients.Group(groupName).SendAsync("ConsultationStatusChanged", notification, cancellationToken);
    }

    /// <summary>
    /// Notify user about payment status
    /// </summary>
    public async Task NotifyPaymentStatusAsync(Guid userId, PaymentNotification notification, CancellationToken cancellationToken = default)
    {
        var groupName = NotificationHub.GetUserGroupName(userId);

        _logger.LogInformation("Sending payment notification to user {UserId}: Status={Status}",
            userId, notification.Status);

        await _notificationHub.Clients.Group(groupName).SendAsync("PaymentStatus", notification, cancellationToken);
    }

    /// <summary>
    /// Notify organization about a new project application
    /// </summary>
    public async Task NotifyProjectApplicationAsync(Guid orgId, ProjectApplicationNotification notification, CancellationToken cancellationToken = default)
    {
        var groupName = NotificationHub.GetOrgGroupName(orgId);

        _logger.LogInformation("Sending project application notification to org {OrgId} for project {ProjectId}",
            orgId, notification.ProjectId);

        await _notificationHub.Clients.Group(groupName).SendAsync("ProjectApplication", notification, cancellationToken);
    }

    /// <summary>
    /// Send a generic notification to a specific user
    /// </summary>
    public async Task NotifyUserAsync(Guid userId, NotificationMessage notification, CancellationToken cancellationToken = default)
    {
        var groupName = NotificationHub.GetUserGroupName(userId);

        _logger.LogInformation("Sending notification to user {UserId}: Type={Type}",
            userId, notification.Type);

        await _notificationHub.Clients.Group(groupName).SendAsync("Notification", notification, cancellationToken);
    }

    /// <summary>
    /// Broadcast a notification to all connected users
    /// </summary>
    public async Task BroadcastAsync(NotificationMessage notification, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Broadcasting notification to all users: Type={Type}", notification.Type);

        await _notificationHub.Clients.All.SendAsync("Broadcast", notification, cancellationToken);
    }
}

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace XpertConnect.API.Hubs;

/// <summary>
/// SignalR hub for user notifications
/// </summary>
[Authorize]
public class NotificationHub : Hub
{
    private readonly ILogger<NotificationHub> _logger;

    public NotificationHub(ILogger<NotificationHub> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Called when a client connects to the hub
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!string.IsNullOrEmpty(userId))
        {
            // Add user to their personal notification group
            await Groups.AddToGroupAsync(Context.ConnectionId, GetUserGroupName(userId));

            _logger.LogInformation("User {UserId} connected to NotificationHub. ConnectionId: {ConnectionId}",
                userId, Context.ConnectionId);

            // Notify client of successful connection
            await Clients.Caller.SendAsync("Connected", new
            {
                connectionId = Context.ConnectionId,
                timestamp = DateTime.UtcNow
            });
        }

        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Called when a client disconnects from the hub
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!string.IsNullOrEmpty(userId))
        {
            _logger.LogInformation("User {UserId} disconnected from NotificationHub. ConnectionId: {ConnectionId}",
                userId, Context.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Subscribe to a specific notification channel (e.g., for specific consultation or project)
    /// </summary>
    public async Task Subscribe(string channel)
    {
        if (string.IsNullOrEmpty(channel))
        {
            throw new HubException("Channel name is required");
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, channel);

        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        _logger.LogInformation("User {UserId} subscribed to channel {Channel}", userId, channel);

        await Clients.Caller.SendAsync("Subscribed", new { channel });
    }

    /// <summary>
    /// Unsubscribe from a notification channel
    /// </summary>
    public async Task Unsubscribe(string channel)
    {
        if (string.IsNullOrEmpty(channel))
        {
            throw new HubException("Channel name is required");
        }

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, channel);

        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        _logger.LogInformation("User {UserId} unsubscribed from channel {Channel}", userId, channel);

        await Clients.Caller.SendAsync("Unsubscribed", new { channel });
    }

    /// <summary>
    /// Mark notifications as read
    /// </summary>
    public async Task MarkAsRead(List<Guid> notificationIds)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        _logger.LogInformation("User {UserId} marked {Count} notifications as read",
            userId, notificationIds.Count);

        // Note: In a full implementation, this would update the database
        await Clients.Caller.SendAsync("NotificationsRead", new { notificationIds });
    }

    /// <summary>
    /// Ping to keep connection alive
    /// </summary>
    public async Task Ping()
    {
        await Clients.Caller.SendAsync("Pong", new
        {
            serverTime = DateTime.UtcNow
        });
    }

    public static string GetUserGroupName(string userId) => $"user_{userId}";
    public static string GetUserGroupName(Guid userId) => $"user_{userId}";
    public static string GetConsultationGroupName(Guid consultationId) => $"consultation_{consultationId}";
    public static string GetProjectGroupName(Guid projectId) => $"project_{projectId}";
    public static string GetOrgGroupName(Guid orgId) => $"org_{orgId}";
}

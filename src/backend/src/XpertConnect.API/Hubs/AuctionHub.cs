using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using XpertConnect.Application.Common.Interfaces;
using XpertConnect.Application.Features.Notifications.DTOs;

namespace XpertConnect.API.Hubs;

/// <summary>
/// SignalR hub for real-time auction updates
/// </summary>
[Authorize]
public class AuctionHub : Hub
{
    private readonly IAuctionRepository _auctionRepository;
    private readonly ILogger<AuctionHub> _logger;

    public AuctionHub(
        IAuctionRepository auctionRepository,
        ILogger<AuctionHub> logger)
    {
        _auctionRepository = auctionRepository;
        _logger = logger;
    }

    /// <summary>
    /// Called when a client connects to the hub
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        _logger.LogInformation("User {UserId} connected to AuctionHub. ConnectionId: {ConnectionId}",
            userId, Context.ConnectionId);

        // Add user to their personal group for targeted notifications
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
        }

        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Called when a client disconnects from the hub
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        _logger.LogInformation("User {UserId} disconnected from AuctionHub. ConnectionId: {ConnectionId}",
            userId, Context.ConnectionId);

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Join an auction room to receive real-time bid updates
    /// </summary>
    public async Task JoinAuction(Guid auctionId)
    {
        var auction = await _auctionRepository.GetByIdAsync(auctionId);
        if (auction == null)
        {
            throw new HubException("Auction not found");
        }

        var groupName = GetAuctionGroupName(auctionId);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        _logger.LogInformation("User {UserId} joined auction {AuctionId}", userId, auctionId);

        // Send current auction state to the joining client
        await Clients.Caller.SendAsync("AuctionJoined", new
        {
            auctionId,
            currentBid = auction.CurrentHighBid,
            bidCount = auction.Bids?.Count ?? 0,
            status = auction.Status.ToString(),
            endTime = auction.EndTime
        });
    }

    /// <summary>
    /// Leave an auction room
    /// </summary>
    public async Task LeaveAuction(Guid auctionId)
    {
        var groupName = GetAuctionGroupName(auctionId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        _logger.LogInformation("User {UserId} left auction {AuctionId}", userId, auctionId);
    }

    /// <summary>
    /// Get the current state of an auction
    /// </summary>
    public async Task GetAuctionState(Guid auctionId)
    {
        var auction = await _auctionRepository.GetByIdAsync(auctionId);
        if (auction == null)
        {
            throw new HubException("Auction not found");
        }

        await Clients.Caller.SendAsync("AuctionState", new
        {
            auctionId,
            currentBid = auction.CurrentHighBid,
            bidCount = auction.Bids?.Count ?? 0,
            status = auction.Status.ToString(),
            endTime = auction.EndTime,
            timeRemaining = (int)Math.Max(0, (auction.EndTime - DateTime.UtcNow).TotalSeconds)
        });
    }

    /// <summary>
    /// Ping to keep connection alive and get server time
    /// </summary>
    public async Task Ping()
    {
        await Clients.Caller.SendAsync("Pong", new
        {
            serverTime = DateTime.UtcNow
        });
    }

    public static string GetAuctionGroupName(Guid auctionId) => $"auction_{auctionId}";
}

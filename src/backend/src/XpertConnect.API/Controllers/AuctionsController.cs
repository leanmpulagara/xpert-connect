using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using XpertConnect.Application.Common.Interfaces;
using XpertConnect.Application.Common.Models;
using XpertConnect.Application.Features.Auctions.DTOs;
using XpertConnect.Application.Features.Bids.DTOs;
using XpertConnect.Application.Features.Notifications.DTOs;
using XpertConnect.Domain.Entities;
using XpertConnect.Domain.Enums;
using XpertConnect.Infrastructure.Identity;

namespace XpertConnect.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuctionsController : ControllerBase
{
    private readonly IAuctionRepository _auctionRepository;
    private readonly IBidRepository _bidRepository;
    private readonly IExpertRepository _expertRepository;
    private readonly ISeekerRepository _seekerRepository;
    private readonly INotificationService _notificationService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public AuctionsController(
        IAuctionRepository auctionRepository,
        IBidRepository bidRepository,
        IExpertRepository expertRepository,
        ISeekerRepository seekerRepository,
        INotificationService notificationService,
        UserManager<ApplicationUser> userManager,
        IMapper mapper)
    {
        _auctionRepository = auctionRepository;
        _bidRepository = bidRepository;
        _expertRepository = expertRepository;
        _seekerRepository = seekerRepository;
        _notificationService = notificationService;
        _userManager = userManager;
        _mapper = mapper;
    }

    /// <summary>
    /// Browse all auctions (public)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AuctionListResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] AuctionQueryParams queryParams, CancellationToken cancellationToken)
    {
        var result = await _auctionRepository.GetAllAsync(queryParams, cancellationToken);
        var responses = _mapper.Map<IReadOnlyList<AuctionListResponse>>(result.Items);

        return Ok(PagedResult<AuctionListResponse>.Create(
            responses,
            result.TotalCount,
            result.PageNumber,
            result.PageSize));
    }

    /// <summary>
    /// Get auction by ID (public)
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AuctionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var auction = await _auctionRepository.GetByIdAsync(id, cancellationToken);
        if (auction == null)
        {
            return NotFound(new { message = "Auction not found" });
        }

        return Ok(_mapper.Map<AuctionResponse>(auction));
    }

    /// <summary>
    /// Get current expert's auctions
    /// </summary>
    [HttpGet("my")]
    [Authorize(Policy = "RequireExpertRole")]
    [ProducesResponseType(typeof(PagedResult<AuctionListResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyAuctions([FromQuery] AuctionQueryParams queryParams, CancellationToken cancellationToken)
    {
        var appUser = await GetCurrentAppUserAsync();
        if (appUser?.DomainUserId == null)
        {
            return NotFound(new { message = "User profile not found" });
        }

        var expert = await _expertRepository.GetByUserIdAsync(appUser.DomainUserId.Value, cancellationToken);
        if (expert == null)
        {
            return NotFound(new { message = "Expert profile not found" });
        }

        var result = await _auctionRepository.GetByExpertIdAsync(expert.Id, queryParams, cancellationToken);
        var responses = _mapper.Map<IReadOnlyList<AuctionListResponse>>(result.Items);

        return Ok(PagedResult<AuctionListResponse>.Create(
            responses,
            result.TotalCount,
            result.PageNumber,
            result.PageSize));
    }

    /// <summary>
    /// Create a new auction (Expert only)
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "RequireExpertRole")]
    [ProducesResponseType(typeof(AuctionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateAuctionRequest request, CancellationToken cancellationToken)
    {
        var appUser = await GetCurrentAppUserAsync();
        if (appUser?.DomainUserId == null)
        {
            return NotFound(new { message = "User profile not found" });
        }

        var expert = await _expertRepository.GetByUserIdAsync(appUser.DomainUserId.Value, cancellationToken);
        if (expert == null)
        {
            return NotFound(new { message = "Expert profile not found" });
        }

        var auction = new AuctionLot
        {
            ExpertId = expert.Id,
            BeneficiaryOrgId = request.BeneficiaryOrgId,
            Title = request.Title,
            Description = request.Description,
            MeetingType = request.MeetingType,
            GuestLimit = request.GuestLimit,
            StartingBid = request.StartingBid,
            BuyNowPrice = request.BuyNowPrice,
            StartTime = DateTime.SpecifyKind(request.StartTime, DateTimeKind.Utc),
            EndTime = DateTime.SpecifyKind(request.EndTime, DateTimeKind.Utc),
            Status = AuctionStatus.Draft
        };

        await _auctionRepository.CreateAsync(auction, cancellationToken);

        // Reload with includes
        auction = await _auctionRepository.GetByIdAsync(auction.Id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = auction!.Id }, _mapper.Map<AuctionResponse>(auction));
    }

    /// <summary>
    /// Update an auction (Expert only, before it goes live)
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "RequireExpertRole")]
    [ProducesResponseType(typeof(AuctionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAuctionRequest request, CancellationToken cancellationToken)
    {
        var auction = await _auctionRepository.GetByIdAsync(id, cancellationToken);
        if (auction == null)
        {
            return NotFound(new { message = "Auction not found" });
        }

        // Verify ownership
        var appUser = await GetCurrentAppUserAsync();
        if (appUser?.DomainUserId == null || auction.Expert.UserId != appUser.DomainUserId)
        {
            return Forbid();
        }

        // Can only update draft or scheduled auctions
        if (auction.Status != AuctionStatus.Draft && auction.Status != AuctionStatus.Scheduled)
        {
            return BadRequest(new { message = "Cannot update auction after it has started" });
        }

        // Update fields
        if (request.Title != null) auction.Title = request.Title;
        if (request.Description != null) auction.Description = request.Description;
        if (request.MeetingType.HasValue) auction.MeetingType = request.MeetingType.Value;
        if (request.GuestLimit.HasValue) auction.GuestLimit = request.GuestLimit.Value;
        if (request.StartingBid.HasValue) auction.StartingBid = request.StartingBid.Value;
        if (request.BuyNowPrice.HasValue) auction.BuyNowPrice = request.BuyNowPrice.Value;
        if (request.StartTime.HasValue) auction.StartTime = DateTime.SpecifyKind(request.StartTime.Value, DateTimeKind.Utc);
        if (request.EndTime.HasValue) auction.EndTime = DateTime.SpecifyKind(request.EndTime.Value, DateTimeKind.Utc);

        await _auctionRepository.UpdateAsync(auction, cancellationToken);

        return Ok(_mapper.Map<AuctionResponse>(auction));
    }

    /// <summary>
    /// Publish auction (move from Draft to Scheduled)
    /// </summary>
    [HttpPost("{id:guid}/publish")]
    [Authorize(Policy = "RequireExpertRole")]
    [ProducesResponseType(typeof(AuctionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Publish(Guid id, CancellationToken cancellationToken)
    {
        var auction = await _auctionRepository.GetByIdAsync(id, cancellationToken);
        if (auction == null)
        {
            return NotFound(new { message = "Auction not found" });
        }

        // Verify ownership
        var appUser = await GetCurrentAppUserAsync();
        if (appUser?.DomainUserId == null || auction.Expert.UserId != appUser.DomainUserId)
        {
            return Forbid();
        }

        if (auction.Status != AuctionStatus.Draft)
        {
            return BadRequest(new { message = "Only draft auctions can be published" });
        }

        // Validate auction is ready
        if (auction.StartTime <= DateTime.UtcNow)
        {
            return BadRequest(new { message = "Start time must be in the future" });
        }

        auction.Status = AuctionStatus.Scheduled;
        await _auctionRepository.UpdateAsync(auction, cancellationToken);

        return Ok(_mapper.Map<AuctionResponse>(auction));
    }

    /// <summary>
    /// Place a bid on an auction
    /// </summary>
    [HttpPost("{id:guid}/bids")]
    [Authorize(Policy = "RequireSeekerRole")]
    [ProducesResponseType(typeof(BidResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PlaceBid(Guid id, [FromBody] PlaceBidRequest request, CancellationToken cancellationToken)
    {
        var auction = await _auctionRepository.GetByIdAsync(id, cancellationToken);
        if (auction == null)
        {
            return NotFound(new { message = "Auction not found" });
        }

        // Check auction is open
        if (auction.Status != AuctionStatus.Open)
        {
            return BadRequest(new { message = "Auction is not open for bidding" });
        }

        var now = DateTime.UtcNow;
        if (now < auction.StartTime || now > auction.EndTime)
        {
            return BadRequest(new { message = "Auction is not currently active" });
        }

        // Get seeker
        var appUser = await GetCurrentAppUserAsync();
        if (appUser?.DomainUserId == null)
        {
            return NotFound(new { message = "User profile not found" });
        }

        var seeker = await _seekerRepository.GetByUserIdAsync(appUser.DomainUserId.Value, cancellationToken);
        if (seeker == null)
        {
            return NotFound(new { message = "Seeker profile not found" });
        }

        // Validate bid amount
        var minBid = auction.CurrentHighBid ?? auction.StartingBid;
        if (request.Amount <= minBid)
        {
            return BadRequest(new { message = $"Bid must be greater than current high bid: {minBid:C}" });
        }

        // Check for buy now
        if (auction.BuyNowPrice.HasValue && request.Amount >= auction.BuyNowPrice.Value)
        {
            // Buy now - instant win
            request.Amount = auction.BuyNowPrice.Value;
        }

        // Get previous high bidder for outbid notification
        var previousHighBid = await _bidRepository.GetHighestBidAsync(id, cancellationToken);
        var previousHighBidderId = previousHighBid?.Seeker?.UserId;

        // Clear previous winning flags
        await _bidRepository.ClearWinningFlagsAsync(id, cancellationToken);

        // Create bid
        var bid = new Bid
        {
            AuctionId = id,
            SeekerId = seeker.Id,
            Amount = request.Amount,
            IsProxyBid = request.IsProxyBid,
            MaxProxyAmount = request.MaxProxyAmount,
            PlacedAt = DateTime.UtcNow,
            IsWinning = true
        };

        await _bidRepository.CreateAsync(bid, cancellationToken);

        // Update auction high bid
        auction.CurrentHighBid = request.Amount;

        // If buy now, close auction
        var isBuyNow = auction.BuyNowPrice.HasValue && request.Amount >= auction.BuyNowPrice.Value;
        if (isBuyNow)
        {
            auction.Status = AuctionStatus.Closed;
            auction.WinningBidId = bid.Id;
        }

        await _auctionRepository.UpdateAsync(auction, cancellationToken);

        // Send real-time notifications
        var bidCount = await _bidRepository.GetBidCountAsync(id, cancellationToken);
        var bidNotification = new BidNotification
        {
            AuctionId = id,
            BidId = bid.Id,
            Amount = request.Amount,
            BidderInitials = GetInitials(seeker.User.FirstName, seeker.User.LastName),
            IsProxyBid = request.IsProxyBid,
            TotalBids = bidCount
        };
        await _notificationService.NotifyNewBidAsync(id, bidNotification, cancellationToken);

        // Notify previous high bidder they've been outbid
        if (previousHighBidderId.HasValue && previousHighBidderId.Value != seeker.UserId)
        {
            var outbidNotification = new OutbidNotification
            {
                AuctionId = id,
                AuctionTitle = auction.Title,
                YourBidAmount = previousHighBid!.Amount,
                NewHighBidAmount = request.Amount
            };
            await _notificationService.NotifyOutbidAsync(previousHighBidderId.Value, outbidNotification, cancellationToken);
        }

        // If buy now, notify winner and watchers
        if (isBuyNow)
        {
            var statusNotification = new AuctionStatusNotification
            {
                AuctionId = id,
                Title = auction.Title,
                Status = "Closed",
                WinnerName = $"{seeker.User.FirstName} {seeker.User.LastName}",
                WinningAmount = request.Amount
            };
            await _notificationService.NotifyAuctionStatusChangeAsync(id, statusNotification, cancellationToken);
            await _notificationService.NotifyAuctionWinnerAsync(seeker.UserId, statusNotification, cancellationToken);
        }

        // Reload bid with includes
        bid = await _bidRepository.GetByIdAsync(bid.Id, cancellationToken);
        return CreatedAtAction(nameof(GetBidHistory), new { id }, _mapper.Map<BidResponse>(bid));
    }

    /// <summary>
    /// Get bid history for an auction (anonymized)
    /// </summary>
    [HttpGet("{id:guid}/bids")]
    [ProducesResponseType(typeof(BidHistoryResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBidHistory(Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var auction = await _auctionRepository.GetByIdAsync(id, cancellationToken);
        if (auction == null)
        {
            return NotFound(new { message = "Auction not found" });
        }

        var bidsPage = await _bidRepository.GetByAuctionIdAsync(id, page, pageSize, cancellationToken);
        var highestBid = await _bidRepository.GetHighestBidAsync(id, cancellationToken);

        var response = new BidHistoryResponse
        {
            AuctionId = id,
            TotalBids = bidsPage.TotalCount,
            CurrentHighBid = auction.CurrentHighBid,
            RecentBids = bidsPage.Items.Select(b => new BidHistoryItem
            {
                Amount = b.Amount,
                PlacedAt = b.PlacedAt,
                BidderInitials = GetInitials(b.Seeker.User.FirstName, b.Seeker.User.LastName),
                IsCurrentHigh = b.Id == highestBid?.Id
            }).ToList()
        };

        return Ok(response);
    }

    /// <summary>
    /// Cancel an auction (Expert only, before it has bids)
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    [Authorize(Policy = "RequireExpertRole")]
    [ProducesResponseType(typeof(AuctionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        var auction = await _auctionRepository.GetByIdAsync(id, cancellationToken);
        if (auction == null)
        {
            return NotFound(new { message = "Auction not found" });
        }

        // Verify ownership
        var appUser = await GetCurrentAppUserAsync();
        if (appUser?.DomainUserId == null || auction.Expert.UserId != appUser.DomainUserId)
        {
            return Forbid();
        }

        // Can only cancel if no bids
        var bidCount = await _bidRepository.GetBidCountAsync(id, cancellationToken);
        if (bidCount > 0)
        {
            return BadRequest(new { message = "Cannot cancel auction with existing bids" });
        }

        auction.Status = AuctionStatus.Cancelled;
        await _auctionRepository.UpdateAsync(auction, cancellationToken);

        return Ok(_mapper.Map<AuctionResponse>(auction));
    }

    /// <summary>
    /// Open auction manually (for testing - normally done by scheduler)
    /// </summary>
    [HttpPost("{id:guid}/open")]
    [Authorize(Policy = "RequireExpertRole")]
    [ProducesResponseType(typeof(AuctionResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Open(Guid id, CancellationToken cancellationToken)
    {
        var auction = await _auctionRepository.GetByIdAsync(id, cancellationToken);
        if (auction == null)
        {
            return NotFound(new { message = "Auction not found" });
        }

        // Verify ownership
        var appUser = await GetCurrentAppUserAsync();
        if (appUser?.DomainUserId == null || auction.Expert.UserId != appUser.DomainUserId)
        {
            return Forbid();
        }

        if (auction.Status != AuctionStatus.Scheduled)
        {
            return BadRequest(new { message = "Only scheduled auctions can be opened" });
        }

        auction.Status = AuctionStatus.Open;
        await _auctionRepository.UpdateAsync(auction, cancellationToken);

        // Send notification that auction is now open
        var notification = new AuctionStatusNotification
        {
            AuctionId = id,
            Title = auction.Title,
            Status = "Open"
        };
        await _notificationService.NotifyAuctionStatusChangeAsync(id, notification, cancellationToken);

        return Ok(_mapper.Map<AuctionResponse>(auction));
    }

    /// <summary>
    /// Close auction and select winner
    /// </summary>
    [HttpPost("{id:guid}/close")]
    [Authorize(Policy = "RequireExpertRole")]
    [ProducesResponseType(typeof(AuctionResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Close(Guid id, CancellationToken cancellationToken)
    {
        var auction = await _auctionRepository.GetByIdAsync(id, cancellationToken);
        if (auction == null)
        {
            return NotFound(new { message = "Auction not found" });
        }

        // Verify ownership
        var appUser = await GetCurrentAppUserAsync();
        if (appUser?.DomainUserId == null || auction.Expert.UserId != appUser.DomainUserId)
        {
            return Forbid();
        }

        if (auction.Status != AuctionStatus.Open)
        {
            return BadRequest(new { message = "Only open auctions can be closed" });
        }

        // Find winning bid
        var winningBid = await _bidRepository.GetHighestBidAsync(id, cancellationToken);

        auction.Status = AuctionStatus.Closed;
        if (winningBid != null)
        {
            auction.WinningBidId = winningBid.Id;
            auction.Status = AuctionStatus.WinnerSelected;
        }

        await _auctionRepository.UpdateAsync(auction, cancellationToken);

        // Reload to get winner info
        auction = await _auctionRepository.GetByIdAsync(id, cancellationToken);

        // Send notifications
        var statusNotification = new AuctionStatusNotification
        {
            AuctionId = id,
            Title = auction!.Title,
            Status = auction.Status.ToString(),
            WinnerName = winningBid != null
                ? $"{winningBid.Seeker.User.FirstName} {winningBid.Seeker.User.LastName}"
                : null,
            WinningAmount = winningBid?.Amount
        };
        await _notificationService.NotifyAuctionStatusChangeAsync(id, statusNotification, cancellationToken);

        // Notify winner
        if (winningBid != null)
        {
            await _notificationService.NotifyAuctionWinnerAsync(winningBid.Seeker.UserId, statusNotification, cancellationToken);
        }

        return Ok(_mapper.Map<AuctionResponse>(auction));
    }

    private async Task<ApplicationUser?> GetCurrentAppUserAsync()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return null;
        }
        return await _userManager.FindByIdAsync(userId);
    }

    private static string GetInitials(string firstName, string lastName)
    {
        var first = string.IsNullOrEmpty(firstName) ? "" : firstName[0].ToString().ToUpper();
        var last = string.IsNullOrEmpty(lastName) ? "" : lastName[0].ToString().ToUpper();
        return $"{first}{last}";
    }
}

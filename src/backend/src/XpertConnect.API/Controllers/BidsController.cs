using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using XpertConnect.Application.Common.Interfaces;
using XpertConnect.Application.Common.Models;
using XpertConnect.Application.Features.Bids.DTOs;
using XpertConnect.Infrastructure.Identity;

namespace XpertConnect.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BidsController : ControllerBase
{
    private readonly IBidRepository _bidRepository;
    private readonly ISeekerRepository _seekerRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public BidsController(
        IBidRepository bidRepository,
        ISeekerRepository seekerRepository,
        UserManager<ApplicationUser> userManager,
        IMapper mapper)
    {
        _bidRepository = bidRepository;
        _seekerRepository = seekerRepository;
        _userManager = userManager;
        _mapper = mapper;
    }

    /// <summary>
    /// Get bid by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BidResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var bid = await _bidRepository.GetByIdAsync(id, cancellationToken);
        if (bid == null)
        {
            return NotFound(new { message = "Bid not found" });
        }

        // Verify access - only admin or bid owner
        var appUser = await GetCurrentAppUserAsync();
        if (appUser?.DomainUserId == null)
        {
            return Forbid();
        }

        var isAdmin = User.IsInRole("Admin");
        var seeker = await _seekerRepository.GetByUserIdAsync(appUser.DomainUserId.Value, cancellationToken);
        var isOwner = seeker != null && bid.SeekerId == seeker.Id;

        if (!isAdmin && !isOwner)
        {
            return Forbid();
        }

        return Ok(_mapper.Map<BidResponse>(bid));
    }

    /// <summary>
    /// Get current seeker's bids
    /// </summary>
    [HttpGet("my")]
    [Authorize(Policy = "RequireSeekerRole")]
    [ProducesResponseType(typeof(PagedResult<BidResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyBids([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
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

        var result = await _bidRepository.GetBySeekerIdAsync(seeker.Id, page, pageSize, cancellationToken);
        var responses = _mapper.Map<IReadOnlyList<BidResponse>>(result.Items);

        return Ok(PagedResult<BidResponse>.Create(
            responses,
            result.TotalCount,
            result.PageNumber,
            result.PageSize));
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
}

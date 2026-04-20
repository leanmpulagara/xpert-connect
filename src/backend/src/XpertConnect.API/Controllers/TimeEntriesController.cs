using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using XpertConnect.Application.Common.Interfaces;
using XpertConnect.Application.Common.Models;
using XpertConnect.Application.Features.TimeEntries.DTOs;
using XpertConnect.Domain.Entities;
using XpertConnect.Domain.Enums;
using XpertConnect.Infrastructure.Identity;

namespace XpertConnect.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TimeEntriesController : ControllerBase
{
    private readonly ITimeEntryRepository _timeEntryRepository;
    private readonly IProBonoProjectRepository _projectRepository;
    private readonly IExpertRepository _expertRepository;
    private readonly INonProfitOrgRepository _orgRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public TimeEntriesController(
        ITimeEntryRepository timeEntryRepository,
        IProBonoProjectRepository projectRepository,
        IExpertRepository expertRepository,
        INonProfitOrgRepository orgRepository,
        UserManager<ApplicationUser> userManager,
        IMapper mapper)
    {
        _timeEntryRepository = timeEntryRepository;
        _projectRepository = projectRepository;
        _expertRepository = expertRepository;
        _orgRepository = orgRepository;
        _userManager = userManager;
        _mapper = mapper;
    }

    /// <summary>
    /// Get time entries for a project
    /// </summary>
    [HttpGet("project/{projectId:guid}")]
    [ProducesResponseType(typeof(PagedResult<TimeEntryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByProject(Guid projectId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var project = await _projectRepository.GetByIdAsync(projectId, cancellationToken);
        if (project == null)
        {
            return NotFound(new { message = "Project not found" });
        }

        // Verify access - must be expert, org, or admin
        var appUser = await GetCurrentAppUserAsync();
        if (appUser?.DomainUserId == null)
        {
            return Forbid();
        }

        var isAdmin = User.IsInRole("Admin");
        var expert = await _expertRepository.GetByUserIdAsync(appUser.DomainUserId.Value, cancellationToken);
        var org = await _orgRepository.GetByUserIdAsync(appUser.DomainUserId.Value, cancellationToken);

        var isExpert = expert != null && project.ExpertId == expert.Id;
        var isOrg = org != null && project.OrgId == org.Id;

        if (!isAdmin && !isExpert && !isOrg)
        {
            return Forbid();
        }

        var result = await _timeEntryRepository.GetByProjectIdAsync(projectId, page, pageSize, cancellationToken);
        var responses = _mapper.Map<IReadOnlyList<TimeEntryResponse>>(result.Items);

        return Ok(PagedResult<TimeEntryResponse>.Create(
            responses,
            result.TotalCount,
            result.PageNumber,
            result.PageSize));
    }

    /// <summary>
    /// Get my time entries (Expert only)
    /// </summary>
    [HttpGet("my")]
    [Authorize(Policy = "RequireExpertRole")]
    [ProducesResponseType(typeof(PagedResult<TimeEntryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyTimeEntries([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
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

        var result = await _timeEntryRepository.GetByExpertIdAsync(expert.Id, page, pageSize, cancellationToken);
        var responses = _mapper.Map<IReadOnlyList<TimeEntryResponse>>(result.Items);

        return Ok(PagedResult<TimeEntryResponse>.Create(
            responses,
            result.TotalCount,
            result.PageNumber,
            result.PageSize));
    }

    /// <summary>
    /// Log time for a project (Expert only)
    /// </summary>
    [HttpPost("project/{projectId:guid}")]
    [Authorize(Policy = "RequireExpertRole")]
    [ProducesResponseType(typeof(TimeEntryResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LogTime(Guid projectId, [FromBody] CreateTimeEntryRequest request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdAsync(projectId, cancellationToken);
        if (project == null)
        {
            return NotFound(new { message = "Project not found" });
        }

        if (project.Status != ProjectStatus.InProgress)
        {
            return BadRequest(new { message = "Can only log time for in-progress projects" });
        }

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

        // Verify expert is assigned to project
        if (project.ExpertId != expert.Id)
        {
            return Forbid();
        }

        var timeEntry = new TimeEntry
        {
            ProjectId = projectId,
            ExpertId = expert.Id,
            Hours = request.Hours,
            Description = request.Description,
            LoggedAt = request.LoggedAt?.ToUniversalTime() ?? DateTime.UtcNow
        };

        await _timeEntryRepository.CreateAsync(timeEntry, cancellationToken);

        // Reload with navigation properties
        timeEntry = await _timeEntryRepository.GetByIdAsync(timeEntry.Id, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = timeEntry!.Id }, _mapper.Map<TimeEntryResponse>(timeEntry));
    }

    /// <summary>
    /// Get time entry by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TimeEntryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var timeEntry = await _timeEntryRepository.GetByIdAsync(id, cancellationToken);
        if (timeEntry == null)
        {
            return NotFound(new { message = "Time entry not found" });
        }

        // Verify access
        var appUser = await GetCurrentAppUserAsync();
        if (appUser?.DomainUserId == null)
        {
            return Forbid();
        }

        var isAdmin = User.IsInRole("Admin");
        var expert = await _expertRepository.GetByUserIdAsync(appUser.DomainUserId.Value, cancellationToken);
        var org = await _orgRepository.GetByUserIdAsync(appUser.DomainUserId.Value, cancellationToken);

        var isOwner = expert != null && timeEntry.ExpertId == expert.Id;
        var isOrg = org != null && timeEntry.Project.OrgId == org.Id;

        if (!isAdmin && !isOwner && !isOrg)
        {
            return Forbid();
        }

        return Ok(_mapper.Map<TimeEntryResponse>(timeEntry));
    }

    /// <summary>
    /// Update time entry (Expert only, own entries)
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "RequireExpertRole")]
    [ProducesResponseType(typeof(TimeEntryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateTimeEntryRequest request, CancellationToken cancellationToken)
    {
        var timeEntry = await _timeEntryRepository.GetByIdAsync(id, cancellationToken);
        if (timeEntry == null)
        {
            return NotFound(new { message = "Time entry not found" });
        }

        var appUser = await GetCurrentAppUserAsync();
        var expert = await _expertRepository.GetByUserIdAsync(appUser!.DomainUserId!.Value, cancellationToken);

        if (expert == null || timeEntry.ExpertId != expert.Id)
        {
            return Forbid();
        }

        // Can only update if project is still in progress
        var project = await _projectRepository.GetByIdAsync(timeEntry.ProjectId, cancellationToken);
        if (project?.Status != ProjectStatus.InProgress)
        {
            return BadRequest(new { message = "Cannot update time entry for completed/cancelled project" });
        }

        timeEntry.Hours = request.Hours;
        timeEntry.Description = request.Description;
        if (request.LoggedAt.HasValue)
        {
            timeEntry.LoggedAt = request.LoggedAt.Value.ToUniversalTime();
        }

        await _timeEntryRepository.UpdateAsync(timeEntry, cancellationToken);

        return Ok(_mapper.Map<TimeEntryResponse>(timeEntry));
    }

    /// <summary>
    /// Delete time entry (Expert only, own entries)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "RequireExpertRole")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var timeEntry = await _timeEntryRepository.GetByIdAsync(id, cancellationToken);
        if (timeEntry == null)
        {
            return NotFound(new { message = "Time entry not found" });
        }

        var appUser = await GetCurrentAppUserAsync();
        var expert = await _expertRepository.GetByUserIdAsync(appUser!.DomainUserId!.Value, cancellationToken);

        if (expert == null || timeEntry.ExpertId != expert.Id)
        {
            return Forbid();
        }

        // Can only delete if project is still in progress
        var project = await _projectRepository.GetByIdAsync(timeEntry.ProjectId, cancellationToken);
        if (project?.Status != ProjectStatus.InProgress)
        {
            return BadRequest(new { message = "Cannot delete time entry for completed/cancelled project" });
        }

        await _timeEntryRepository.DeleteAsync(id, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Get CSR hours summary for current expert
    /// </summary>
    [HttpGet("summary")]
    [Authorize(Policy = "RequireExpertRole")]
    [ProducesResponseType(typeof(ExpertHoursSummary), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMySummary(CancellationToken cancellationToken)
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

        var summary = await _timeEntryRepository.GetExpertHoursSummaryAsync(expert.Id, cancellationToken);

        return Ok(summary);
    }

    /// <summary>
    /// Get total hours for a project
    /// </summary>
    [HttpGet("project/{projectId:guid}/total")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProjectTotalHours(Guid projectId, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdAsync(projectId, cancellationToken);
        if (project == null)
        {
            return NotFound(new { message = "Project not found" });
        }

        var totalHours = await _timeEntryRepository.GetTotalHoursByProjectAsync(projectId, cancellationToken);

        return Ok(new
        {
            projectId,
            projectTitle = project.Title,
            estimatedHours = project.EstimatedHours,
            actualHours = totalHours,
            progressPercentage = project.EstimatedHours > 0
                ? Math.Min(100, Math.Round((totalHours / project.EstimatedHours) * 100, 1))
                : 0
        });
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

using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using XpertConnect.Application.Common.Interfaces;
using XpertConnect.Application.Common.Models;
using XpertConnect.Application.Features.Projects.DTOs;
using XpertConnect.Domain.Entities;
using XpertConnect.Domain.Enums;
using XpertConnect.Infrastructure.Identity;

namespace XpertConnect.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IProBonoProjectRepository _projectRepository;
    private readonly INonProfitOrgRepository _orgRepository;
    private readonly IExpertRepository _expertRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public ProjectsController(
        IProBonoProjectRepository projectRepository,
        INonProfitOrgRepository orgRepository,
        IExpertRepository expertRepository,
        UserManager<ApplicationUser> userManager,
        IMapper mapper)
    {
        _projectRepository = projectRepository;
        _orgRepository = orgRepository;
        _expertRepository = expertRepository;
        _userManager = userManager;
        _mapper = mapper;
    }

    /// <summary>
    /// Browse all projects (public for open projects)
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PagedResult<ProjectListResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] ProjectQueryParams queryParams, CancellationToken cancellationToken)
    {
        // Anonymous users can only see open projects
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            queryParams.Status = ProjectStatus.Open;
        }

        var result = await _projectRepository.GetAllAsync(queryParams, cancellationToken);
        var responses = _mapper.Map<IReadOnlyList<ProjectListResponse>>(result.Items);

        return Ok(PagedResult<ProjectListResponse>.Create(
            responses,
            result.TotalCount,
            result.PageNumber,
            result.PageSize));
    }

    /// <summary>
    /// Get project by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdAsync(id, cancellationToken);
        if (project == null)
        {
            return NotFound(new { message = "Project not found" });
        }

        return Ok(_mapper.Map<ProjectResponse>(project));
    }

    /// <summary>
    /// Get my organization's projects (NonProfit only)
    /// </summary>
    [HttpGet("my-org")]
    [Authorize(Policy = "RequireNonProfitRole")]
    [ProducesResponseType(typeof(PagedResult<ProjectResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyOrgProjects([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var appUser = await GetCurrentAppUserAsync();
        if (appUser?.DomainUserId == null)
        {
            return NotFound(new { message = "User profile not found" });
        }

        var org = await _orgRepository.GetByUserIdAsync(appUser.DomainUserId.Value, cancellationToken);
        if (org == null)
        {
            return NotFound(new { message = "Organization not found" });
        }

        var result = await _projectRepository.GetByOrgIdAsync(org.Id, page, pageSize, cancellationToken);
        var responses = _mapper.Map<IReadOnlyList<ProjectResponse>>(result.Items);

        return Ok(PagedResult<ProjectResponse>.Create(
            responses,
            result.TotalCount,
            result.PageNumber,
            result.PageSize));
    }

    /// <summary>
    /// Get my projects (Expert only)
    /// </summary>
    [HttpGet("my")]
    [Authorize(Policy = "RequireExpertRole")]
    [ProducesResponseType(typeof(PagedResult<ProjectResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyProjects([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
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

        var result = await _projectRepository.GetByExpertIdAsync(expert.Id, page, pageSize, cancellationToken);
        var responses = _mapper.Map<IReadOnlyList<ProjectResponse>>(result.Items);

        return Ok(PagedResult<ProjectResponse>.Create(
            responses,
            result.TotalCount,
            result.PageNumber,
            result.PageSize));
    }

    /// <summary>
    /// Create a new project (NonProfit only)
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "RequireNonProfitRole")]
    [ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateProjectRequest request, CancellationToken cancellationToken)
    {
        var appUser = await GetCurrentAppUserAsync();
        if (appUser?.DomainUserId == null)
        {
            return NotFound(new { message = "User profile not found" });
        }

        var org = await _orgRepository.GetByUserIdAsync(appUser.DomainUserId.Value, cancellationToken);
        if (org == null)
        {
            return NotFound(new { message = "Organization not found" });
        }

        var project = new ProBonoProject
        {
            OrgId = org.Id,
            Title = request.Title,
            Description = request.Description,
            Deliverables = request.Deliverables,
            EstimatedHours = request.EstimatedHours,
            StartDate = request.StartDate?.ToUniversalTime(),
            EndDate = request.EndDate?.ToUniversalTime(),
            Status = ProjectStatus.Draft
        };

        await _projectRepository.CreateAsync(project, cancellationToken);

        // Reload with navigation properties
        project = await _projectRepository.GetByIdAsync(project.Id, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = project!.Id }, _mapper.Map<ProjectResponse>(project));
    }

    /// <summary>
    /// Update a project (NonProfit only, own projects)
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "RequireNonProfitRole")]
    [ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProjectRequest request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdAsync(id, cancellationToken);
        if (project == null)
        {
            return NotFound(new { message = "Project not found" });
        }

        // Verify ownership
        var appUser = await GetCurrentAppUserAsync();
        if (appUser?.DomainUserId == null)
        {
            return Forbid();
        }

        var org = await _orgRepository.GetByUserIdAsync(appUser.DomainUserId.Value, cancellationToken);
        if (org == null || project.OrgId != org.Id)
        {
            return Forbid();
        }

        // Can only update draft or open projects
        if (project.Status != ProjectStatus.Draft && project.Status != ProjectStatus.Open)
        {
            return BadRequest(new { message = "Cannot update project in current status" });
        }

        // Update fields
        if (request.Title != null) project.Title = request.Title;
        if (request.Description != null) project.Description = request.Description;
        if (request.Deliverables != null) project.Deliverables = request.Deliverables;
        if (request.EstimatedHours.HasValue) project.EstimatedHours = request.EstimatedHours.Value;
        if (request.StartDate.HasValue) project.StartDate = request.StartDate.Value.ToUniversalTime();
        if (request.EndDate.HasValue) project.EndDate = request.EndDate.Value.ToUniversalTime();

        await _projectRepository.UpdateAsync(project, cancellationToken);

        return Ok(_mapper.Map<ProjectResponse>(project));
    }

    /// <summary>
    /// Publish a project (make it open for applications)
    /// </summary>
    [HttpPost("{id:guid}/publish")]
    [Authorize(Policy = "RequireNonProfitRole")]
    [ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Publish(Guid id, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdAsync(id, cancellationToken);
        if (project == null)
        {
            return NotFound(new { message = "Project not found" });
        }

        // Verify ownership
        var appUser = await GetCurrentAppUserAsync();
        var org = await _orgRepository.GetByUserIdAsync(appUser!.DomainUserId!.Value, cancellationToken);
        if (org == null || project.OrgId != org.Id)
        {
            return Forbid();
        }

        if (project.Status != ProjectStatus.Draft)
        {
            return BadRequest(new { message = "Only draft projects can be published" });
        }

        await _projectRepository.UpdateStatusAsync(id, ProjectStatus.Open, cancellationToken);
        project.Status = ProjectStatus.Open;

        return Ok(_mapper.Map<ProjectResponse>(project));
    }

    /// <summary>
    /// Expert applies to a project
    /// </summary>
    [HttpPost("{id:guid}/apply")]
    [Authorize(Policy = "RequireExpertRole")]
    [ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Apply(Guid id, [FromBody] ApplyToProjectRequest request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdAsync(id, cancellationToken);
        if (project == null)
        {
            return NotFound(new { message = "Project not found" });
        }

        if (project.Status != ProjectStatus.Open)
        {
            return BadRequest(new { message = "Project is not open for applications" });
        }

        if (project.ExpertId.HasValue)
        {
            return BadRequest(new { message = "Project already has an assigned expert" });
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

        // Assign expert to project
        await _projectRepository.AssignExpertAsync(id, expert.Id, cancellationToken);

        // Reload project
        project = await _projectRepository.GetByIdAsync(id, cancellationToken);

        return Ok(_mapper.Map<ProjectResponse>(project));
    }

    /// <summary>
    /// Accept expert application (NonProfit only)
    /// </summary>
    [HttpPost("{id:guid}/accept")]
    [Authorize(Policy = "RequireNonProfitRole")]
    [ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> AcceptExpert(Guid id, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdAsync(id, cancellationToken);
        if (project == null)
        {
            return NotFound(new { message = "Project not found" });
        }

        var appUser = await GetCurrentAppUserAsync();
        var org = await _orgRepository.GetByUserIdAsync(appUser!.DomainUserId!.Value, cancellationToken);
        if (org == null || project.OrgId != org.Id)
        {
            return Forbid();
        }

        if (project.Status != ProjectStatus.Matching)
        {
            return BadRequest(new { message = "No pending expert application" });
        }

        await _projectRepository.UpdateStatusAsync(id, ProjectStatus.InProgress, cancellationToken);
        project.Status = ProjectStatus.InProgress;
        project.StartDate = DateTime.UtcNow;

        await _projectRepository.UpdateAsync(project, cancellationToken);

        return Ok(_mapper.Map<ProjectResponse>(project));
    }

    /// <summary>
    /// Reject expert application (NonProfit only)
    /// </summary>
    [HttpPost("{id:guid}/reject")]
    [Authorize(Policy = "RequireNonProfitRole")]
    [ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> RejectExpert(Guid id, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdAsync(id, cancellationToken);
        if (project == null)
        {
            return NotFound(new { message = "Project not found" });
        }

        var appUser = await GetCurrentAppUserAsync();
        var org = await _orgRepository.GetByUserIdAsync(appUser!.DomainUserId!.Value, cancellationToken);
        if (org == null || project.OrgId != org.Id)
        {
            return Forbid();
        }

        if (project.Status != ProjectStatus.Matching)
        {
            return BadRequest(new { message = "No pending expert application" });
        }

        await _projectRepository.RemoveExpertAsync(id, cancellationToken);

        // Reload project
        project = await _projectRepository.GetByIdAsync(id, cancellationToken);

        return Ok(_mapper.Map<ProjectResponse>(project));
    }

    /// <summary>
    /// Start project (Expert only)
    /// </summary>
    [HttpPost("{id:guid}/start")]
    [Authorize(Policy = "RequireExpertRole")]
    [ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Start(Guid id, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdAsync(id, cancellationToken);
        if (project == null)
        {
            return NotFound(new { message = "Project not found" });
        }

        var appUser = await GetCurrentAppUserAsync();
        var expert = await _expertRepository.GetByUserIdAsync(appUser!.DomainUserId!.Value, cancellationToken);
        if (expert == null || project.ExpertId != expert.Id)
        {
            return Forbid();
        }

        if (project.Status != ProjectStatus.InProgress)
        {
            return BadRequest(new { message = "Project must be in progress to start" });
        }

        project.StartDate = DateTime.UtcNow;
        await _projectRepository.UpdateAsync(project, cancellationToken);

        return Ok(_mapper.Map<ProjectResponse>(project));
    }

    /// <summary>
    /// Complete project
    /// </summary>
    [HttpPost("{id:guid}/complete")]
    [Authorize]
    [ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Complete(Guid id, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdAsync(id, cancellationToken);
        if (project == null)
        {
            return NotFound(new { message = "Project not found" });
        }

        var appUser = await GetCurrentAppUserAsync();
        var isAdmin = User.IsInRole("Admin");

        // Check if user is the expert or org owner
        var expert = await _expertRepository.GetByUserIdAsync(appUser!.DomainUserId!.Value, cancellationToken);
        var org = await _orgRepository.GetByUserIdAsync(appUser.DomainUserId.Value, cancellationToken);

        var isExpert = expert != null && project.ExpertId == expert.Id;
        var isOrg = org != null && project.OrgId == org.Id;

        if (!isAdmin && !isExpert && !isOrg)
        {
            return Forbid();
        }

        if (project.Status != ProjectStatus.InProgress)
        {
            return BadRequest(new { message = "Only in-progress projects can be completed" });
        }

        project.Status = ProjectStatus.Completed;
        project.EndDate = DateTime.UtcNow;
        await _projectRepository.UpdateAsync(project, cancellationToken);

        return Ok(_mapper.Map<ProjectResponse>(project));
    }

    /// <summary>
    /// Cancel project
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    [Authorize]
    [ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdAsync(id, cancellationToken);
        if (project == null)
        {
            return NotFound(new { message = "Project not found" });
        }

        var appUser = await GetCurrentAppUserAsync();
        var isAdmin = User.IsInRole("Admin");

        var org = await _orgRepository.GetByUserIdAsync(appUser!.DomainUserId!.Value, cancellationToken);
        var isOrg = org != null && project.OrgId == org.Id;

        if (!isAdmin && !isOrg)
        {
            return Forbid();
        }

        if (project.Status == ProjectStatus.Completed || project.Status == ProjectStatus.Cancelled)
        {
            return BadRequest(new { message = "Cannot cancel project in current status" });
        }

        await _projectRepository.UpdateStatusAsync(id, ProjectStatus.Cancelled, cancellationToken);
        project.Status = ProjectStatus.Cancelled;

        return Ok(_mapper.Map<ProjectResponse>(project));
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

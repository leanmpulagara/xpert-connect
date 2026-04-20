using Microsoft.EntityFrameworkCore;
using XpertConnect.Application.Common.Interfaces;
using XpertConnect.Application.Common.Models;
using XpertConnect.Application.Features.Projects.DTOs;
using XpertConnect.Domain.Entities;
using XpertConnect.Domain.Enums;
using XpertConnect.Infrastructure.Data;

namespace XpertConnect.Infrastructure.Repositories;

public class ProBonoProjectRepository : IProBonoProjectRepository
{
    private readonly ApplicationDbContext _context;

    public ProBonoProjectRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProBonoProject?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ProBonoProjects
            .Include(p => p.Org)
            .Include(p => p.Expert)
                .ThenInclude(e => e!.User)
            .Include(p => p.Mou)
            .Include(p => p.TimeEntries)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<PagedResult<ProBonoProject>> GetAllAsync(ProjectQueryParams queryParams, CancellationToken cancellationToken = default)
    {
        var query = _context.ProBonoProjects
            .Include(p => p.Org)
            .Include(p => p.Expert)
                .ThenInclude(e => e!.User)
            .AsQueryable();

        // Apply filters
        if (queryParams.Status.HasValue)
        {
            query = query.Where(p => p.Status == queryParams.Status.Value);
        }

        if (queryParams.OrgId.HasValue)
        {
            query = query.Where(p => p.OrgId == queryParams.OrgId.Value);
        }

        if (queryParams.ExpertId.HasValue)
        {
            query = query.Where(p => p.ExpertId == queryParams.ExpertId.Value);
        }

        if (queryParams.OpenForApplication == true)
        {
            query = query.Where(p => p.Status == ProjectStatus.Open && p.ExpertId == null);
        }

        if (!string.IsNullOrWhiteSpace(queryParams.SearchTerm))
        {
            var searchLower = queryParams.SearchTerm.ToLower();
            query = query.Where(p =>
                p.Title.ToLower().Contains(searchLower) ||
                (p.Description != null && p.Description.ToLower().Contains(searchLower)) ||
                p.Org.OrgName.ToLower().Contains(searchLower));
        }

        // Apply sorting
        query = queryParams.SortBy?.ToLower() switch
        {
            "title" => queryParams.SortDescending
                ? query.OrderByDescending(p => p.Title)
                : query.OrderBy(p => p.Title),
            "estimatedhours" => queryParams.SortDescending
                ? query.OrderByDescending(p => p.EstimatedHours)
                : query.OrderBy(p => p.EstimatedHours),
            "startdate" => queryParams.SortDescending
                ? query.OrderByDescending(p => p.StartDate)
                : query.OrderBy(p => p.StartDate),
            "status" => queryParams.SortDescending
                ? query.OrderByDescending(p => p.Status)
                : query.OrderBy(p => p.Status),
            _ => queryParams.SortDescending
                ? query.OrderByDescending(p => p.CreatedAt)
                : query.OrderBy(p => p.CreatedAt)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((queryParams.Page - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .ToListAsync(cancellationToken);

        return PagedResult<ProBonoProject>.Create(items, totalCount, queryParams.Page, queryParams.PageSize);
    }

    public async Task<PagedResult<ProBonoProject>> GetByOrgIdAsync(Guid orgId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.ProBonoProjects
            .Include(p => p.Org)
            .Include(p => p.Expert)
                .ThenInclude(e => e!.User)
            .Where(p => p.OrgId == orgId)
            .OrderByDescending(p => p.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PagedResult<ProBonoProject>.Create(items, totalCount, page, pageSize);
    }

    public async Task<PagedResult<ProBonoProject>> GetByExpertIdAsync(Guid expertId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.ProBonoProjects
            .Include(p => p.Org)
            .Include(p => p.Expert)
                .ThenInclude(e => e!.User)
            .Where(p => p.ExpertId == expertId)
            .OrderByDescending(p => p.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PagedResult<ProBonoProject>.Create(items, totalCount, page, pageSize);
    }

    public async Task<IList<ProBonoProject>> GetOpenProjectsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ProBonoProjects
            .Include(p => p.Org)
            .Where(p => p.Status == ProjectStatus.Open && p.ExpertId == null)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<ProBonoProject> CreateAsync(ProBonoProject project, CancellationToken cancellationToken = default)
    {
        project.CreatedAt = DateTime.UtcNow;
        _context.ProBonoProjects.Add(project);
        await _context.SaveChangesAsync(cancellationToken);
        return project;
    }

    public async Task UpdateAsync(ProBonoProject project, CancellationToken cancellationToken = default)
    {
        project.UpdatedAt = DateTime.UtcNow;
        _context.ProBonoProjects.Update(project);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> UpdateStatusAsync(Guid id, ProjectStatus status, CancellationToken cancellationToken = default)
    {
        var project = await _context.ProBonoProjects.FindAsync(new object[] { id }, cancellationToken);
        if (project == null) return false;

        project.Status = status;
        project.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> AssignExpertAsync(Guid projectId, Guid expertId, CancellationToken cancellationToken = default)
    {
        var project = await _context.ProBonoProjects.FindAsync(new object[] { projectId }, cancellationToken);
        if (project == null) return false;

        project.ExpertId = expertId;
        project.Status = ProjectStatus.Matching;
        project.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> RemoveExpertAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var project = await _context.ProBonoProjects.FindAsync(new object[] { projectId }, cancellationToken);
        if (project == null) return false;

        project.ExpertId = null;
        project.Status = ProjectStatus.Open;
        project.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<decimal> GetTotalHoursAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return await _context.TimeEntries
            .Where(t => t.ProjectId == projectId)
            .SumAsync(t => t.Hours, cancellationToken);
    }
}

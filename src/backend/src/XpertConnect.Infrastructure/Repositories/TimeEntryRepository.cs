using Microsoft.EntityFrameworkCore;
using XpertConnect.Application.Common.Interfaces;
using XpertConnect.Application.Common.Models;
using XpertConnect.Application.Features.TimeEntries.DTOs;
using XpertConnect.Domain.Entities;
using XpertConnect.Domain.Enums;
using XpertConnect.Infrastructure.Data;

namespace XpertConnect.Infrastructure.Repositories;

public class TimeEntryRepository : ITimeEntryRepository
{
    private readonly ApplicationDbContext _context;

    public TimeEntryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TimeEntry?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.TimeEntries
            .Include(t => t.Project)
                .ThenInclude(p => p.Org)
            .Include(t => t.Expert)
                .ThenInclude(e => e.User)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<PagedResult<TimeEntry>> GetByProjectIdAsync(Guid projectId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.TimeEntries
            .Include(t => t.Expert)
                .ThenInclude(e => e.User)
            .Where(t => t.ProjectId == projectId)
            .OrderByDescending(t => t.LoggedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PagedResult<TimeEntry>.Create(items, totalCount, page, pageSize);
    }

    public async Task<PagedResult<TimeEntry>> GetByExpertIdAsync(Guid expertId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.TimeEntries
            .Include(t => t.Project)
                .ThenInclude(p => p.Org)
            .Where(t => t.ExpertId == expertId)
            .OrderByDescending(t => t.LoggedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PagedResult<TimeEntry>.Create(items, totalCount, page, pageSize);
    }

    public async Task<TimeEntry> CreateAsync(TimeEntry timeEntry, CancellationToken cancellationToken = default)
    {
        timeEntry.CreatedAt = DateTime.UtcNow;
        _context.TimeEntries.Add(timeEntry);
        await _context.SaveChangesAsync(cancellationToken);

        // Update project actual hours
        await UpdateProjectActualHoursAsync(timeEntry.ProjectId, cancellationToken);

        return timeEntry;
    }

    public async Task UpdateAsync(TimeEntry timeEntry, CancellationToken cancellationToken = default)
    {
        timeEntry.UpdatedAt = DateTime.UtcNow;
        _context.TimeEntries.Update(timeEntry);
        await _context.SaveChangesAsync(cancellationToken);

        // Update project actual hours
        await UpdateProjectActualHoursAsync(timeEntry.ProjectId, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var timeEntry = await _context.TimeEntries.FindAsync(new object[] { id }, cancellationToken);
        if (timeEntry != null)
        {
            var projectId = timeEntry.ProjectId;
            _context.TimeEntries.Remove(timeEntry);
            await _context.SaveChangesAsync(cancellationToken);

            // Update project actual hours
            await UpdateProjectActualHoursAsync(projectId, cancellationToken);
        }
    }

    public async Task<decimal> GetTotalHoursByProjectAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return await _context.TimeEntries
            .Where(t => t.ProjectId == projectId)
            .SumAsync(t => t.Hours, cancellationToken);
    }

    public async Task<decimal> GetTotalHoursByExpertAsync(Guid expertId, CancellationToken cancellationToken = default)
    {
        return await _context.TimeEntries
            .Where(t => t.ExpertId == expertId)
            .SumAsync(t => t.Hours, cancellationToken);
    }

    public async Task<ExpertHoursSummary> GetExpertHoursSummaryAsync(Guid expertId, CancellationToken cancellationToken = default)
    {
        var expert = await _context.Experts
            .Include(e => e.User)
            .FirstOrDefaultAsync(e => e.Id == expertId, cancellationToken);

        if (expert == null)
        {
            return new ExpertHoursSummary { ExpertId = expertId };
        }

        var projects = await _context.ProBonoProjects
            .Include(p => p.Org)
            .Include(p => p.TimeEntries.Where(t => t.ExpertId == expertId))
            .Where(p => p.ExpertId == expertId)
            .ToListAsync(cancellationToken);

        var projectSummaries = projects.Select(p => new ProjectHoursSummary
        {
            ProjectId = p.Id,
            ProjectTitle = p.Title,
            OrgName = p.Org.OrgName,
            TotalHours = p.TimeEntries.Sum(t => t.Hours),
            EntryCount = p.TimeEntries.Count,
            FirstEntry = p.TimeEntries.OrderBy(t => t.LoggedAt).FirstOrDefault()?.LoggedAt,
            LastEntry = p.TimeEntries.OrderByDescending(t => t.LoggedAt).FirstOrDefault()?.LoggedAt
        }).ToList();

        return new ExpertHoursSummary
        {
            ExpertId = expertId,
            ExpertName = $"{expert.User.FirstName} {expert.User.LastName}",
            TotalHours = projectSummaries.Sum(p => p.TotalHours),
            ProjectsCount = projects.Count,
            CompletedProjectsCount = projects.Count(p => p.Status == ProjectStatus.Completed),
            Projects = projectSummaries
        };
    }

    private async Task UpdateProjectActualHoursAsync(Guid projectId, CancellationToken cancellationToken)
    {
        var totalHours = await GetTotalHoursByProjectAsync(projectId, cancellationToken);
        var project = await _context.ProBonoProjects.FindAsync(new object[] { projectId }, cancellationToken);
        if (project != null)
        {
            project.ActualHours = totalHours;
            project.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}

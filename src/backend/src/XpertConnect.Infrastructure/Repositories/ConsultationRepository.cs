using Microsoft.EntityFrameworkCore;
using XpertConnect.Application.Common.Interfaces;
using XpertConnect.Application.Common.Models;
using XpertConnect.Application.Features.Consultations.DTOs;
using XpertConnect.Domain.Entities;
using XpertConnect.Domain.Enums;
using XpertConnect.Infrastructure.Data;

namespace XpertConnect.Infrastructure.Repositories;

public class ConsultationRepository : IConsultationRepository
{
    private readonly ApplicationDbContext _context;

    public ConsultationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Consultation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Consultations
            .Include(c => c.Expert)
                .ThenInclude(e => e.User)
            .Include(c => c.Seeker)
                .ThenInclude(s => s.User)
            .Include(c => c.Feedback)
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, cancellationToken);
    }

    public async Task<PagedResult<Consultation>> GetByExpertIdAsync(Guid expertId, ConsultationQueryParams queryParams, CancellationToken cancellationToken = default)
    {
        var query = _context.Consultations
            .Include(c => c.Expert)
                .ThenInclude(e => e.User)
            .Include(c => c.Seeker)
                .ThenInclude(s => s.User)
            .Include(c => c.Feedback)
            .Where(c => c.ExpertId == expertId && !c.IsDeleted)
            .AsQueryable();

        query = ApplyFilters(query, queryParams);
        return await ExecutePagedQueryAsync(query, queryParams, cancellationToken);
    }

    public async Task<PagedResult<Consultation>> GetBySeekerIdAsync(Guid seekerId, ConsultationQueryParams queryParams, CancellationToken cancellationToken = default)
    {
        var query = _context.Consultations
            .Include(c => c.Expert)
                .ThenInclude(e => e.User)
            .Include(c => c.Seeker)
                .ThenInclude(s => s.User)
            .Include(c => c.Feedback)
            .Where(c => c.SeekerId == seekerId && !c.IsDeleted)
            .AsQueryable();

        query = ApplyFilters(query, queryParams);
        return await ExecutePagedQueryAsync(query, queryParams, cancellationToken);
    }

    public async Task<Consultation> CreateAsync(Consultation consultation, CancellationToken cancellationToken = default)
    {
        _context.Consultations.Add(consultation);
        await _context.SaveChangesAsync(cancellationToken);
        return consultation;
    }

    public async Task UpdateAsync(Consultation consultation, CancellationToken cancellationToken = default)
    {
        _context.Consultations.Update(consultation);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> UpdateStatusAsync(Guid id, BookingStatus status, CancellationToken cancellationToken = default)
    {
        var consultation = await _context.Consultations.FindAsync(new object[] { id }, cancellationToken);
        if (consultation == null || consultation.IsDeleted)
        {
            return false;
        }

        consultation.Status = status;
        if (status == BookingStatus.Completed)
        {
            consultation.CompletedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> HasOverlappingBookingAsync(Guid expertId, DateTime scheduledAt, int durationMinutes, Guid? excludeConsultationId = null, CancellationToken cancellationToken = default)
    {
        var scheduledEnd = scheduledAt.AddMinutes(durationMinutes);

        var query = _context.Consultations
            .Where(c => c.ExpertId == expertId
                && !c.IsDeleted
                && c.Status != BookingStatus.Cancelled
                && c.Status != BookingStatus.Refunded);

        if (excludeConsultationId.HasValue)
        {
            query = query.Where(c => c.Id != excludeConsultationId.Value);
        }

        return await query.AnyAsync(c =>
            (scheduledAt >= c.ScheduledAt && scheduledAt < c.ScheduledAt.AddMinutes(c.DurationMinutes)) ||
            (scheduledEnd > c.ScheduledAt && scheduledEnd <= c.ScheduledAt.AddMinutes(c.DurationMinutes)) ||
            (scheduledAt <= c.ScheduledAt && scheduledEnd >= c.ScheduledAt.AddMinutes(c.DurationMinutes)),
            cancellationToken);
    }

    private static IQueryable<Consultation> ApplyFilters(IQueryable<Consultation> query, ConsultationQueryParams queryParams)
    {
        if (queryParams.Status.HasValue)
        {
            query = query.Where(c => c.Status == queryParams.Status.Value);
        }

        if (queryParams.MeetingType.HasValue)
        {
            query = query.Where(c => c.MeetingType == queryParams.MeetingType.Value);
        }

        if (queryParams.FromDate.HasValue)
        {
            query = query.Where(c => c.ScheduledAt >= queryParams.FromDate.Value);
        }

        if (queryParams.ToDate.HasValue)
        {
            query = query.Where(c => c.ScheduledAt <= queryParams.ToDate.Value);
        }

        return query;
    }

    private static async Task<PagedResult<Consultation>> ExecutePagedQueryAsync(
        IQueryable<Consultation> query,
        ConsultationQueryParams queryParams,
        CancellationToken cancellationToken)
    {
        var totalCount = await query.CountAsync(cancellationToken);

        query = queryParams.SortBy?.ToLower() switch
        {
            "status" => queryParams.SortDescending
                ? query.OrderByDescending(c => c.Status)
                : query.OrderBy(c => c.Status),
            "rate" => queryParams.SortDescending
                ? query.OrderByDescending(c => c.Rate)
                : query.OrderBy(c => c.Rate),
            "createdat" => queryParams.SortDescending
                ? query.OrderByDescending(c => c.CreatedAt)
                : query.OrderBy(c => c.CreatedAt),
            _ => queryParams.SortDescending
                ? query.OrderByDescending(c => c.ScheduledAt)
                : query.OrderBy(c => c.ScheduledAt)
        };

        var items = await query
            .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .ToListAsync(cancellationToken);

        return PagedResult<Consultation>.Create(items, totalCount, queryParams.PageNumber, queryParams.PageSize);
    }
}

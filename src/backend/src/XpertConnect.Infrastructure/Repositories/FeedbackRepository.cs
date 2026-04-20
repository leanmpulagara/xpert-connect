using Microsoft.EntityFrameworkCore;
using XpertConnect.Application.Common.Interfaces;
using XpertConnect.Application.Common.Models;
using XpertConnect.Domain.Entities;
using XpertConnect.Infrastructure.Data;

namespace XpertConnect.Infrastructure.Repositories;

public class FeedbackRepository : IFeedbackRepository
{
    private readonly ApplicationDbContext _context;

    public FeedbackRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Feedback?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Feedbacks
            .Include(f => f.Seeker)
                .ThenInclude(s => s.User)
            .Include(f => f.Consultation)
            .FirstOrDefaultAsync(f => f.Id == id && !f.IsDeleted, cancellationToken);
    }

    public async Task<Feedback?> GetByConsultationIdAsync(Guid consultationId, CancellationToken cancellationToken = default)
    {
        return await _context.Feedbacks
            .Include(f => f.Seeker)
                .ThenInclude(s => s.User)
            .FirstOrDefaultAsync(f => f.ConsultationId == consultationId && !f.IsDeleted, cancellationToken);
    }

    public async Task<PagedResult<Feedback>> GetByExpertIdAsync(Guid expertId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.Feedbacks
            .Include(f => f.Seeker)
                .ThenInclude(s => s.User)
            .Include(f => f.Consultation)
            .Where(f => f.Consultation.ExpertId == expertId && !f.IsDeleted)
            .OrderByDescending(f => f.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PagedResult<Feedback>.Create(items, totalCount, pageNumber, pageSize);
    }

    public async Task<Feedback> CreateAsync(Feedback feedback, CancellationToken cancellationToken = default)
    {
        _context.Feedbacks.Add(feedback);
        await _context.SaveChangesAsync(cancellationToken);
        return feedback;
    }

    public async Task<(double AverageRating, int TotalCount)> GetExpertRatingSummaryAsync(Guid expertId, CancellationToken cancellationToken = default)
    {
        var feedbacks = await _context.Feedbacks
            .Where(f => f.Consultation.ExpertId == expertId && !f.IsDeleted)
            .Select(f => f.Rating)
            .ToListAsync(cancellationToken);

        if (feedbacks.Count == 0)
        {
            return (0, 0);
        }

        return (feedbacks.Average(), feedbacks.Count);
    }

    public async Task<Dictionary<int, int>> GetExpertRatingDistributionAsync(Guid expertId, CancellationToken cancellationToken = default)
    {
        var distribution = await _context.Feedbacks
            .Where(f => f.Consultation.ExpertId == expertId && !f.IsDeleted)
            .GroupBy(f => f.Rating)
            .Select(g => new { Rating = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var result = new Dictionary<int, int>
        {
            { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 }
        };

        foreach (var item in distribution)
        {
            result[item.Rating] = item.Count;
        }

        return result;
    }
}

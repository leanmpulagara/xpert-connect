using Microsoft.EntityFrameworkCore;
using XpertConnect.Application.Common.Interfaces;
using XpertConnect.Application.Common.Models;
using XpertConnect.Application.Features.Experts.DTOs;
using XpertConnect.Domain.Entities;
using XpertConnect.Infrastructure.Data;

namespace XpertConnect.Infrastructure.Repositories;

public class ExpertRepository : IExpertRepository
{
    private readonly ApplicationDbContext _context;

    public ExpertRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Expert?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Experts
            .Include(e => e.User)
            .Include(e => e.Credentials.Where(c => !c.IsDeleted))
            .Include(e => e.Availabilities)
            .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, cancellationToken);
    }

    public async Task<Expert?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Experts
            .Include(e => e.User)
            .Include(e => e.Credentials.Where(c => !c.IsDeleted))
            .Include(e => e.Availabilities)
            .FirstOrDefaultAsync(e => e.UserId == userId && !e.IsDeleted, cancellationToken);
    }

    public async Task<PagedResult<Expert>> GetAllAsync(ExpertQueryParams queryParams, CancellationToken cancellationToken = default)
    {
        var query = _context.Experts
            .Include(e => e.User)
            .Include(e => e.Credentials.Where(c => !c.IsDeleted))
            .Where(e => !e.IsDeleted && !e.User.IsDeleted)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(queryParams.SearchTerm))
        {
            var searchTerm = queryParams.SearchTerm.ToLower();
            query = query.Where(e =>
                e.User.FirstName.ToLower().Contains(searchTerm) ||
                e.User.LastName.ToLower().Contains(searchTerm) ||
                (e.Headline != null && e.Headline.ToLower().Contains(searchTerm)) ||
                (e.Bio != null && e.Bio.ToLower().Contains(searchTerm)));
        }

        if (queryParams.Category.HasValue)
        {
            query = query.Where(e => e.Category == queryParams.Category.Value);
        }

        if (queryParams.VerificationStatus.HasValue)
        {
            query = query.Where(e => e.User.VerificationStatus == queryParams.VerificationStatus.Value);
        }

        if (queryParams.IsAvailable.HasValue)
        {
            query = query.Where(e => e.IsAvailable == queryParams.IsAvailable.Value);
        }

        if (queryParams.MinHourlyRate.HasValue)
        {
            query = query.Where(e => e.HourlyRate >= queryParams.MinHourlyRate.Value);
        }

        if (queryParams.MaxHourlyRate.HasValue)
        {
            query = query.Where(e => e.HourlyRate <= queryParams.MaxHourlyRate.Value);
        }

        if (!string.IsNullOrWhiteSpace(queryParams.Currency))
        {
            query = query.Where(e => e.Currency == queryParams.Currency);
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply sorting
        query = queryParams.SortBy?.ToLower() switch
        {
            "name" => queryParams.SortDescending
                ? query.OrderByDescending(e => e.User.LastName).ThenByDescending(e => e.User.FirstName)
                : query.OrderBy(e => e.User.LastName).ThenBy(e => e.User.FirstName),
            "hourlyrate" => queryParams.SortDescending
                ? query.OrderByDescending(e => e.HourlyRate)
                : query.OrderBy(e => e.HourlyRate),
            "category" => queryParams.SortDescending
                ? query.OrderByDescending(e => e.Category)
                : query.OrderBy(e => e.Category),
            _ => queryParams.SortDescending
                ? query.OrderByDescending(e => e.CreatedAt)
                : query.OrderBy(e => e.CreatedAt)
        };

        // Apply pagination
        var items = await query
            .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .ToListAsync(cancellationToken);

        return PagedResult<Expert>.Create(items, totalCount, queryParams.PageNumber, queryParams.PageSize);
    }

    public async Task<Expert> CreateAsync(Expert expert, CancellationToken cancellationToken = default)
    {
        _context.Experts.Add(expert);
        await _context.SaveChangesAsync(cancellationToken);
        return expert;
    }

    public async Task UpdateAsync(Expert expert, CancellationToken cancellationToken = default)
    {
        _context.Experts.Update(expert);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var expert = await _context.Experts.FindAsync(new object[] { id }, cancellationToken);
        if (expert == null || expert.IsDeleted)
        {
            return false;
        }

        expert.IsDeleted = true;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    // Credentials
    public async Task<Credential> AddCredentialAsync(Credential credential, CancellationToken cancellationToken = default)
    {
        _context.Credentials.Add(credential);
        await _context.SaveChangesAsync(cancellationToken);
        return credential;
    }

    public async Task<bool> RemoveCredentialAsync(Guid credentialId, CancellationToken cancellationToken = default)
    {
        var credential = await _context.Credentials.FindAsync(new object[] { credentialId }, cancellationToken);
        if (credential == null || credential.IsDeleted)
        {
            return false;
        }

        credential.IsDeleted = true;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    // Availability
    public async Task<ExpertAvailability> AddAvailabilityAsync(ExpertAvailability availability, CancellationToken cancellationToken = default)
    {
        _context.ExpertAvailabilities.Add(availability);
        await _context.SaveChangesAsync(cancellationToken);
        return availability;
    }

    public async Task<bool> RemoveAvailabilityAsync(Guid availabilityId, CancellationToken cancellationToken = default)
    {
        var availability = await _context.ExpertAvailabilities.FindAsync(new object[] { availabilityId }, cancellationToken);
        if (availability == null)
        {
            return false;
        }

        _context.ExpertAvailabilities.Remove(availability);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task ClearAvailabilitiesAsync(Guid expertId, CancellationToken cancellationToken = default)
    {
        var availabilities = await _context.ExpertAvailabilities
            .Where(a => a.ExpertId == expertId)
            .ToListAsync(cancellationToken);

        _context.ExpertAvailabilities.RemoveRange(availabilities);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

using Microsoft.EntityFrameworkCore;
using XpertConnect.Application.Common.Interfaces;
using XpertConnect.Application.Common.Models;
using XpertConnect.Application.Features.Auctions.DTOs;
using XpertConnect.Domain.Entities;
using XpertConnect.Domain.Enums;
using XpertConnect.Infrastructure.Data;

namespace XpertConnect.Infrastructure.Repositories;

public class AuctionRepository : IAuctionRepository
{
    private readonly ApplicationDbContext _context;

    public AuctionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AuctionLot?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.AuctionLots
            .Include(a => a.Expert)
                .ThenInclude(e => e.User)
            .Include(a => a.BeneficiaryOrg)
            .Include(a => a.Bids.Where(b => !b.IsDeleted))
            .Include(a => a.WinningBid)
                .ThenInclude(b => b!.Seeker)
                    .ThenInclude(s => s.User)
            .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted, cancellationToken);
    }

    public async Task<PagedResult<AuctionLot>> GetAllAsync(AuctionQueryParams queryParams, CancellationToken cancellationToken = default)
    {
        var query = _context.AuctionLots
            .Include(a => a.Expert)
                .ThenInclude(e => e.User)
            .Include(a => a.Bids.Where(b => !b.IsDeleted))
            .Where(a => !a.IsDeleted)
            .AsQueryable();

        query = ApplyFilters(query, queryParams);
        return await ExecutePagedQueryAsync(query, queryParams, cancellationToken);
    }

    public async Task<PagedResult<AuctionLot>> GetByExpertIdAsync(Guid expertId, AuctionQueryParams queryParams, CancellationToken cancellationToken = default)
    {
        var query = _context.AuctionLots
            .Include(a => a.Expert)
                .ThenInclude(e => e.User)
            .Include(a => a.Bids.Where(b => !b.IsDeleted))
            .Where(a => a.ExpertId == expertId && !a.IsDeleted)
            .AsQueryable();

        query = ApplyFilters(query, queryParams);
        return await ExecutePagedQueryAsync(query, queryParams, cancellationToken);
    }

    public async Task<AuctionLot> CreateAsync(AuctionLot auction, CancellationToken cancellationToken = default)
    {
        _context.AuctionLots.Add(auction);
        await _context.SaveChangesAsync(cancellationToken);
        return auction;
    }

    public async Task UpdateAsync(AuctionLot auction, CancellationToken cancellationToken = default)
    {
        _context.AuctionLots.Update(auction);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> UpdateStatusAsync(Guid id, AuctionStatus status, CancellationToken cancellationToken = default)
    {
        var auction = await _context.AuctionLots.FindAsync(new object[] { id }, cancellationToken);
        if (auction == null || auction.IsDeleted)
        {
            return false;
        }

        auction.Status = status;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IList<AuctionLot>> GetAuctionsToOpenAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _context.AuctionLots
            .Where(a => !a.IsDeleted
                && a.Status == AuctionStatus.Scheduled
                && a.StartTime <= now)
            .ToListAsync(cancellationToken);
    }

    public async Task<IList<AuctionLot>> GetAuctionsToCloseAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _context.AuctionLots
            .Where(a => !a.IsDeleted
                && a.Status == AuctionStatus.Open
                && a.EndTime <= now)
            .ToListAsync(cancellationToken);
    }

    private static IQueryable<AuctionLot> ApplyFilters(IQueryable<AuctionLot> query, AuctionQueryParams queryParams)
    {
        if (!string.IsNullOrWhiteSpace(queryParams.SearchTerm))
        {
            var searchTerm = queryParams.SearchTerm.ToLower();
            query = query.Where(a =>
                a.Title.ToLower().Contains(searchTerm) ||
                (a.Description != null && a.Description.ToLower().Contains(searchTerm)) ||
                a.Expert.User.FirstName.ToLower().Contains(searchTerm) ||
                a.Expert.User.LastName.ToLower().Contains(searchTerm));
        }

        if (queryParams.Status.HasValue)
        {
            query = query.Where(a => a.Status == queryParams.Status.Value);
        }

        if (queryParams.MeetingType.HasValue)
        {
            query = query.Where(a => a.MeetingType == queryParams.MeetingType.Value);
        }

        if (queryParams.ExpertId.HasValue)
        {
            query = query.Where(a => a.ExpertId == queryParams.ExpertId.Value);
        }

        if (queryParams.ActiveOnly == true)
        {
            var now = DateTime.UtcNow;
            query = query.Where(a => a.Status == AuctionStatus.Open && a.StartTime <= now && a.EndTime > now);
        }

        if (queryParams.MinBid.HasValue)
        {
            query = query.Where(a => a.CurrentHighBid >= queryParams.MinBid.Value || a.StartingBid >= queryParams.MinBid.Value);
        }

        if (queryParams.MaxBid.HasValue)
        {
            query = query.Where(a => a.StartingBid <= queryParams.MaxBid.Value);
        }

        return query;
    }

    private static async Task<PagedResult<AuctionLot>> ExecutePagedQueryAsync(
        IQueryable<AuctionLot> query,
        AuctionQueryParams queryParams,
        CancellationToken cancellationToken)
    {
        var totalCount = await query.CountAsync(cancellationToken);

        query = queryParams.SortBy?.ToLower() switch
        {
            "starttime" => queryParams.SortDescending
                ? query.OrderByDescending(a => a.StartTime)
                : query.OrderBy(a => a.StartTime),
            "currenthighbid" => queryParams.SortDescending
                ? query.OrderByDescending(a => a.CurrentHighBid ?? a.StartingBid)
                : query.OrderBy(a => a.CurrentHighBid ?? a.StartingBid),
            "startingbid" => queryParams.SortDescending
                ? query.OrderByDescending(a => a.StartingBid)
                : query.OrderBy(a => a.StartingBid),
            "createdat" => queryParams.SortDescending
                ? query.OrderByDescending(a => a.CreatedAt)
                : query.OrderBy(a => a.CreatedAt),
            _ => queryParams.SortDescending
                ? query.OrderByDescending(a => a.EndTime)
                : query.OrderBy(a => a.EndTime)
        };

        var items = await query
            .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .ToListAsync(cancellationToken);

        return PagedResult<AuctionLot>.Create(items, totalCount, queryParams.PageNumber, queryParams.PageSize);
    }
}

using Microsoft.EntityFrameworkCore;
using XpertConnect.Application.Common.Interfaces;
using XpertConnect.Application.Common.Models;
using XpertConnect.Domain.Entities;
using XpertConnect.Infrastructure.Data;

namespace XpertConnect.Infrastructure.Repositories;

public class BidRepository : IBidRepository
{
    private readonly ApplicationDbContext _context;

    public BidRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Bid?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Bids
            .Include(b => b.Seeker)
                .ThenInclude(s => s.User)
            .Include(b => b.Auction)
            .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted, cancellationToken);
    }

    public async Task<Bid?> GetHighestBidAsync(Guid auctionId, CancellationToken cancellationToken = default)
    {
        return await _context.Bids
            .Include(b => b.Seeker)
                .ThenInclude(s => s.User)
            .Where(b => b.AuctionId == auctionId && !b.IsDeleted)
            .OrderByDescending(b => b.Amount)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PagedResult<Bid>> GetByAuctionIdAsync(Guid auctionId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.Bids
            .Include(b => b.Seeker)
                .ThenInclude(s => s.User)
            .Where(b => b.AuctionId == auctionId && !b.IsDeleted)
            .OrderByDescending(b => b.Amount);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PagedResult<Bid>.Create(items, totalCount, pageNumber, pageSize);
    }

    public async Task<PagedResult<Bid>> GetBySeekerIdAsync(Guid seekerId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.Bids
            .Include(b => b.Auction)
                .ThenInclude(a => a.Expert)
                    .ThenInclude(e => e.User)
            .Where(b => b.SeekerId == seekerId && !b.IsDeleted)
            .OrderByDescending(b => b.PlacedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PagedResult<Bid>.Create(items, totalCount, pageNumber, pageSize);
    }

    public async Task<Bid> CreateAsync(Bid bid, CancellationToken cancellationToken = default)
    {
        _context.Bids.Add(bid);
        await _context.SaveChangesAsync(cancellationToken);
        return bid;
    }

    public async Task UpdateAsync(Bid bid, CancellationToken cancellationToken = default)
    {
        _context.Bids.Update(bid);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> GetBidCountAsync(Guid auctionId, CancellationToken cancellationToken = default)
    {
        return await _context.Bids
            .CountAsync(b => b.AuctionId == auctionId && !b.IsDeleted, cancellationToken);
    }

    public async Task ClearWinningFlagsAsync(Guid auctionId, CancellationToken cancellationToken = default)
    {
        var bids = await _context.Bids
            .Where(b => b.AuctionId == auctionId && b.IsWinning && !b.IsDeleted)
            .ToListAsync(cancellationToken);

        foreach (var bid in bids)
        {
            bid.IsWinning = false;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}

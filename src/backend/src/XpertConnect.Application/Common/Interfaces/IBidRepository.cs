using XpertConnect.Application.Common.Models;
using XpertConnect.Domain.Entities;

namespace XpertConnect.Application.Common.Interfaces;

public interface IBidRepository
{
    Task<Bid?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Bid?> GetHighestBidAsync(Guid auctionId, CancellationToken cancellationToken = default);
    Task<PagedResult<Bid>> GetByAuctionIdAsync(Guid auctionId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<PagedResult<Bid>> GetBySeekerIdAsync(Guid seekerId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<Bid> CreateAsync(Bid bid, CancellationToken cancellationToken = default);
    Task UpdateAsync(Bid bid, CancellationToken cancellationToken = default);
    Task<int> GetBidCountAsync(Guid auctionId, CancellationToken cancellationToken = default);
    Task ClearWinningFlagsAsync(Guid auctionId, CancellationToken cancellationToken = default);
}

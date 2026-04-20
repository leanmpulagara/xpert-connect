using XpertConnect.Application.Common.Models;
using XpertConnect.Application.Features.Auctions.DTOs;
using XpertConnect.Domain.Entities;
using XpertConnect.Domain.Enums;

namespace XpertConnect.Application.Common.Interfaces;

public interface IAuctionRepository
{
    Task<AuctionLot?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<AuctionLot>> GetAllAsync(AuctionQueryParams queryParams, CancellationToken cancellationToken = default);
    Task<PagedResult<AuctionLot>> GetByExpertIdAsync(Guid expertId, AuctionQueryParams queryParams, CancellationToken cancellationToken = default);
    Task<AuctionLot> CreateAsync(AuctionLot auction, CancellationToken cancellationToken = default);
    Task UpdateAsync(AuctionLot auction, CancellationToken cancellationToken = default);
    Task<bool> UpdateStatusAsync(Guid id, AuctionStatus status, CancellationToken cancellationToken = default);
    Task<IList<AuctionLot>> GetAuctionsToOpenAsync(CancellationToken cancellationToken = default);
    Task<IList<AuctionLot>> GetAuctionsToCloseAsync(CancellationToken cancellationToken = default);
}

using XpertConnect.Domain.Entities;
using XpertConnect.Domain.Enums;

namespace XpertConnect.Application.Common.Interfaces;

public interface IEscrowRepository
{
    Task<EscrowAccount?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<EscrowAccount?> GetByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken = default);
    Task<EscrowAccount> CreateAsync(EscrowAccount escrow, CancellationToken cancellationToken = default);
    Task UpdateAsync(EscrowAccount escrow, CancellationToken cancellationToken = default);
    Task<bool> UpdateStatusAsync(Guid id, PaymentStatus status, CancellationToken cancellationToken = default);
    Task<Milestone?> GetMilestoneByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Milestone> AddMilestoneAsync(Milestone milestone, CancellationToken cancellationToken = default);
    Task<bool> ApproveMilestoneAsync(Guid milestoneId, CancellationToken cancellationToken = default);
}

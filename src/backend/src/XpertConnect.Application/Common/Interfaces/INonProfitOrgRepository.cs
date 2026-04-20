using XpertConnect.Domain.Entities;

namespace XpertConnect.Application.Common.Interfaces;

public interface INonProfitOrgRepository
{
    Task<NonProfitOrg?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<NonProfitOrg?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}

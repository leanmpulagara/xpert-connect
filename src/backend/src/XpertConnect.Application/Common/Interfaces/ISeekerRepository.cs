using XpertConnect.Domain.Entities;

namespace XpertConnect.Application.Common.Interfaces;

public interface ISeekerRepository
{
    Task<Seeker?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Seeker?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Seeker> CreateAsync(Seeker seeker, CancellationToken cancellationToken = default);
    Task UpdateAsync(Seeker seeker, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

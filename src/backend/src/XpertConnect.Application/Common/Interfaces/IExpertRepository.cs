using XpertConnect.Application.Common.Models;
using XpertConnect.Application.Features.Experts.DTOs;
using XpertConnect.Domain.Entities;

namespace XpertConnect.Application.Common.Interfaces;

public interface IExpertRepository
{
    Task<Expert?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Expert?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<PagedResult<Expert>> GetAllAsync(ExpertQueryParams queryParams, CancellationToken cancellationToken = default);
    Task<Expert> CreateAsync(Expert expert, CancellationToken cancellationToken = default);
    Task UpdateAsync(Expert expert, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    // Credentials
    Task<Credential> AddCredentialAsync(Credential credential, CancellationToken cancellationToken = default);
    Task<bool> RemoveCredentialAsync(Guid credentialId, CancellationToken cancellationToken = default);

    // Availability
    Task<ExpertAvailability> AddAvailabilityAsync(ExpertAvailability availability, CancellationToken cancellationToken = default);
    Task<bool> RemoveAvailabilityAsync(Guid availabilityId, CancellationToken cancellationToken = default);
    Task ClearAvailabilitiesAsync(Guid expertId, CancellationToken cancellationToken = default);
}

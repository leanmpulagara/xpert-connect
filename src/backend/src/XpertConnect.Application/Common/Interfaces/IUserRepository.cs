using XpertConnect.Application.Common.Models;
using XpertConnect.Application.Features.Users.DTOs;
using XpertConnect.Domain.Entities;

namespace XpertConnect.Application.Common.Interfaces;

public interface IUserRepository
{
    Task<PagedResult<User>> GetAllAsync(UserQueryParams queryParams, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User> CreateAsync(User user, CancellationToken cancellationToken = default);
    Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, Guid? excludeUserId = null, CancellationToken cancellationToken = default);
}

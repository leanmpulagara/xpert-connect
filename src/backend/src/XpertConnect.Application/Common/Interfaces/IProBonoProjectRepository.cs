using XpertConnect.Application.Common.Models;
using XpertConnect.Application.Features.Projects.DTOs;
using XpertConnect.Domain.Entities;
using XpertConnect.Domain.Enums;

namespace XpertConnect.Application.Common.Interfaces;

public interface IProBonoProjectRepository
{
    Task<ProBonoProject?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<ProBonoProject>> GetAllAsync(ProjectQueryParams queryParams, CancellationToken cancellationToken = default);
    Task<PagedResult<ProBonoProject>> GetByOrgIdAsync(Guid orgId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<PagedResult<ProBonoProject>> GetByExpertIdAsync(Guid expertId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IList<ProBonoProject>> GetOpenProjectsAsync(CancellationToken cancellationToken = default);
    Task<ProBonoProject> CreateAsync(ProBonoProject project, CancellationToken cancellationToken = default);
    Task UpdateAsync(ProBonoProject project, CancellationToken cancellationToken = default);
    Task<bool> UpdateStatusAsync(Guid id, ProjectStatus status, CancellationToken cancellationToken = default);
    Task<bool> AssignExpertAsync(Guid projectId, Guid expertId, CancellationToken cancellationToken = default);
    Task<bool> RemoveExpertAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalHoursAsync(Guid projectId, CancellationToken cancellationToken = default);
}

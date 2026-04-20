using XpertConnect.Application.Common.Models;
using XpertConnect.Application.Features.TimeEntries.DTOs;
using XpertConnect.Domain.Entities;

namespace XpertConnect.Application.Common.Interfaces;

public interface ITimeEntryRepository
{
    Task<TimeEntry?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<TimeEntry>> GetByProjectIdAsync(Guid projectId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<PagedResult<TimeEntry>> GetByExpertIdAsync(Guid expertId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<TimeEntry> CreateAsync(TimeEntry timeEntry, CancellationToken cancellationToken = default);
    Task UpdateAsync(TimeEntry timeEntry, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalHoursByProjectAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalHoursByExpertAsync(Guid expertId, CancellationToken cancellationToken = default);
    Task<ExpertHoursSummary> GetExpertHoursSummaryAsync(Guid expertId, CancellationToken cancellationToken = default);
}

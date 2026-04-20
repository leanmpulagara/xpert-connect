using XpertConnect.Application.Common.Models;
using XpertConnect.Domain.Entities;

namespace XpertConnect.Application.Common.Interfaces;

public interface IFeedbackRepository
{
    Task<Feedback?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Feedback?> GetByConsultationIdAsync(Guid consultationId, CancellationToken cancellationToken = default);
    Task<PagedResult<Feedback>> GetByExpertIdAsync(Guid expertId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<Feedback> CreateAsync(Feedback feedback, CancellationToken cancellationToken = default);
    Task<(double AverageRating, int TotalCount)> GetExpertRatingSummaryAsync(Guid expertId, CancellationToken cancellationToken = default);
    Task<Dictionary<int, int>> GetExpertRatingDistributionAsync(Guid expertId, CancellationToken cancellationToken = default);
}

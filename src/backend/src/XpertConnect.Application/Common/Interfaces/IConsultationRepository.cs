using XpertConnect.Application.Common.Models;
using XpertConnect.Application.Features.Consultations.DTOs;
using XpertConnect.Domain.Entities;
using XpertConnect.Domain.Enums;

namespace XpertConnect.Application.Common.Interfaces;

public interface IConsultationRepository
{
    Task<Consultation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<Consultation>> GetByExpertIdAsync(Guid expertId, ConsultationQueryParams queryParams, CancellationToken cancellationToken = default);
    Task<PagedResult<Consultation>> GetBySeekerIdAsync(Guid seekerId, ConsultationQueryParams queryParams, CancellationToken cancellationToken = default);
    Task<Consultation> CreateAsync(Consultation consultation, CancellationToken cancellationToken = default);
    Task UpdateAsync(Consultation consultation, CancellationToken cancellationToken = default);
    Task<bool> UpdateStatusAsync(Guid id, BookingStatus status, CancellationToken cancellationToken = default);
    Task<bool> HasOverlappingBookingAsync(Guid expertId, DateTime scheduledAt, int durationMinutes, Guid? excludeConsultationId = null, CancellationToken cancellationToken = default);
}

using XpertConnect.Application.Common.Models;
using XpertConnect.Application.Features.Payments.DTOs;
using XpertConnect.Domain.Entities;
using XpertConnect.Domain.Enums;

namespace XpertConnect.Application.Common.Interfaces;

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Payment?> GetByConsultationIdAsync(Guid consultationId, CancellationToken cancellationToken = default);
    Task<PagedResult<Payment>> GetAllAsync(PaymentQueryParams queryParams, CancellationToken cancellationToken = default);
    Task<PagedResult<Payment>> GetByUserIdAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Payment> CreateAsync(Payment payment, CancellationToken cancellationToken = default);
    Task UpdateAsync(Payment payment, CancellationToken cancellationToken = default);
    Task<bool> UpdateStatusAsync(Guid id, PaymentStatus status, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalAmountByUserAsync(Guid userId, PaymentStatus? status = null, CancellationToken cancellationToken = default);
}

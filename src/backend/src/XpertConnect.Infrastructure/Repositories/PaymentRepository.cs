using Microsoft.EntityFrameworkCore;
using XpertConnect.Application.Common.Interfaces;
using XpertConnect.Application.Common.Models;
using XpertConnect.Application.Features.Payments.DTOs;
using XpertConnect.Domain.Entities;
using XpertConnect.Domain.Enums;
using XpertConnect.Infrastructure.Data;

namespace XpertConnect.Infrastructure.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly ApplicationDbContext _context;

    public PaymentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .Include(p => p.Consultation)
                .ThenInclude(c => c.Expert)
                    .ThenInclude(e => e.User)
            .Include(p => p.Consultation)
                .ThenInclude(c => c.Seeker)
                    .ThenInclude(s => s.User)
            .Include(p => p.EscrowAccount)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Payment?> GetByConsultationIdAsync(Guid consultationId, CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .Include(p => p.EscrowAccount)
            .FirstOrDefaultAsync(p => p.ConsultationId == consultationId, cancellationToken);
    }

    public async Task<PagedResult<Payment>> GetAllAsync(PaymentQueryParams queryParams, CancellationToken cancellationToken = default)
    {
        var query = _context.Payments
            .Include(p => p.Consultation)
            .AsQueryable();

        // Apply filters
        if (queryParams.Status.HasValue)
        {
            query = query.Where(p => p.Status == queryParams.Status.Value);
        }

        if (queryParams.ConsultationId.HasValue)
        {
            query = query.Where(p => p.ConsultationId == queryParams.ConsultationId.Value);
        }

        if (queryParams.FromDate.HasValue)
        {
            query = query.Where(p => p.CreatedAt >= queryParams.FromDate.Value);
        }

        if (queryParams.ToDate.HasValue)
        {
            query = query.Where(p => p.CreatedAt <= queryParams.ToDate.Value);
        }

        // Apply sorting
        query = queryParams.SortBy?.ToLower() switch
        {
            "amount" => queryParams.SortDescending
                ? query.OrderByDescending(p => p.Amount)
                : query.OrderBy(p => p.Amount),
            "status" => queryParams.SortDescending
                ? query.OrderByDescending(p => p.Status)
                : query.OrderBy(p => p.Status),
            _ => queryParams.SortDescending
                ? query.OrderByDescending(p => p.CreatedAt)
                : query.OrderBy(p => p.CreatedAt)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((queryParams.Page - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .ToListAsync(cancellationToken);

        return PagedResult<Payment>.Create(items, totalCount, queryParams.Page, queryParams.PageSize);
    }

    public async Task<PagedResult<Payment>> GetByUserIdAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.Payments
            .Include(p => p.Consultation)
            .Where(p => p.Consultation.Seeker.UserId == userId || p.Consultation.Expert.UserId == userId)
            .OrderByDescending(p => p.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PagedResult<Payment>.Create(items, totalCount, page, pageSize);
    }

    public async Task<Payment> CreateAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        payment.CreatedAt = DateTime.UtcNow;
        _context.Payments.Add(payment);
        await _context.SaveChangesAsync(cancellationToken);
        return payment;
    }

    public async Task UpdateAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        payment.UpdatedAt = DateTime.UtcNow;
        _context.Payments.Update(payment);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> UpdateStatusAsync(Guid id, PaymentStatus status, CancellationToken cancellationToken = default)
    {
        var payment = await _context.Payments.FindAsync(new object[] { id }, cancellationToken);
        if (payment == null) return false;

        payment.Status = status;
        payment.UpdatedAt = DateTime.UtcNow;

        if (status == PaymentStatus.Authorized)
        {
            payment.AuthorizedAt = DateTime.UtcNow;
        }
        else if (status == PaymentStatus.Released)
        {
            payment.CapturedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<decimal> GetTotalAmountByUserAsync(Guid userId, PaymentStatus? status = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Payments
            .Where(p => p.Consultation.Seeker.UserId == userId || p.Consultation.Expert.UserId == userId);

        if (status.HasValue)
        {
            query = query.Where(p => p.Status == status.Value);
        }

        return await query.SumAsync(p => p.Amount, cancellationToken);
    }
}

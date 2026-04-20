using Microsoft.EntityFrameworkCore;
using XpertConnect.Application.Common.Interfaces;
using XpertConnect.Domain.Entities;
using XpertConnect.Domain.Enums;
using XpertConnect.Infrastructure.Data;

namespace XpertConnect.Infrastructure.Repositories;

public class EscrowRepository : IEscrowRepository
{
    private readonly ApplicationDbContext _context;

    public EscrowRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<EscrowAccount?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.EscrowAccounts
            .Include(e => e.Payment)
                .ThenInclude(p => p.Consultation)
            .Include(e => e.Milestones)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<EscrowAccount?> GetByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken = default)
    {
        return await _context.EscrowAccounts
            .Include(e => e.Milestones)
            .FirstOrDefaultAsync(e => e.PaymentId == paymentId, cancellationToken);
    }

    public async Task<EscrowAccount> CreateAsync(EscrowAccount escrow, CancellationToken cancellationToken = default)
    {
        escrow.CreatedAt = DateTime.UtcNow;
        _context.EscrowAccounts.Add(escrow);
        await _context.SaveChangesAsync(cancellationToken);
        return escrow;
    }

    public async Task UpdateAsync(EscrowAccount escrow, CancellationToken cancellationToken = default)
    {
        escrow.UpdatedAt = DateTime.UtcNow;
        _context.EscrowAccounts.Update(escrow);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> UpdateStatusAsync(Guid id, PaymentStatus status, CancellationToken cancellationToken = default)
    {
        var escrow = await _context.EscrowAccounts.FindAsync(new object[] { id }, cancellationToken);
        if (escrow == null) return false;

        escrow.Status = status;
        escrow.UpdatedAt = DateTime.UtcNow;

        if (status == PaymentStatus.InEscrow)
        {
            escrow.FundedAt = DateTime.UtcNow;
        }
        else if (status == PaymentStatus.Released)
        {
            escrow.ReleasedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<Milestone?> GetMilestoneByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Milestones
            .Include(m => m.Escrow)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<Milestone> AddMilestoneAsync(Milestone milestone, CancellationToken cancellationToken = default)
    {
        _context.Milestones.Add(milestone);
        await _context.SaveChangesAsync(cancellationToken);
        return milestone;
    }

    public async Task<bool> ApproveMilestoneAsync(Guid milestoneId, CancellationToken cancellationToken = default)
    {
        var milestone = await _context.Milestones.FindAsync(new object[] { milestoneId }, cancellationToken);
        if (milestone == null) return false;

        milestone.IsApproved = true;
        milestone.CompletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

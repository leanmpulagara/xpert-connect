using Microsoft.EntityFrameworkCore;
using XpertConnect.Application.Common.Interfaces;
using XpertConnect.Domain.Entities;
using XpertConnect.Infrastructure.Data;

namespace XpertConnect.Infrastructure.Repositories;

public class NonProfitOrgRepository : INonProfitOrgRepository
{
    private readonly ApplicationDbContext _context;

    public NonProfitOrgRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<NonProfitOrg?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.NonProfitOrgs
            .Include(n => n.User)
            .FirstOrDefaultAsync(n => n.Id == id, cancellationToken);
    }

    public async Task<NonProfitOrg?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.NonProfitOrgs
            .Include(n => n.User)
            .FirstOrDefaultAsync(n => n.UserId == userId, cancellationToken);
    }
}

using Microsoft.EntityFrameworkCore;
using XpertConnect.Application.Common.Interfaces;
using XpertConnect.Domain.Entities;
using XpertConnect.Infrastructure.Data;

namespace XpertConnect.Infrastructure.Repositories;

public class SeekerRepository : ISeekerRepository
{
    private readonly ApplicationDbContext _context;

    public SeekerRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Seeker?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Seekers
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted, cancellationToken);
    }

    public async Task<Seeker?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Seekers
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.UserId == userId && !s.IsDeleted, cancellationToken);
    }

    public async Task<Seeker> CreateAsync(Seeker seeker, CancellationToken cancellationToken = default)
    {
        _context.Seekers.Add(seeker);
        await _context.SaveChangesAsync(cancellationToken);
        return seeker;
    }

    public async Task UpdateAsync(Seeker seeker, CancellationToken cancellationToken = default)
    {
        _context.Seekers.Update(seeker);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var seeker = await _context.Seekers.FindAsync(new object[] { id }, cancellationToken);
        if (seeker == null || seeker.IsDeleted)
        {
            return false;
        }

        seeker.IsDeleted = true;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

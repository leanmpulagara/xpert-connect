using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using XpertConnect.Application.Common.Interfaces;
using XpertConnect.Domain.Entities;
using XpertConnect.Infrastructure.Identity;

namespace XpertConnect.Infrastructure.Data;

/// <summary>
/// Application database context with Identity support
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // Identity
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    // User & Auth (DomainUsers to avoid conflict with Identity Users)
    public DbSet<User> DomainUsers => Set<User>();
    public DbSet<Expert> Experts => Set<Expert>();
    public DbSet<Seeker> Seekers => Set<Seeker>();
    public DbSet<NonProfitOrg> NonProfitOrgs => Set<NonProfitOrg>();

    // Verification & Compliance
    public DbSet<KycVerification> KycVerifications => Set<KycVerification>();
    public DbSet<FinancialReference> FinancialReferences => Set<FinancialReference>();
    public DbSet<ComplianceCheck> ComplianceChecks => Set<ComplianceCheck>();
    public DbSet<Credential> Credentials => Set<Credential>();

    // Auction Model
    public DbSet<AuctionLot> AuctionLots => Set<AuctionLot>();
    public DbSet<Bid> Bids => Set<Bid>();

    // Professional Fee Model
    public DbSet<Consultation> Consultations => Set<Consultation>();
    public DbSet<ExpertAvailability> ExpertAvailabilities => Set<ExpertAvailability>();
    public DbSet<Feedback> Feedbacks => Set<Feedback>();

    // Pro-Bono Model
    public DbSet<ProBonoProject> ProBonoProjects => Set<ProBonoProject>();
    public DbSet<TimeEntry> TimeEntries => Set<TimeEntry>();

    // Payments & Escrow
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<EscrowAccount> EscrowAccounts => Set<EscrowAccount>();
    public DbSet<Milestone> Milestones => Set<Milestone>();

    // Legal Documents
    public DbSet<Nda> Ndas => Set<Nda>();
    public DbSet<Mou> Mous => Set<Mou>();

    // Physical Meetings & Geofencing
    public DbSet<Venue> Venues => Set<Venue>();
    public DbSet<PhysicalMeeting> PhysicalMeetings => Set<PhysicalMeeting>();
    public DbSet<Geofence> Geofences => Set<Geofence>();
    public DbSet<GeofenceEvent> GeofenceEvents => Set<GeofenceEvent>();
    public DbSet<Guest> Guests => Set<Guest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply Identity configuration first
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Configure ApplicationUser
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.HasOne(u => u.DomainUser)
                .WithOne()
                .HasForeignKey<ApplicationUser>(u => u.DomainUserId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasMany(u => u.RefreshTokens)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure RefreshToken
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Token).HasMaxLength(500).IsRequired();
            entity.HasIndex(r => r.Token).IsUnique();
        });
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Auto-update timestamps
        foreach (var entry in ChangeTracker.Entries<Domain.Entities.Common.BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}

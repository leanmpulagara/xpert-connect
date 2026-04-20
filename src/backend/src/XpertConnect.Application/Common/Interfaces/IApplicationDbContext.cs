using Microsoft.EntityFrameworkCore;
using XpertConnect.Domain.Entities;

namespace XpertConnect.Application.Common.Interfaces;

/// <summary>
/// Database context interface - implemented by Infrastructure layer
/// </summary>
public interface IApplicationDbContext
{
    DbSet<User> DomainUsers { get; }
    DbSet<Expert> Experts { get; }
    DbSet<Seeker> Seekers { get; }
    DbSet<NonProfitOrg> NonProfitOrgs { get; }
    DbSet<AuctionLot> AuctionLots { get; }
    DbSet<Bid> Bids { get; }
    DbSet<Consultation> Consultations { get; }
    DbSet<ProBonoProject> ProBonoProjects { get; }
    DbSet<Payment> Payments { get; }
    DbSet<EscrowAccount> EscrowAccounts { get; }
    DbSet<Milestone> Milestones { get; }
    DbSet<Nda> Ndas { get; }
    DbSet<Mou> Mous { get; }
    DbSet<Credential> Credentials { get; }
    DbSet<KycVerification> KycVerifications { get; }
    DbSet<FinancialReference> FinancialReferences { get; }
    DbSet<ComplianceCheck> ComplianceChecks { get; }
    DbSet<Feedback> Feedbacks { get; }
    DbSet<TimeEntry> TimeEntries { get; }
    DbSet<ExpertAvailability> ExpertAvailabilities { get; }
    DbSet<Venue> Venues { get; }
    DbSet<PhysicalMeeting> PhysicalMeetings { get; }
    DbSet<Geofence> Geofences { get; }
    DbSet<GeofenceEvent> GeofenceEvents { get; }
    DbSet<Guest> Guests { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

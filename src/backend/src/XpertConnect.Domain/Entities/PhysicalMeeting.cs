using XpertConnect.Domain.Entities.Common;
using XpertConnect.Domain.Enums;

namespace XpertConnect.Domain.Entities;

/// <summary>
/// Physical meeting details for auctions and in-person consultations
/// </summary>
public class PhysicalMeeting : AuditableEntity
{
    public Guid? AuctionId { get; set; }
    public Guid? ConsultationId { get; set; }
    public Guid VenueId { get; set; }
    public Guid GeofenceId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public bool IsVerified { get; set; } = false;
    public DateTime? VerifiedAt { get; set; }

    // Navigation properties
    public virtual AuctionLot? Auction { get; set; }
    public virtual Consultation? Consultation { get; set; }
    public virtual Venue Venue { get; set; } = null!;
    public virtual Geofence Geofence { get; set; } = null!;
    public virtual ICollection<Guest> Guests { get; set; } = new List<Guest>();
}

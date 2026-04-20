using XpertConnect.Domain.Entities.Common;
using XpertConnect.Domain.Enums;

namespace XpertConnect.Domain.Entities;

/// <summary>
/// Expert profile - the "Accomplished Mind"
/// </summary>
public class Expert : AuditableEntity
{
    public Guid UserId { get; set; }
    public ExpertCategory Category { get; set; }
    public string? Headline { get; set; }
    public string? Bio { get; set; }
    public decimal HourlyRate { get; set; }
    public string? Currency { get; set; } = "USD";
    public bool IsAvailable { get; set; } = true;
    public string? LinkedInUrl { get; set; }
    public string? SecurityLevel { get; set; }
    public bool RequiresExecutiveProtection { get; set; } = false;

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual ICollection<Credential> Credentials { get; set; } = new List<Credential>();
    public virtual ICollection<AuctionLot> AuctionLots { get; set; } = new List<AuctionLot>();
    public virtual ICollection<Consultation> Consultations { get; set; } = new List<Consultation>();
    public virtual ICollection<ProBonoProject> ProBonoProjects { get; set; } = new List<ProBonoProject>();
    public virtual ICollection<ExpertAvailability> Availabilities { get; set; } = new List<ExpertAvailability>();
}

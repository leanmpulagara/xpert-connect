using XpertConnect.Domain.Entities.Common;
using XpertConnect.Domain.Enums;

namespace XpertConnect.Domain.Entities;

/// <summary>
/// Non-profit organization for pro-bono projects and auction beneficiaries
/// </summary>
public class NonProfitOrg : AuditableEntity
{
    public Guid UserId { get; set; }
    public string OrgName { get; set; } = string.Empty;
    public string? TaxId { get; set; }
    public string? Mission { get; set; }
    public string? Website { get; set; }
    public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.Pending;
    public DateTime? VerifiedAt { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual ICollection<ProBonoProject> ProBonoProjects { get; set; } = new List<ProBonoProject>();
    public virtual ICollection<AuctionLot> BeneficiaryAuctions { get; set; } = new List<AuctionLot>();
}

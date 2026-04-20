using XpertConnect.Domain.Entities.Common;

namespace XpertConnect.Domain.Entities;

/// <summary>
/// Guest for physical meetings (auction winners can bring guests)
/// </summary>
public class Guest : AuditableEntity
{
    public Guid MeetingId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Relationship { get; set; }
    public bool KycVerified { get; set; } = false;
    public bool NdaSigned { get; set; } = false;

    // Navigation properties
    public virtual PhysicalMeeting Meeting { get; set; } = null!;
}

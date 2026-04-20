using XpertConnect.Domain.Entities.Common;

namespace XpertConnect.Domain.Entities;

/// <summary>
/// Milestone for escrow-based payments
/// </summary>
public class Milestone : BaseEntity
{
    public Guid EscrowId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public bool IsApproved { get; set; } = false;

    // Navigation properties
    public virtual EscrowAccount Escrow { get; set; } = null!;
}

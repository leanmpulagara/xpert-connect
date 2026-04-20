using XpertConnect.Domain.Entities.Common;

namespace XpertConnect.Domain.Entities;

/// <summary>
/// Financial reference for premium seeker verification
/// </summary>
public class FinancialReference : AuditableEntity
{
    public Guid SeekerId { get; set; }
    public string BankName { get; set; } = string.Empty;
    public string? AccountType { get; set; }
    public decimal? ValidatedAmount { get; set; }
    public DateTime? ValidatedAt { get; set; }

    // Navigation properties
    public virtual Seeker Seeker { get; set; } = null!;
}

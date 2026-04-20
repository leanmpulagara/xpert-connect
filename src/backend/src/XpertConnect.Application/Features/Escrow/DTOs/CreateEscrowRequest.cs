namespace XpertConnect.Application.Features.Escrow.DTOs;

/// <summary>
/// Request to create an escrow account
/// </summary>
public class CreateEscrowRequest
{
    public Guid PaymentId { get; set; }
    public List<CreateMilestoneRequest>? Milestones { get; set; }
}

/// <summary>
/// Request to create a milestone
/// </summary>
public class CreateMilestoneRequest
{
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime? DueDate { get; set; }
}

/// <summary>
/// Request to fund an escrow account
/// </summary>
public class FundEscrowRequest
{
    public string? PaymentMethodId { get; set; }
}

/// <summary>
/// Request to release escrow funds
/// </summary>
public class ReleaseEscrowRequest
{
    public Guid? MilestoneId { get; set; }
    public decimal? Amount { get; set; }
}

/// <summary>
/// Request to dispute an escrow
/// </summary>
public class DisputeEscrowRequest
{
    public string Reason { get; set; } = string.Empty;
    public string? Evidence { get; set; }
}

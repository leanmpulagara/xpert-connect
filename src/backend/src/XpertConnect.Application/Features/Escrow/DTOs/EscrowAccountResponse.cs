using XpertConnect.Domain.Enums;

namespace XpertConnect.Application.Features.Escrow.DTOs;

/// <summary>
/// Escrow account details response
/// </summary>
public class EscrowAccountResponse
{
    public Guid Id { get; set; }
    public Guid PaymentId { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; }
    public string StatusName => Status.ToString();
    public string? EscrowProviderRef { get; set; }
    public DateTime? FundedAt { get; set; }
    public DateTime? ReleasedAt { get; set; }
    public List<MilestoneResponse> Milestones { get; set; } = new();
    public decimal TotalReleasedAmount => Milestones.Where(m => m.IsApproved).Sum(m => m.Amount);
    public decimal RemainingAmount => Amount - TotalReleasedAmount;
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Milestone response
/// </summary>
public class MilestoneResponse
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public bool IsApproved { get; set; }
}

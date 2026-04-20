using XpertConnect.Domain.Enums;

namespace XpertConnect.Application.Common.Models;

public class KycResult
{
    public bool IsSuccess { get; set; }
    public string? ProviderRef { get; set; }
    public VerificationStatus Status { get; set; }
    public string? Error { get; set; }
    public DateTime? VerifiedAt { get; set; }
}

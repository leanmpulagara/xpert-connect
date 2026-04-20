using XpertConnect.Domain.Enums;

namespace XpertConnect.Application.Features.Experts.DTOs;

public class CredentialResponse
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string IssuingBody { get; set; } = string.Empty;
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public VerificationStatus VerificationStatus { get; set; }
    public string VerificationStatusName => VerificationStatus.ToString();
    public DateTime? VerifiedAt { get; set; }
}

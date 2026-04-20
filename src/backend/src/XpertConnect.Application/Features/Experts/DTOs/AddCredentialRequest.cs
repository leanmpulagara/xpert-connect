namespace XpertConnect.Application.Features.Experts.DTOs;

/// <summary>
/// Request to add a credential to expert profile
/// </summary>
public class AddCredentialRequest
{
    public string Type { get; set; } = string.Empty;
    public string IssuingBody { get; set; } = string.Empty;
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? VerificationSource { get; set; }
}

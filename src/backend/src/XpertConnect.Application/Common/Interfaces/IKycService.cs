using XpertConnect.Application.Common.Models;

namespace XpertConnect.Application.Common.Interfaces;

/// <summary>
/// KYC verification service interface (Onfido/Jumio integration)
/// </summary>
public interface IKycService
{
    Task<KycResult> InitiateVerificationAsync(Guid userId, string documentType, CancellationToken cancellationToken = default);
    Task<KycResult> CheckVerificationStatusAsync(string providerRef, CancellationToken cancellationToken = default);
    Task<bool> PerformLivenessCheckAsync(Guid userId, byte[] imageData, CancellationToken cancellationToken = default);
}

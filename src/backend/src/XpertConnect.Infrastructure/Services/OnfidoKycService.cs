using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using XpertConnect.Application.Common.Interfaces;
using XpertConnect.Application.Common.Models;
using XpertConnect.Domain.Enums;

namespace XpertConnect.Infrastructure.Services;

/// <summary>
/// Onfido KYC verification service implementation
/// </summary>
public class OnfidoKycService : IKycService
{
    private readonly ILogger<OnfidoKycService> _logger;
    private readonly HttpClient _httpClient;
    private readonly string _apiToken;
    private readonly string _baseUrl;

    public OnfidoKycService(IConfiguration configuration, ILogger<OnfidoKycService> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient("Onfido");

        _apiToken = configuration["Onfido:ApiToken"]
            ?? throw new InvalidOperationException("Onfido:ApiToken not configured");

        // Use sandbox URL for testing, live URL for production
        _baseUrl = configuration["Onfido:BaseUrl"] ?? "https://api.onfido.com/v3.6";

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", $"token={_apiToken}");
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<KycResult> InitiateVerificationAsync(
        Guid userId,
        string documentType,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Step 1: Create an applicant
            var applicantResponse = await CreateApplicantAsync(userId, cancellationToken);

            if (applicantResponse == null)
            {
                return new KycResult
                {
                    IsSuccess = false,
                    Status = VerificationStatus.Failed,
                    Error = "Failed to create applicant"
                };
            }

            // Step 2: Create a check (document + facial similarity)
            var checkResponse = await CreateCheckAsync(applicantResponse.Id, documentType, cancellationToken);

            if (checkResponse == null)
            {
                return new KycResult
                {
                    IsSuccess = false,
                    Status = VerificationStatus.Failed,
                    Error = "Failed to create verification check"
                };
            }

            _logger.LogInformation("KYC verification initiated for user {UserId} with check ID {CheckId}",
                userId, checkResponse.Id);

            return new KycResult
            {
                IsSuccess = true,
                ProviderRef = checkResponse.Id,
                Status = VerificationStatus.Pending
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initiating KYC verification for user {UserId}", userId);
            return new KycResult
            {
                IsSuccess = false,
                Status = VerificationStatus.Failed,
                Error = ex.Message
            };
        }
    }

    public async Task<KycResult> CheckVerificationStatusAsync(
        string providerRef,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/checks/{providerRef}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Failed to check verification status. Status: {StatusCode}, Response: {Response}",
                    response.StatusCode, errorContent);

                return new KycResult
                {
                    IsSuccess = false,
                    Status = VerificationStatus.Failed,
                    Error = $"Failed to check status: {response.StatusCode}"
                };
            }

            var checkResult = await response.Content.ReadFromJsonAsync<OnfidoCheckResponse>(cancellationToken: cancellationToken);

            var status = checkResult?.Status switch
            {
                "complete" when checkResult.Result == "clear" => VerificationStatus.Verified,
                "complete" when checkResult.Result == "consider" => VerificationStatus.ManualReviewRequired,
                "complete" => VerificationStatus.Rejected,
                "in_progress" => VerificationStatus.Pending,
                "awaiting_applicant" => VerificationStatus.Pending,
                _ => VerificationStatus.Failed
            };

            return new KycResult
            {
                IsSuccess = true,
                ProviderRef = providerRef,
                Status = status,
                VerifiedAt = status == VerificationStatus.Verified ? DateTime.UtcNow : null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking KYC verification status for {ProviderRef}", providerRef);
            return new KycResult
            {
                IsSuccess = false,
                Status = VerificationStatus.Failed,
                Error = ex.Message
            };
        }
    }

    public async Task<bool> PerformLivenessCheckAsync(
        Guid userId,
        byte[] imageData,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // This would typically upload a live photo for facial similarity check
            // For now, we'll implement a placeholder that logs the attempt
            _logger.LogInformation("Liveness check requested for user {UserId} with image size {Size} bytes",
                userId, imageData.Length);

            // In a real implementation, you would:
            // 1. Upload the live photo to Onfido
            // 2. Create a facial similarity check
            // 3. Return the result

            return true; // Placeholder
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing liveness check for user {UserId}", userId);
            return false;
        }
    }

    private async Task<OnfidoApplicantResponse?> CreateApplicantAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var applicantData = new
        {
            // In a real implementation, you would fetch user details
            // and populate first_name, last_name, email, etc.
            first_name = "Pending",
            last_name = "Verification",
            email = $"user_{userId}@verification.xpertconnect.com"
        };

        var content = new StringContent(
            JsonSerializer.Serialize(applicantData),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync($"{_baseUrl}/applicants", content, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("Failed to create applicant. Status: {StatusCode}, Response: {Response}",
                response.StatusCode, errorContent);
            return null;
        }

        return await response.Content.ReadFromJsonAsync<OnfidoApplicantResponse>(cancellationToken: cancellationToken);
    }

    private async Task<OnfidoCheckResponse?> CreateCheckAsync(
        string applicantId,
        string documentType,
        CancellationToken cancellationToken)
    {
        var checkData = new
        {
            applicant_id = applicantId,
            report_names = new[] { "document", "facial_similarity_photo" }
        };

        var content = new StringContent(
            JsonSerializer.Serialize(checkData),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync($"{_baseUrl}/checks", content, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("Failed to create check. Status: {StatusCode}, Response: {Response}",
                response.StatusCode, errorContent);
            return null;
        }

        return await response.Content.ReadFromJsonAsync<OnfidoCheckResponse>(cancellationToken: cancellationToken);
    }

    private class OnfidoApplicantResponse
    {
        public string Id { get; set; } = string.Empty;
    }

    private class OnfidoCheckResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Result { get; set; }
    }
}

/// <summary>
/// Mock KYC service for development/testing
/// </summary>
public class MockKycService : IKycService
{
    private readonly ILogger<MockKycService> _logger;

    public MockKycService(ILogger<MockKycService> logger)
    {
        _logger = logger;
    }

    public Task<KycResult> InitiateVerificationAsync(
        Guid userId,
        string documentType,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[MOCK KYC] Verification initiated for user {UserId} with document type {DocumentType}",
            userId, documentType);

        return Task.FromResult(new KycResult
        {
            IsSuccess = true,
            ProviderRef = $"mock_check_{Guid.NewGuid():N}",
            Status = VerificationStatus.Pending
        });
    }

    public Task<KycResult> CheckVerificationStatusAsync(
        string providerRef,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[MOCK KYC] Checking status for {ProviderRef}", providerRef);

        // Simulate auto-approval for testing
        return Task.FromResult(new KycResult
        {
            IsSuccess = true,
            ProviderRef = providerRef,
            Status = VerificationStatus.Verified,
            VerifiedAt = DateTime.UtcNow
        });
    }

    public Task<bool> PerformLivenessCheckAsync(
        Guid userId,
        byte[] imageData,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[MOCK KYC] Liveness check for user {UserId} with {Size} bytes",
            userId, imageData.Length);

        return Task.FromResult(true);
    }
}

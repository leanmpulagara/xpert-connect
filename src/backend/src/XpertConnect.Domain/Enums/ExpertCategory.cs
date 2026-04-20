namespace XpertConnect.Domain.Enums;

/// <summary>
/// Expert verification categories based on accomplishment level
/// </summary>
public enum ExpertCategory
{
    /// <summary>Subject Matter Expert - Verified via credentials, publications</summary>
    CategoryA_SME = 1,

    /// <summary>C-Suite Executive - Verified via SEC filings, company registries</summary>
    CategoryB_CSuite = 2,

    /// <summary>High-Profile/Celebrity - Verified via official representatives</summary>
    CategoryC_Celebrity = 3
}

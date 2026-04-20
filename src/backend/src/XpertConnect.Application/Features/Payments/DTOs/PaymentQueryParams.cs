using XpertConnect.Domain.Enums;

namespace XpertConnect.Application.Features.Payments.DTOs;

/// <summary>
/// Query parameters for filtering payments
/// </summary>
public class PaymentQueryParams
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public PaymentStatus? Status { get; set; }
    public Guid? ConsultationId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? SortBy { get; set; } = "CreatedAt";
    public bool SortDescending { get; set; } = true;
}

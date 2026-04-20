namespace XpertConnect.Application.Common.Models;

public class PaymentResult
{
    public bool IsSuccess { get; set; }
    public string? PaymentId { get; set; }
    public string? Error { get; set; }
    public decimal? Amount { get; set; }
    public string? Currency { get; set; }
    public string? Status { get; set; }
}

namespace XpertConnect.Domain.Enums;

public enum PaymentStatus
{
    Pending = 1,
    Authorized = 2,
    InEscrow = 3,
    Released = 4,
    Refunded = 5,
    Failed = 6,
    Disputed = 7
}

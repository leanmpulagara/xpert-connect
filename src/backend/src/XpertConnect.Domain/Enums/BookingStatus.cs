namespace XpertConnect.Domain.Enums;

public enum BookingStatus
{
    Initiated = 1,
    NdaPending = 2,
    NdaSigned = 3,
    PaymentPending = 4,
    PaymentAuthorized = 5,
    Confirmed = 6,
    Rescheduled = 7,
    InProgress = 8,
    PendingCompletion = 9,
    Completed = 10,
    FeedbackPending = 11,
    Settled = 12,
    Cancelled = 13,
    Refunded = 14,
    NoShow = 15,
    Disputed = 16
}

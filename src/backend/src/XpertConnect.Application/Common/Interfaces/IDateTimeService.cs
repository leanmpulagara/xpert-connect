namespace XpertConnect.Application.Common.Interfaces;

/// <summary>
/// Date/time service for testability
/// </summary>
public interface IDateTimeService
{
    DateTime UtcNow { get; }
}

using XpertConnect.Application.Common.Interfaces;

namespace XpertConnect.Infrastructure.Services;

/// <summary>
/// Date/time service implementation
/// </summary>
public class DateTimeService : IDateTimeService
{
    public DateTime UtcNow => DateTime.UtcNow;
}

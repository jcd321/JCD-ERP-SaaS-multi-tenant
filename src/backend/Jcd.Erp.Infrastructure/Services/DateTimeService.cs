using Jcd.Erp.Application.Common.Interfaces;

namespace Jcd.Erp.Infrastructure.Services;

public class DateTimeService : IDateTimeService
{
    public DateTime UtcNow => DateTime.UtcNow;
}

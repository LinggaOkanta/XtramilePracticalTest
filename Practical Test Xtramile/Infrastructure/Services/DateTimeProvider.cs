using Practical_Test_Xtramile.Application.Common.Interfaces;

namespace Practical_Test_Xtramile.Infrastructure.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}

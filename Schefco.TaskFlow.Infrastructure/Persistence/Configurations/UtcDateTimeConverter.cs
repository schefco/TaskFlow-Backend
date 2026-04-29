using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore;

namespace Schefco.TaskFlow.Infrastructure.Persistence.Configurations
{
    public class UtcDateTimeConverter : ValueConverter<DateTime, DateTime>
    {
        // For ensuring all DateTime are converted correctly for PostgreSQL
        public UtcDateTimeConverter() : base(
            v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
        { }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Schefco.TaskFlow.Infrastructure.Persistence.Configurations
{
    public class UtcNullableDateTimeConverter : ValueConverter<DateTime?, DateTime?>
    {
        // Normalize Null date and time for PostgreSQL
        public UtcNullableDateTimeConverter() : base(
            v => v.HasValue ? (v.Value.Kind == DateTimeKind.Utc ? v.Value : DateTime.SpecifyKind(v.Value, DateTimeKind.Utc)) : v,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v)
        { }
    }
}

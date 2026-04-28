using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Schefco.TaskFlow.Infrastructure.Persistence
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var conn = Environment.GetEnvironmentVariable("DATABASE_URL");

            Console.WriteLine(">>> FACTORY HIT");
            Console.WriteLine($">>> DATABASE_URL = '{conn}'");

            if (string.IsNullOrWhiteSpace(conn))
                throw new InvalidOperationException("DATABASE_URL is not set. EF Tools require this environment variable.");

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseNpgsql(conn);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}


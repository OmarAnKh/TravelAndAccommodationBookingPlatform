using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TravelAndAccommodationBookingPlatform.Infrastructure.Data;

public class SqlServerDbContextFactory : IDesignTimeDbContextFactory<SqlServerDbContext>
{
    public SqlServerDbContext CreateDbContext(string[] args)
    {
        Env.Load();

        var optionsBuilder = new DbContextOptionsBuilder<SqlServerDbContext>();
        var connectionString = Environment.GetEnvironmentVariable("SQLSERVERCONNECTIONSTRING");

        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException("SQLSERVERCONNECTIONSTRING not found in environment variables.");

        optionsBuilder.UseSqlServer(connectionString);

        return new SqlServerDbContext(optionsBuilder.Options);
    }
}
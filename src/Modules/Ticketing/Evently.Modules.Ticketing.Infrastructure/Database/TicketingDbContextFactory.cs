using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Evently.Modules.Ticketing.Infrastructure.Database;

internal sealed class TicketingDbContextFactory : IDesignTimeDbContextFactory<TicketingDbContext>
{
    public TicketingDbContext CreateDbContext(string[] args)
    {
        string connectionString = "Host=evently.database;Port=5432;Database=evently;Username=postgres;Password=sqluser10$;Include Error Detail=true";
        DbContextOptionsBuilder<TicketingDbContext> optionBuilder = new();

        optionBuilder.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();

        return new TicketingDbContext(optionBuilder.Options);
    }
}

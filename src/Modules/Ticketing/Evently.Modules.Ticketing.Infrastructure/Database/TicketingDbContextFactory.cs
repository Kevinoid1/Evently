using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Evently.Modules.Ticketing.Infrastructure.Database;

internal sealed class TicketingDbContextFactory(IConfiguration configuration) : IDesignTimeDbContextFactory<TicketingDbContext>
{
    public TicketingDbContext CreateDbContext(string[] args)
    {
        string connectionString =  configuration.GetConnectionString("Database");
        DbContextOptionsBuilder<TicketingDbContext> optionBuilder = new();

        optionBuilder.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();

        return new TicketingDbContext(optionBuilder.Options);
    }
}

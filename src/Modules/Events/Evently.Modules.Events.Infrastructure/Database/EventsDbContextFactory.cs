using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Evently.Modules.Events.Infrastructure.Database;

internal sealed class EventsDbContextFactory(IConfiguration configuration) : IDesignTimeDbContextFactory<EventsDbContext>
{
    public EventsDbContext CreateDbContext(string[] args)
    {
        string connectionString = configuration.GetConnectionString("Database")!;
        DbContextOptionsBuilder<EventsDbContext> optionBuilder = new();

        optionBuilder.UseNpgsql(connectionString);

        return new EventsDbContext(optionBuilder.Options);
    }
}

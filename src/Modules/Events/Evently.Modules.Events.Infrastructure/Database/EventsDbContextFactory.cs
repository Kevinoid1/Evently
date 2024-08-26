using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Evently.Modules.Events.Infrastructure.Database;

internal sealed class EventsDbContextFactory : IDesignTimeDbContextFactory<EventsDbContext>
{
    public EventsDbContext CreateDbContext(string[] args)
    {
        string connectionString = "Host=evently.database;Port=5432;Database=evently;Username=postgres;Password=sqluser10$;Include Error Detail=true";
        DbContextOptionsBuilder<EventsDbContext> optionBuilder = new();

        optionBuilder.UseNpgsql(connectionString);

        return new EventsDbContext(optionBuilder.Options);
    }
}

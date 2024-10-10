using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Evently.Modules.Events.Infrastructure.Database;

internal sealed class EventsDbContextFactory : IDesignTimeDbContextFactory<EventsDbContext>
{
    public EventsDbContext CreateDbContext(string[] args)
    {
        string connectionString = "Host=localhost;Port=5433;Database=evently;Username=postgres;Password=sqluser10$;Include Error Detail=true";
        DbContextOptionsBuilder<EventsDbContext> optionBuilder = new();

        optionBuilder.UseNpgsql(connectionString);

        optionBuilder.UseSnakeCaseNamingConvention();

        return new EventsDbContext(optionBuilder.Options);
    }
}

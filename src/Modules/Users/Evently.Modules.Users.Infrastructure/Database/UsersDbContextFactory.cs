using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Evently.Modules.Users.Infrastructure.Database;

internal sealed class UsersDbContextFactory :IDesignTimeDbContextFactory<UsersDbContext>
{
    public UsersDbContext CreateDbContext(string[] args)
    {
        string connectionString = "Host=evently.database;Port=5432;Database=evently;Username=postgres;Password=sqluser10$;Include Error Detail=true";
        DbContextOptionsBuilder<UsersDbContext> optionBuilder = new();

        optionBuilder.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();

        return new UsersDbContext(optionBuilder.Options);
    }
}


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Evently.Modules.Users.Infrastructure.Database;

internal sealed class UsersDbContextFactory(IConfiguration configuration) :IDesignTimeDbContextFactory<UsersDbContext>
{
    public UsersDbContext CreateDbContext(string[] args)
    {
        string connectionString =  configuration.GetConnectionString("Database");
        DbContextOptionsBuilder<UsersDbContext> optionBuilder = new();

        optionBuilder.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();

        return new UsersDbContext(optionBuilder.Options);
    }
}


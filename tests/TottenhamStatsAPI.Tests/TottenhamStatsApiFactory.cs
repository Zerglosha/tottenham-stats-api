using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TottenhamStatsAPI.Data;

namespace TottenhamStatsAPI.Tests;

public class TottenhamStatsApiFactory : WebApplicationFactory<Program>
{
    private static readonly object MigrationLock = new();
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<TottenhamStatsApiFactory>()
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString("TestConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Test connection string was not found. Add ConnectionStrings:TestConnection to test project user secrets.");
            }

            services.RemoveAll<DbContextOptions<AppDbContext>>();

            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));

            using var scope = services.BuildServiceProvider().CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            lock (MigrationLock)
            {
                dbContext.Database.Migrate();
            }
        });
    }
}
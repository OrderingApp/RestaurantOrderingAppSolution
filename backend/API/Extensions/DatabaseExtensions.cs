using Infrastructure.Database;
using Infrastructure.Database.Seeder;
using Infrastructure.Database.Seeder.Seeders;
using Microsoft.EntityFrameworkCore;
using RestaurantOrdering.Events.Infrastructure.Database;

namespace API.Extensions;

public static class DatabaseExtensions
{
    public static IServiceCollection AddDatabaseServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddSingleton<AuditTimestampsInterceptor>();

        services.AddDbContext<RestaurantOrderingContext>((sp, opt) =>
        {
            opt.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
            opt.AddInterceptors(sp.GetRequiredService<AuditTimestampsInterceptor>());
        });

        services.AddDbContext<EventsDatabaseContext>(opt =>
        {
            opt.UseSqlite(configuration.GetConnectionString("EventsDatabaseContext"));
        });

        // Register JSON-based seeders
        services.AddDatabaseSeeders();

        return services;
    }

    public static async Task UseDatabaseMigrationAndSeeding(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var restaurantOrderingContext = services.GetRequiredService<RestaurantOrderingContext>();
            await restaurantOrderingContext.Database.MigrateAsync();

            // Seed from JSON files in Seeder/SeedData/ (if-empty, safe to call on every startup)
            var seeder = services.GetRequiredService<DatabaseSeeder>();
            await seeder.SeedAsync();

            var eventsDatabaseContext = services.GetRequiredService<EventsDatabaseContext>();
            await eventsDatabaseContext.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred during migration or seeding");
        }
    }
}

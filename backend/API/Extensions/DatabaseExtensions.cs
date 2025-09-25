using Infrastructure.Database;
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

        return services;
    }

    public static async Task UseDatabaseMigrationAndSeeding(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var restaurantOrderingContext =
                services.GetRequiredService<RestaurantOrderingContext>();
            await restaurantOrderingContext.Database.MigrateAsync();

            var excelSeeder = new ExcelSeeder(restaurantOrderingContext);

            await excelSeeder.SeedFromExcel(
                Path.Combine(Environment.CurrentDirectory, "SeedData.xlsx")
            );

            var eventsDatabaseContext = services.GetRequiredService<EventsDatabaseContext>();
            await eventsDatabaseContext.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred during migration");
        }
    }
}

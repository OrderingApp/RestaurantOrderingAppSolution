using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Database.Seeder;

/// <summary>
/// Registers the JSON-based seeding infrastructure.
/// Call AddDatabaseSeeders() in DI setup then resolve DatabaseSeeder
/// and call SeedAsync() to seed the database from Seeder/SeedData JSON files.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabaseSeeders(this IServiceCollection services)
    {
        // Reads and deserializes JSON files from Seeder/SeedData/
        services.AddSingleton<SeedDataReader>();

        // Individual seeders – each operates in "if empty" mode
        services.AddScoped<Seeders.MenuCategorySeeder>();
        services.AddScoped<Seeders.SubCategorySeeder>();
        services.AddScoped<Seeders.IngredientCategorySeeder>();
        services.AddScoped<Seeders.IngredientSeeder>();
        services.AddScoped<Seeders.TagSeeder>();
        services.AddScoped<Seeders.AllergenSeeder>();
        services.AddScoped<Seeders.AreaSeeder>();
        services.AddScoped<Seeders.TableSeeder>();
        services.AddScoped<Seeders.MenuItemSeeder>();
        services.AddScoped<Seeders.IngredientTagRelSeeder>();
        services.AddScoped<Seeders.IngredientAllergenRelSeeder>();
        services.AddScoped<Seeders.MenuItemIngredientRelSeeder>();

        // Orchestrator
        services.AddScoped<Seeders.DatabaseSeeder>();

        return services;
    }
}

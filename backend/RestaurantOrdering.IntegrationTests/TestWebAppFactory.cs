// RestaurantOrdering.IntegrationTests/TestWebAppFactory.cs
using Infrastructure.Database;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RestaurantOrdering.Events.Infrastructure.Database;

public class TestWebAppFactory : WebApplicationFactory<Program>
{
    private SqliteConnection? _mainConn;
    private SqliteConnection? _eventsConn;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");

        builder.ConfigureServices(services =>
        {
            // 1) Usuń produkcyjne DbContexty
            RemoveDbContext<RestaurantOrderingContext>(services);
            RemoveDbContext<EventsDatabaseContext>(services);

            // 2) Dodaj SQLite in-memory (oddzielne połączenia na konteksty)
            _mainConn = new SqliteConnection("DataSource=:memory:");
            _mainConn.Open();

            _eventsConn = new SqliteConnection("DataSource=:memory:");
            _eventsConn.Open();

            services.AddDbContext<RestaurantOrderingContext>((sp, opt) =>
            {
                opt.UseSqlite(_mainConn);
                // jeśli w prod używasz interceptorów, możesz je dodać też w testach
                var interceptor = sp.GetService<AuditTimestampsInterceptor>();
                if (interceptor != null) opt.AddInterceptors(interceptor);
            });

            services.AddDbContext<EventsDatabaseContext>(opt =>
            {
                opt.UseSqlite(_eventsConn);
            });

            // 3) Zbuduj provider, utwórz schemat i seed minimalny
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();

            var main = scope.ServiceProvider.GetRequiredService<RestaurantOrderingContext>();
            main.Database.EnsureCreated();
            // Minimalny seed do testów – szybki i stabilny:
            if (!main.MenuItems.Any())
            {
                main.MenuItems.Add(new Domain.MenuItem
                {
                    Id = Guid.NewGuid(),
                    Name = "Pizza",
                    Price = 12.5m
                });
                main.SaveChanges();
            }

            var eventsDb = scope.ServiceProvider.GetRequiredService<EventsDatabaseContext>();
            eventsDb.Database.EnsureCreated();

            // Uwaga: NIE wywołujemy tutaj UseDatabaseMigrationAndSeeding()!
        });
    }
    private static void RemoveDbContext<TContext>(IServiceCollection services)
        where TContext : DbContext
    {
        services.RemoveAll<DbContextOptions<TContext>>();
        services.RemoveAll<TContext>();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _mainConn?.Dispose();
        _eventsConn?.Dispose();
    }
}

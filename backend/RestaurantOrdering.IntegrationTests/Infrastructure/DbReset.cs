using Infrastructure.Database;

public static class DbReset
{
    public static async Task ResetAsync(IServiceProvider sp)
    {
        using var scope = sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<RestaurantOrderingContext>();
        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();

        var eventsDb = scope.ServiceProvider.GetRequiredService<RestaurantOrderingContext>();
        await eventsDb.Database.EnsureDeletedAsync();
        await eventsDb.Database.EnsureCreatedAsync();
    }
}

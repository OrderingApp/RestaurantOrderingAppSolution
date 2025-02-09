using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using RestaurantOrdering.Events.Domain;
using RestaurantOrdering.Events.Infrastructure.Database;

public class EventContextInterceptor : SaveChangesInterceptor
{
    private readonly IEventContextMiddleware _middleware;

    public EventContextInterceptor(IEventContextMiddleware middleware)
    {
        _middleware = middleware;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        HandleEventContext(eventData);
        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        HandleEventContext(eventData);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void HandleEventContext(DbContextEventData eventData)
    {
        var context = eventData.Context;
        if (context == null) return;

        var addedEntities = context.ChangeTracker.Entries<EventContext>()
            .Where(e => e.State == EntityState.Added)
            .Select(e => e.Entity)
            .ToList();

        foreach (var entity in addedEntities)
        {
            _middleware.HandleEventContextAdded(entity);
        }
    }
}

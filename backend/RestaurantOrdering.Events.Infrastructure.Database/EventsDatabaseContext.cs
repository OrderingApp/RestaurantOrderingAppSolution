using Microsoft.EntityFrameworkCore;
using RestaurantOrdering.Events.Domain;

namespace RestaurantOrdering.Events.Infrastructure.Database;
public class EventsDatabaseContext : DbContext
{
    public EventsDatabaseContext(DbContextOptions options) : base(options)
    {
        
    }

    public DbSet<EventContext> EventContexts { get; set; }


    //public override int SaveChanges()
    //{
    //    TriggerMiddlewareForEventContext();
    //    return base.SaveChanges();
    //}

    //public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    //{
    //    TriggerMiddlewareForEventContext();
    //    return await base.SaveChangesAsync(cancellationToken);
    //}

    //private void TriggerMiddlewareForEventContext()
    //{
    //    var addedEntities = ChangeTracker.Entries<EventContext>()
    //        .Where(e => e.State == EntityState.Added)
    //        .Select(e => e.Entity)
    //        .ToList();

    //    foreach (var entity in addedEntities)
    //    {
    //        _middleware.HandleEventContextAdded(entity);
    //    }
    //}
}

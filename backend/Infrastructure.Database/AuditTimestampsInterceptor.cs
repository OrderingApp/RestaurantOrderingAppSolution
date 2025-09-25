using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Database;

public sealed class AuditTimestampsInterceptor : SaveChangesInterceptor
{
    private static readonly TimeZoneInfo PolandTz =
        TimeZoneInfo.FindSystemTimeZoneById("Europe/Warsaw");

    private static void Stamp(EntityEntry entry)
    {
        if (entry.Entity is not Domain.AuditableEntity auditable) return;

        var nowPl = TimeZoneInfo.ConvertTime(DateTime.UtcNow, PolandTz);

        if (entry.State == EntityState.Added)
        {
            if(auditable.CreatedAt == default)
                auditable.CreatedAt = nowPl;

            auditable.LastModified = null;
        }
        else if(entry.State == EntityState.Modified)
        {
            auditable.LastModified = nowPl;

            entry.Property(nameof(Domain.AuditableEntity.CreatedAt)).IsModified = false;
        }
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        var ctx = eventData.Context;

        if(ctx is null) return base.SavingChanges(eventData, result);

        foreach(var entry in ctx.ChangeTracker.Entries())
            Stamp(entry);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, 
        InterceptionResult<int> result, 
        CancellationToken cancellationToken = default)
    {
        var ctx = eventData.Context;
        
        if(ctx is not null)
        {
            foreach(var entry in ctx.ChangeTracker.Entries()) Stamp(entry);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}

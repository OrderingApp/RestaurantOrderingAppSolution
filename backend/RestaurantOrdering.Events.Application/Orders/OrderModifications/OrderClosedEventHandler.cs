using Domain;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using RestaurantOrdering.Events.Domain.Orders.ModificationsOrder;

namespace RestaurantOrdering.Events.Application.Orders.OrderModifications;

public class OrderClosedEventHandler(RestaurantOrderingContext restaurantOrderingContext)
{
    public async Task HandleEventAsync(OrderClosedEvent orderClosedEvent)
    {
        var today = DateTime.Now.Date;

        var totalAmount = orderClosedEvent.Payments.Sum(p => p.Amount);
        var existingDailyRevenue = await restaurantOrderingContext.SalesRevenues
            .FirstOrDefaultAsync(sr => sr.Date == today);

        if (existingDailyRevenue != null)
        {
            existingDailyRevenue.Amount += totalAmount;
        }
        else
        {
            await restaurantOrderingContext.SalesRevenues.AddAsync(new SalesRevenue
            {
                Amount = totalAmount,
                Date = today
            });
        }

        var menuItemIds = orderClosedEvent.OrderItems.Select(oi => oi.MenuItemId).Distinct().ToList();
        var existingMenuItemSales = await restaurantOrderingContext.MenuItemSales
            .Where(mi => menuItemIds.Contains(mi.MenuItemId) && mi.Date == today)
            .ToDictionaryAsync(mi => mi.MenuItemId);

        List<MenuItemSale> newMenuItemSales = new();

        foreach (var orderItem in orderClosedEvent.OrderItems)
        {
            if (existingMenuItemSales.TryGetValue(orderItem.MenuItemId, out var existingSale))
            {
                existingSale.Amount++;
            }
            else
            {
                newMenuItemSales.Add(new MenuItemSale
                {
                    Amount = 1,
                    Date = today,
                    MenuItemId = orderItem.MenuItemId
                });
            }
        }

        if (newMenuItemSales.Count > 0)
        {
            await restaurantOrderingContext.MenuItemSales.AddRangeAsync(newMenuItemSales);
        }

        await restaurantOrderingContext.SaveChangesAsync();
    }
}

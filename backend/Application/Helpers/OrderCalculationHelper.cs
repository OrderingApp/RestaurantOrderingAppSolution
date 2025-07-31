using Domain;
using Application.Dtos.OrderItems;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Database;

namespace Application.Helpers;

public static class OrderCalculationHelper
{
    public static async Task<List<OrderItem>> PopulateOrderItemsAsync(
        RestaurantOrderingContext context,
        IEnumerable<OrderItemCreateDto> itemDtos,
        Guid orderId
    )
    {
        var menuItemIds = itemDtos.Select(dto => dto.MenuItemId).Distinct();

        var menuItems = await context.MenuItems
            .Where(mi => menuItemIds.Contains(mi.Id))
            .Include(mi => mi.MenuItemIngredientRels)
            .ThenInclude(rel => rel.Ingredient)
            .ToDictionaryAsync(mi => mi.Id);

        var ingredientIds = itemDtos
            .SelectMany(dto => dto.ExtraIngredients.Select(e => e.IngredientId)
                             .Concat(dto.RemovedIngredientIds))
            .Distinct()
            .ToList();

        var ingredients = await context.Ingredients
            .Where(i => ingredientIds.Contains(i.Id))
            .ToDictionaryAsync(i => i.Id);

        var orderItems = new List<OrderItem>();

        foreach (var dto in itemDtos)
        {
            if (!menuItems.TryGetValue(dto.MenuItemId, out var menuItem))
                throw new KeyNotFoundException($"MenuItem with ID {dto.MenuItemId} not found.");

            var orderItem = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                MenuItemId = dto.MenuItemId,
                SpecialInstructions = dto.SpecialInstructions,
                Price = menuItem.Price,
            };

            if (dto.ExtraIngredients.Any())
            {
                HandleExtraIngredients(orderItem, dto, ingredients);
            }

            if (dto.RemovedIngredientIds.Any())
            {
                HandleRemovedIngredients(orderItem, dto, menuItem);
            }

            orderItems.Add(orderItem);
        }

        return orderItems;
    }

    public static void HandleExtraIngredients(
        OrderItem orderItem,
        OrderItemCreateDto dto,
        Dictionary<Guid, Ingredient> ingredients
    )
    {
        foreach (var extra in dto.ExtraIngredients)
        {
            if (ingredients.TryGetValue(extra.IngredientId, out var ingredient))
            {
                var extraCost = ingredient.Price * extra.Quantity;
                orderItem.Price += extraCost;
                orderItem.ExtraIngredients.Add(new OrderItemIngredient
                {
                    Id = Guid.NewGuid(),
                    Name = ingredient.Name,
                    Price = ingredient.Price,
                    Quantity = extra.Quantity,
                });
            }
        }
    }

    public static void HandleRemovedIngredients(
        OrderItem orderItem,
        OrderItemCreateDto dto,
        MenuItem menuItem
    )
    {
        var removableIngredients = menuItem.MenuItemIngredientRels
            .Select(r => r.Ingredient)
            .Where(i => dto.RemovedIngredientIds.Contains(i.Id))
            .ToList();

        foreach (var ingredient in removableIngredients)
        {
            orderItem.RemovedIngredients.Add(new OrderItemIngredient
            {
                Id = Guid.NewGuid(),
                Name = ingredient.Name,
                Price = 0,
                Quantity = 1,
            });
        }
    }

    public static decimal RecalculateOrderTotal(Order order)
    {
        return order.OrderItems.Sum(oi => oi.Price * (1 - (oi.Discount / 100))) - order.Discount;
    }
}

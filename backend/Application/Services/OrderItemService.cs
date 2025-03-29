using System.Net;
using Application.Contracts;
using Application.Dtos.Common;
using Application.Dtos.OrderItems;
using Application.Dtos.Orders;
using AutoMapper;
using Domain;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using RestaurantOrdering.Events.Application.Contracts;
using RestaurantOrdering.Events.Domain.OrderItems;

namespace Application.Services;

public class OrderItemService(
    RestaurantOrderingContext orderingContext,
    IEventHandlerService eventHandlerService,
    IMapper mapper
) : IOrderItemService
{
    public async Task<ResultDto<OrderReadDto>> AddOrderItems(
        Guid orderId,
        List<OrderItemCreateDto> orderItemDtos
    )
    {
        try
        {
            var order = await orderingContext
                .Orders.Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return ResultDto<OrderReadDto>.Failure("Order not found.", HttpStatusCode.NotFound);

            var menuItemIds = orderItemDtos.Select(dto => dto.MenuItemId).Distinct();
            var menuItems = await orderingContext
                .MenuItems.Where(mi => menuItemIds.Contains(mi.Id))
                .Include(mi => mi.MenuItemIngredientRels)
                .ThenInclude(rel => rel.Ingredient)
                .ToDictionaryAsync(mi => mi.Id);

            var orderItems = new List<OrderItem>();

            foreach (var dto in orderItemDtos)
            {
                if (!menuItems.TryGetValue(dto.MenuItemId, out var menuItem))
                    return ResultDto<OrderReadDto>.Failure(
                        $"MenuItem with ID {dto.MenuItemId} not found.",
                        HttpStatusCode.BadRequest
                    );

                var orderItem = mapper.Map<OrderItem>(dto);
                orderItem.OrderId = orderId;
                orderItem.Price = menuItem.Price;

                if (dto.ExtraIngredients.Any())
                {
                    var extraIngredients = await orderingContext
                        .Ingredients.Where(i =>
                            dto.ExtraIngredients.Select(ei => ei.IngredientId).Contains(i.Id)
                        )
                        .ToListAsync();

                    foreach (var extra in dto.ExtraIngredients)
                    {
                        var ingredient = extraIngredients.FirstOrDefault(i =>
                            i.Id == extra.IngredientId
                        );
                        if (ingredient != null)
                        {
                            orderItem.Price += ingredient.Price * extra.Quantity;
                            orderItem.ExtraIngredients.Add(
                                new OrderItemIngredient
                                {
                                    Id = Guid.NewGuid(),
                                    Name = ingredient.Name,
                                    Price = ingredient.Price,
                                    Quantity = extra.Quantity,
                                }
                            );
                        }
                    }
                }

                if (dto.RemovedIngredientIds.Any())
                {
                    await HandleRemovedIngredients(orderItem, dto.RemovedIngredientIds);
                }

                orderItems.Add(orderItem);
            }

            await orderingContext.OrderItems.AddRangeAsync(orderItems);
            order.TotalAmount = RecalculateOrderTotal(order);
            await orderingContext.SaveChangesAsync();

            var orderItemsAddedEvent = mapper.Map<OrderItemAddedEvent>((orderId, orderItems));
            await eventHandlerService.HandleEventAsync(orderItemsAddedEvent);

            var updatedOrderDto = mapper.Map<OrderReadDto>(order);
            return ResultDto<OrderReadDto>.Success(updatedOrderDto, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<OrderReadDto>.Failure(
                $"An error occurred: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ResultDto<OrderItemReadDto>> GetOrderItem(Guid orderId, Guid id)
    {
        try
        {
            var orderItem = await orderingContext
                .OrderItems.Include(oi => oi.MenuItem)
                .ThenInclude(mi => mi.MenuItemIngredientRels)
                .ThenInclude(rel => rel.Ingredient)
                .FirstOrDefaultAsync(oi => oi.Id == id && oi.OrderId == orderId);

            if (orderItem == null)
                return ResultDto<OrderItemReadDto>.Failure(
                    "Order item not found.",
                    HttpStatusCode.NotFound
                );

            var orderItemDto = mapper.Map<OrderItemReadDto>(orderItem);
            return ResultDto<OrderItemReadDto>.Success(orderItemDto, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<OrderItemReadDto>.Failure(
                $"An error occurred: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ResultDto<List<OrderItemsListDto>>> GetOrderItems(Guid orderId)
    {
        try
        {
            var orderItems = await orderingContext
                .OrderItems.Where(oi => oi.OrderId == orderId)
                .Include(oi => oi.MenuItem)
                .OrderBy(oi => oi.Status == OrderItemStatus.Pending ? 0 : 1)
                .ToListAsync();

            var orderItemsDto = mapper.Map<List<OrderItemsListDto>>(orderItems);
            return ResultDto<List<OrderItemsListDto>>.Success(orderItemsDto, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<List<OrderItemsListDto>>.Failure(
                $"An error occurred: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ResultDto<OrderItemReadDto>> UpdateOrderItem(
        Guid orderId,
        Guid id,
        OrderItemUpdateDto updateDto
    )
    {
        try
        {
            var orderItem = await orderingContext
                .OrderItems.Include(oi => oi.MenuItem)
                .ThenInclude(mi => mi.MenuItemIngredientRels)
                .ThenInclude(rel => rel.Ingredient)
                .Include(oi => oi.ExtraIngredients)
                .Include(oi => oi.RemovedIngredients)
                .FirstOrDefaultAsync(oi => oi.Id == id && oi.OrderId == orderId);

            if (orderItem == null)
                return ResultDto<OrderItemReadDto>.Failure(
                    "Order item not found.",
                    HttpStatusCode.NotFound
                );

            if (!string.IsNullOrWhiteSpace(updateDto.SpecialInstructions))
                orderItem.SpecialInstructions = updateDto.SpecialInstructions;

            if (updateDto.Discount.HasValue)
                orderItem.Discount = updateDto.Discount.Value;

            if (updateDto.ExtraIngredients.Any())
            {
                var extraIngredients = await orderingContext
                    .Ingredients.Where(i =>
                        updateDto.ExtraIngredients.Select(ei => ei.IngredientId).Contains(i.Id)
                    )
                    .ToListAsync();

                foreach (var extra in updateDto.ExtraIngredients)
                {
                    var ingredient = extraIngredients.FirstOrDefault(i =>
                        i.Id == extra.IngredientId
                    );
                    if (ingredient != null)
                    {
                        orderItem.Price += ingredient.Price * extra.Quantity;
                        orderItem.ExtraIngredients.Add(
                            new OrderItemIngredient
                            {
                                Id = Guid.NewGuid(),
                                Name = ingredient.Name,
                                Price = ingredient.Price,
                                Quantity = extra.Quantity,
                            }
                        );
                    }
                }
            }

            if (updateDto.RemovedIngredientIds.Any())
            {
                await HandleRemovedIngredients(orderItem, updateDto.RemovedIngredientIds);
            }

            await orderingContext.SaveChangesAsync();

            var orderItemUpdatedEvent = mapper.Map<OrderItemUpdatedEvent>(orderItem);
            await eventHandlerService.HandleEventAsync(orderItemUpdatedEvent);

            var updatedOrderItem = mapper.Map<OrderItemReadDto>(orderItem);
            return ResultDto<OrderItemReadDto>.Success(updatedOrderItem, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<OrderItemReadDto>.Failure(
                $"An error occurred: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ResultDto<bool>> UpdateOrderItemStatus(
        Guid orderId,
        Guid id,
        OrderItemStatus status
    )
    {
        try
        {
            var orderItem = await orderingContext.OrderItems.FirstOrDefaultAsync(oi =>
                oi.Id == id && oi.OrderId == orderId
            );

            if (orderItem == null)
                return ResultDto<bool>.Failure("Order item not found.", HttpStatusCode.NotFound);
            var previousStatus = orderItem.Status;

            orderItem.Status = status;
            await orderingContext.SaveChangesAsync();

            var orderItemStatusUpdatedEvent = mapper.Map<OrderItemStatusUpdatedEvent>(
                (orderItem, previousStatus)
            );
            await eventHandlerService.HandleEventAsync(orderItemStatusUpdatedEvent);

            return ResultDto<bool>.Success(true, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<bool>.Failure(
                $"An error occurred: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ResultDto<bool>> DeleteOrderItem(Guid orderId, Guid id)
    {
        try
        {
            var orderItem = await orderingContext.OrderItems.FirstOrDefaultAsync(oi =>
                oi.Id == id && oi.OrderId == orderId
            );

            if (orderItem == null)
                return ResultDto<bool>.Failure("Order item not found.", HttpStatusCode.NotFound);

            var orderItemDeletedEvent = mapper.Map<OrderItemDeletedEvent>((orderItem));
            await eventHandlerService.HandleEventAsync(orderItemDeletedEvent);

            orderingContext.OrderItems.Remove(orderItem);
            await orderingContext.SaveChangesAsync();

            return ResultDto<bool>.Success(true, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<bool>.Failure(
                $"An error occurred: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    private decimal RecalculateOrderTotal(Order order)
    {
        return order.OrderItems.Sum(oi => oi.Price * (1 - (oi.Discount / 100))) - order.Discount;
    }

    private async Task HandleRemovedIngredients(
        OrderItem orderItem,
        List<Guid> removedIngredientIds
    )
    {
        if (!removedIngredientIds.Any())
            return;

        var menuItemIngredients = await orderingContext
            .MenuItemIngredientRels.Where(mi => mi.MenuItemId == orderItem.MenuItemId)
            .Select(mi => mi.Ingredient)
            .ToListAsync();

        // Remove from extra ingredients (and adjust price)
        var removedExtraIngredients = orderItem
            .ExtraIngredients.Where(extra => removedIngredientIds.Contains(extra.Id))
            .ToList();

        foreach (var extra in removedExtraIngredients)
        {
            orderItem.Price = Math.Max(0, orderItem.Price - (extra.Price * extra.Quantity));
            orderItem.ExtraIngredients.Remove(extra);
        }

        var removedFromMenuItem = menuItemIngredients
            .Where(mi => removedIngredientIds.Contains(mi.Id))
            .ToList();

        // Add removed ingredients from MenuItem to RemovedIngredients
        foreach (var ingredient in removedFromMenuItem)
        {
            // Check if the ingredient is already marked as removed
            if (!orderItem.RemovedIngredients.Any(ri => ri.Name == ingredient.Name))
            {
                orderItem.RemovedIngredients.Add(
                    new OrderItemIngredient
                    {
                        Id = Guid.NewGuid(),
                        Name = ingredient.Name,
                        Price = 0,
                        Quantity = 1,
                    }
                );
            }
        }
    }
}

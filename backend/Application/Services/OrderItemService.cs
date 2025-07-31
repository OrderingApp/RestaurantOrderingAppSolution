using System.Net;
using Application.Contracts;
using Application.Dtos.Common;
using Application.Dtos.OrderItems;
using Application.Dtos.Orders;
using Application.Helpers;
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
    public async Task<ResultDto<OrderReadDto>> AddOrderItemsToOrder(
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

            var orderItems = await OrderCalculationHelper.PopulateOrderItemsAsync(
                orderingContext,
                orderItemDtos,
                orderId
            );

            await orderingContext.OrderItems.AddRangeAsync(orderItems);

            order.TotalAmount = OrderCalculationHelper.RecalculateOrderTotal(order);

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
                return ResultDto<OrderItemReadDto>.Failure("Order item not found.", HttpStatusCode.NotFound);

            // Update basic fields
            orderItem.SpecialInstructions = updateDto.SpecialInstructions;
            orderItem.Discount = updateDto.Discount ?? 0;

            // Reset base price to menu item price
            var basePrice = orderItem.MenuItem.Price;
            orderItem.Price = basePrice;

            // Clear current modifications
            orderItem.ExtraIngredients.Clear();
            orderItem.RemovedIngredients.Clear();

            // Load all involved ingredients
            var allIngredientIds = updateDto.ExtraIngredients.Select(e => e.IngredientId)
                .Concat(updateDto.RemovedIngredientIds)
                .Distinct()
                .ToList();

            var ingredients = await orderingContext.Ingredients
                .Where(i => allIngredientIds.Contains(i.Id))
                .ToDictionaryAsync(i => i.Id);

            // Apply extra ingredients
            foreach (var extra in updateDto.ExtraIngredients)
            {
                if (ingredients.TryGetValue(extra.IngredientId, out var ing))
                {
                    orderItem.Price += ing.Price * extra.Quantity;
                    orderItem.ExtraIngredients.Add(new OrderItemIngredient
                    {
                        Id = Guid.NewGuid(),
                        Name = ing.Name,
                        Price = ing.Price,
                        Quantity = extra.Quantity
                    });
                }
            }

            // Apply removed ingredients
            var removableIngredients = orderItem.MenuItem.MenuItemIngredientRels
                .Select(r => r.Ingredient)
                .Where(i => updateDto.RemovedIngredientIds.Contains(i.Id))
                .ToList();

            foreach (var removed in removableIngredients)
            {
                orderItem.RemovedIngredients.Add(new OrderItemIngredient
                {
                    Id = Guid.NewGuid(),
                    Name = removed.Name,
                    Price = 0,
                    Quantity = 1
                });
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
}

using System.Net;
using Application.Contracts;
using Application.Dtos.Common;
using Application.Dtos.OrderItems;
using Application.Dtos.Orders;
using Application.Dtos.Orders.OrderDelivery;
using Application.Dtos.Orders.OrderDineIn;
using Application.Dtos.Orders.OrderTakeAway;
using AutoMapper;
using Domain;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using RestaurantOrdering.Events.Application.Contracts;
using RestaurantOrdering.Events.Domain.Orders.CreatingOrder;
using RestaurantOrdering.Events.Domain.Orders.DeleteOrder;
using RestaurantOrdering.Events.Domain.Orders.DiscountsOrder;
using RestaurantOrdering.Events.Domain.Orders.ModificationsOrder;
using RestaurantOrdering.Events.Domain.Orders.PaymentsOrder;

namespace Application.Services;

public class OrderService(
    RestaurantOrderingContext orderingContext,
    IEventHandlerService eventHandlerService,
    IMapper mapper
) : IOrderService
{
    public async Task<ResultDto<OrderReadDto>> CreateDineInOrder(
        DineInOrderCreateDto dineInOrderDto
    )
    {
        try
        {
            var table = await orderingContext.Tables.FirstOrDefaultAsync(t =>
                t.Id == dineInOrderDto.TableId
            );

            if (table == null)
                return ResultDto<OrderReadDto>.Failure(
                    "Specified table does not exist.",
                    HttpStatusCode.BadRequest
                );

            var dineInOrder = mapper.Map<Order>(dineInOrderDto);

            dineInOrder.OrderItems = await PopulateOrderItemsAsync(
                dineInOrderDto.OrderItems,
                dineInOrder.Id
            );
            dineInOrder.TotalAmount = dineInOrder.OrderItems.Sum(oi => oi.Price);

            table.Status = dineInOrder.OrderItems.Any()
                ? TableStatus.PendingServingOrderItems
                : TableStatus.Ongoing;

            var result = await orderingContext.Orders.AddAsync(dineInOrder);
            await orderingContext.SaveChangesAsync();

            var createdOrderDto = mapper.Map<OrderReadDto>(dineInOrder);

            var orderCreatedEvent = mapper.Map<DineInOrderCreatedEvent>(result.Entity);
            await eventHandlerService.HandleEventAsync(orderCreatedEvent);

            return ResultDto<OrderReadDto>.Success(createdOrderDto, HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            return ResultDto<OrderReadDto>.Failure(
                $"An error occurred: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ResultDto<OrderReadDto>> CreateTakeawayOrder(
        TakeawayOrderCreateDto takeawayOrderDto
    )
    {
        try
        {
            var takeawayOrder = mapper.Map<Order>(takeawayOrderDto);

            takeawayOrder.OrderItems = await PopulateOrderItemsAsync(
                takeawayOrderDto.OrderItems,
                takeawayOrder.Id
            );
            takeawayOrder.TotalAmount = takeawayOrder.OrderItems.Sum(oi => oi.Price);

            var result = await orderingContext.Orders.AddAsync(takeawayOrder);
            await orderingContext.SaveChangesAsync();

            var orderReadDto = mapper.Map<OrderReadDto>(takeawayOrder);

            var orderCreatedEvent = mapper.Map<TakeawayOrderCreatedEvent>(result.Entity);
            await eventHandlerService.HandleEventAsync(orderCreatedEvent);

            return ResultDto<OrderReadDto>.Success(orderReadDto, HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            return ResultDto<OrderReadDto>.Failure(
                $"An error occurred: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ResultDto<OrderReadDto>> CreateDeliveryOrder(
        DeliveryOrderCreateDto deliveryOrderDto
    )
    {
        try
        {
            var deliveryOrder = mapper.Map<Order>(deliveryOrderDto);

            deliveryOrder.OrderItems = await PopulateOrderItemsAsync(
                deliveryOrderDto.OrderItems,
                deliveryOrder.Id
            );
            deliveryOrder.TotalAmount = deliveryOrder.OrderItems.Sum(oi => oi.Price);

            var result = await orderingContext.Orders.AddAsync(deliveryOrder);
            await orderingContext.SaveChangesAsync();

            var orderReadDto = mapper.Map<OrderReadDto>(deliveryOrder);

            var orderCreatedEvent = mapper.Map<DeliveryOrderCreatedEvent>(result.Entity);
            await eventHandlerService.HandleEventAsync(orderCreatedEvent);

            return ResultDto<OrderReadDto>.Success(orderReadDto, HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            return ResultDto<OrderReadDto>.Failure(
                $"An error occurred: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ResultDto<OrderReadDto>> GetOrder(Guid id)
    {
        try
        {
            var order = await orderingContext
                .Orders.Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return ResultDto<OrderReadDto>.Failure("Order not found", HttpStatusCode.NotFound);

            var orderDto = mapper.Map<OrderReadDto>(order);

            return ResultDto<OrderReadDto>.Success(orderDto, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<OrderReadDto>.Failure(
                $"An error occurred: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ResultDto<List<OrderReadDto>>> GetOrders(OrderStatus? orderStatus)
    {
        try
        {
            var query = orderingContext
                .Orders.Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.CustomerInformation)
                .AsQueryable();

            if (orderStatus.HasValue)
            {
                query = query.Where(o => o.Status == orderStatus.Value);
            }

            var orders = await query.ToListAsync();

            var orderDtos = mapper.Map<List<OrderReadDto>>(orders);

            return ResultDto<List<OrderReadDto>>.Success(orderDtos, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<List<OrderReadDto>>.Failure(
                $"An error occurred: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ResultDto<List<NonDineInOrderSummaryDto>>> GetOngoingNonDineInOrders(
        OrderType orderType,
        DateTime date
    )
    {
        try
        {
            if (orderType != OrderType.Delivery && orderType != OrderType.Takeaway)
            {
                return ResultDto<List<NonDineInOrderSummaryDto>>.Failure(
                    "Invalid order type. Only 'Delivery' and 'Takeaway' types are allowed.",
                    HttpStatusCode.BadRequest
                );
            }

            var nextDay = date.AddDays(1);

            var ongoingOrdersQuery = orderingContext.Orders.Where(o =>
                o.Type == orderType
                && o.Status == OrderStatus.Ongoing
                && o.DateTime >= date
                && o.DateTime < nextDay
            );

            // Get only 10 latest closed orders for the day
            var closedOrdersQuery = orderingContext
                .Orders.Where(o =>
                    o.Type == orderType
                    && o.Status == OrderStatus.Closed
                    && o.DateTime >= date
                    && o.DateTime < nextDay
                )
                .OrderByDescending(o => o.DateTime)
                .Take(10);

            var ordersQuery = ongoingOrdersQuery
                .Union(closedOrdersQuery)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.CustomerInformation);

            var orders = await ordersQuery.ToListAsync();

            var orderDtos = mapper.Map<List<NonDineInOrderSummaryDto>>(orders);

            return ResultDto<List<NonDineInOrderSummaryDto>>.Success(orderDtos, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<List<NonDineInOrderSummaryDto>>.Failure(
                $"An error occurred: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ResultDto<List<OrderSummaryDto>>> GetOngoingOrdersForTable(Guid tableId)
    {
        try
        {
            var ongoingOrders = await orderingContext
                .Orders.Where(o => o.TableId == tableId && o.Status == OrderStatus.Ongoing)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .ToListAsync();

            if (!ongoingOrders.Any())
                return ResultDto<List<OrderSummaryDto>>.Failure(
                    "No ongoing orders found for this table.",
                    HttpStatusCode.NotFound
                );

            var orderDtos = mapper.Map<List<OrderSummaryDto>>(ongoingOrders);
            return ResultDto<List<OrderSummaryDto>>.Success(orderDtos, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<List<OrderSummaryDto>>.Failure(
                $"An error occurred: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ResultDto<OrderReadDto>> ApplyOrderDiscount(
        Guid id,
        decimal discountPercentage
    )
    {
        try
        {
            var order = await orderingContext
                .Orders.Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return ResultDto<OrderReadDto>.Failure("Order not found.", HttpStatusCode.NotFound);

            if (discountPercentage < 0 || discountPercentage > 100)
                return ResultDto<OrderReadDto>.Failure(
                    "Invalid discount percentage.",
                    HttpStatusCode.BadRequest
                );

            order.Discount = discountPercentage;
            order.TotalAmount = RecalculateOrderTotal(order);

            await orderingContext.SaveChangesAsync();

            var updatedOrder = mapper.Map<OrderReadDto>(order);

            var orderDiscountEvent = mapper.Map<OrderDiscountAppliedEvent>(order);
            await eventHandlerService.HandleEventAsync(orderDiscountEvent);

            return ResultDto<OrderReadDto>.Success(updatedOrder, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<OrderReadDto>.Failure(
                $"An error occurred: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ResultDto<OrderReadDto>> ChangeOrderTable(Guid id, Guid newTableId)
    {
        try
        {
            var order = await orderingContext
                .Orders.Include(o => o.Table)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return ResultDto<OrderReadDto>.Failure("Order not found.", HttpStatusCode.NotFound);

            if (order.TableId == null)
                return ResultDto<OrderReadDto>.Failure(
                    "This order is not associated with a table.",
                    HttpStatusCode.BadRequest
                );

            var newTable = await orderingContext.Tables.FirstOrDefaultAsync(t =>
                t.Id == newTableId
            );

            if (newTable == null)
                return ResultDto<OrderReadDto>.Failure(
                    "Specified table does not exist.",
                    HttpStatusCode.BadRequest
                );

            if (newTable.Status == TableStatus.Ongoing)
                return ResultDto<OrderReadDto>.Failure(
                    "The specified table is currently occupied.",
                    HttpStatusCode.Conflict
                );

            if (order.Table != null)
            {
                order.Table.Status = TableStatus.Available;
                orderingContext.Tables.Update(order.Table);
            }

            newTable.Status = TableStatus.Ongoing;
            order.TableId = newTableId;

            orderingContext.Orders.Update(order);
            orderingContext.Tables.Update(newTable);

            await orderingContext.SaveChangesAsync();

            var updatedOrderDto = mapper.Map<OrderReadDto>(order);

            var orderTableChangeEvent = mapper.Map<OrderTableChangedEvent>(order);
            await eventHandlerService.HandleEventAsync(orderTableChangeEvent);

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

    public async Task<ResultDto<OrderReadDto>> CloseOrder(Guid id)
    {
        try
        {
            var order = await orderingContext
                .Orders.Include(o => o.OrderItems)
                .Include(o => o.Payments)
                .Include(o => o.Table)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return ResultDto<OrderReadDto>.Failure("Order not found.", HttpStatusCode.NotFound);

            if (order.Status == OrderStatus.Closed)
                return ResultDto<OrderReadDto>.Failure(
                    "Order is already closed.",
                    HttpStatusCode.BadRequest
                );

            var totalDue = Math.Max(0, order.TotalAmount * (1 - (order.Discount / 100m)));
            var totalPaid = order.Payments.Sum(p => p.Amount);

            if (totalPaid < totalDue)
                return ResultDto<OrderReadDto>.Failure(
                    "Payments do not fully cover the order total.",
                    HttpStatusCode.BadRequest
                );

            order.Status = OrderStatus.Closed;

            if (order.Table != null)
            {
                order.Table.Status = TableStatus.Available;
                orderingContext.Tables.Update(order.Table);
            }

            await orderingContext.SaveChangesAsync();

            var orderClosedEvent = mapper.Map<OrderClosedEvent>(order);
            await eventHandlerService.HandleEventAsync(orderClosedEvent);

            var orderReadDto = mapper.Map<OrderReadDto>(order);

            return ResultDto<OrderReadDto>.Success(orderReadDto, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<OrderReadDto>.Failure(
                $"An error occurred: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ResultDto<OrderReadDto>> UpdateOrderStatus(Guid id, OrderStatus newStatus)
    {
        try
        {
            var order = await orderingContext
                .Orders.Include(o => o.OrderItems)
                .Include(o => o.Table)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return ResultDto<OrderReadDto>.Failure("Order not found", HttpStatusCode.NotFound);

            var previousOrderStatus = order.Status;

            order.Status = newStatus;

            if (newStatus == OrderStatus.Closed && order.Table != null)
            {
                order.Table.Status = TableStatus.Available;
                orderingContext.Tables.Update(order.Table);
            }

            await orderingContext.SaveChangesAsync();

            var orderStatusChangedEvent = mapper.Map<OrderStatusChangedEvent>(
                (order, previousOrderStatus)
            );
            await eventHandlerService.HandleEventAsync(orderStatusChangedEvent);

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

    public async Task<ResultDto<OrderReadDto>> UpdateOrderType(
        Guid id,
        OrderUpdateTypeDto updateTypeDto
    )
    {
        try
        {
            var order = await orderingContext
                .Orders.Include(o => o.OrderItems)
                .Include(o => o.CustomerInformation)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return ResultDto<OrderReadDto>.Failure("Order not found.", HttpStatusCode.NotFound);

            if (order.Type == updateTypeDto.NewOrderType)
                return ResultDto<OrderReadDto>.Failure(
                    "Order type is already set to the requested type.",
                    HttpStatusCode.BadRequest
                );

            var previousOrderType = order.Type;

            switch (updateTypeDto.NewOrderType)
            {
                case OrderType.DineIn:
                    await HandleDineInTransition(order, updateTypeDto.TableId);
                    break;

                case OrderType.Takeaway:
                    HandleTakeawayTransition(
                        order,
                        updateTypeDto.PhoneNumber!,
                        updateTypeDto.AdditionalInstructions
                    );
                    break;

                case OrderType.Delivery:
                    HandleDeliveryTransition(
                        order,
                        updateTypeDto.PhoneNumber!,
                        updateTypeDto.AdditionalInstructions,
                        updateTypeDto.Address!
                    );
                    break;

                default:
                    return ResultDto<OrderReadDto>.Failure(
                        "Invalid order type.",
                        HttpStatusCode.BadRequest
                    );
            }

            order.Type = updateTypeDto.NewOrderType;
            await orderingContext.SaveChangesAsync();

            if (previousOrderType != order.Type)
            {
                var orderTypeChangeEvent = mapper.Map<OrderTypeChangeEvent>(
                    (order, previousOrderType)
                );
                await eventHandlerService.HandleEventAsync(orderTypeChangeEvent);
            }

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

    public async Task<ResultDto<OrderReadDto>> SplitOrder(Guid id, MoveOrderItemsDto splitOrderDto)
    {
        try
        {
            var originalOrder = await orderingContext
                .Orders.Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.ExtraIngredients)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.RemovedIngredients)
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (originalOrder == null)
                return ResultDto<OrderReadDto>.Failure("Order not found.", HttpStatusCode.NotFound);

            if (originalOrder.Payments.Any())
                return ResultDto<OrderReadDto>.Failure(
                    "Order cannot be split after payments have been made.",
                    HttpStatusCode.BadRequest
                );

            var itemsToSplit = originalOrder
                .OrderItems.Where(oi => splitOrderDto.OrderItemIds.Contains(oi.Id))
                .ToList();

            if (!itemsToSplit.Any())
                return ResultDto<OrderReadDto>.Failure(
                    "No valid items selected for splitting.",
                    HttpStatusCode.BadRequest
                );

            var newOrder = mapper.Map<Order, Order>(originalOrder);

            newOrder.OrderItems = mapper.Map<List<OrderItem>>(itemsToSplit);
            newOrder.Id = Guid.NewGuid();
            newOrder.DateTime = DateTime.UtcNow;

            originalOrder.OrderItems.RemoveAll(oi => itemsToSplit.Contains(oi));

            originalOrder.TotalAmount = Math.Max(
                0,
                originalOrder.OrderItems.Sum(item => item.Price)
            );
            newOrder.TotalAmount = Math.Max(0, newOrder.OrderItems.Sum(item => item.Price));

            await orderingContext.Orders.AddAsync(newOrder);
            await orderingContext.SaveChangesAsync();

            var orderSplitEvent = mapper.Map<OrderSplitEvent>(newOrder);
            await eventHandlerService.HandleEventAsync(orderSplitEvent);

            return ResultDto<OrderReadDto>.Success(null, HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            return ResultDto<OrderReadDto>.Failure(
                $"An error occurred: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ResultDto<OrderReadDto>> JoinOrder(Guid sourceOrderId, Guid targetOrderId)
    {
        try
        {
            var sourceOrder = await orderingContext
                .Orders.Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.ExtraIngredients)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.RemovedIngredients)
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.Id == sourceOrderId);

            var targetOrder = await orderingContext
                .Orders.Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.ExtraIngredients)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.RemovedIngredients)
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.Id == targetOrderId);

            if (sourceOrder == null || targetOrder == null)
                return ResultDto<OrderReadDto>.Failure(
                    "One or both orders not found.",
                    HttpStatusCode.NotFound
                );

            if (sourceOrder.Payments.Any() || targetOrder.Payments.Any())
                return ResultDto<OrderReadDto>.Failure(
                    "Orders cannot be joined after payments have been made.",
                    HttpStatusCode.BadRequest
                );

            targetOrder.OrderItems.AddRange(sourceOrder.OrderItems);

            targetOrder.TotalAmount = Math.Max(0, targetOrder.OrderItems.Sum(item => item.Price));

            orderingContext.Orders.Remove(sourceOrder);
            await orderingContext.SaveChangesAsync();

            var orderJoinEvent = mapper.Map<OrderJoinEvent>(targetOrder);
            await eventHandlerService.HandleEventAsync(orderJoinEvent);

            return ResultDto<OrderReadDto>.Success(null, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<OrderReadDto>.Failure(
                $"An error occurred: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ResultDto<OrderReadDto>> MoveOrderItems(
        Guid sourceOrderId,
        Guid targetOrderId,
        MoveOrderItemsDto moveOrderItemsDto
    )
    {
        try
        {
            var sourceOrder = await orderingContext
                .Orders.Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.ExtraIngredients)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.RemovedIngredients)
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.Id == sourceOrderId);

            var targetOrder = await orderingContext
                .Orders.Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.ExtraIngredients)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.RemovedIngredients)
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.Id == targetOrderId);

            if (sourceOrder == null || targetOrder == null)
                return ResultDto<OrderReadDto>.Failure(
                    "One or both orders not found.",
                    HttpStatusCode.NotFound
                );

            if (sourceOrder.Payments.Any() || targetOrder.Payments.Any())
                return ResultDto<OrderReadDto>.Failure(
                    "Cannot move items after payments have been made.",
                    HttpStatusCode.BadRequest
                );

            var itemsToMove = sourceOrder
                .OrderItems.Where(oi => moveOrderItemsDto.OrderItemIds.Contains(oi.Id))
                .ToList();

            if (!itemsToMove.Any())
                return ResultDto<OrderReadDto>.Failure(
                    "No valid items selected for moving.",
                    HttpStatusCode.BadRequest
                );

            targetOrder.OrderItems.AddRange(itemsToMove);
            sourceOrder.OrderItems.RemoveAll(oi => itemsToMove.Contains(oi));

            sourceOrder.TotalAmount = Math.Max(0, sourceOrder.OrderItems.Sum(item => item.Price));
            targetOrder.TotalAmount = Math.Max(0, targetOrder.OrderItems.Sum(item => item.Price));

            await orderingContext.SaveChangesAsync();

            var orderMoveEvent = mapper.Map<OrderItemsMovedEvent>(
                (sourceOrder, targetOrder.Id, moveOrderItemsDto.OrderItemIds)
            );
            await eventHandlerService.HandleEventAsync(orderMoveEvent);

            return ResultDto<OrderReadDto>.Success(null, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<OrderReadDto>.Failure(
                $"An error occurred: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ResultDto<bool>> DeleteOrder(Guid id)
    {
        try
        {
            var orderToDelete = await orderingContext
                .Orders.Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (orderToDelete == null)
            {
                return ResultDto<bool>.Failure("order not found", HttpStatusCode.NotFound);
            }

            orderingContext.Orders.Remove(orderToDelete);
            await orderingContext.SaveChangesAsync();

            var orderDeletedEvent = new OrderDeletedEvent { OrderId = id };
            await eventHandlerService.HandleEventAsync(orderDeletedEvent);

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

    private async Task<List<OrderItem>> PopulateOrderItemsAsync(
        IEnumerable<OrderItemCreateDto> orderItemDtos,
        Guid orderId
    )
    {
        var menuItemIds = orderItemDtos.Select(oi => oi.MenuItemId).Distinct().ToList();

        var menuItems = await orderingContext
            .MenuItems.Where(mi => menuItemIds.Contains(mi.Id))
            .Include(mi => mi.MenuItemIngredientRels)
            .ThenInclude(rel => rel.Ingredient)
            .ToDictionaryAsync(mi => mi.Id);

        var allIngredientIds = orderItemDtos
            .SelectMany(dto =>
                dto.ExtraIngredients.Select(ei => ei.IngredientId).Concat(dto.RemovedIngredientIds)
            )
            .Distinct()
            .ToList();

        var allIngredients = await orderingContext
            .Ingredients.Where(ing => allIngredientIds.Contains(ing.Id))
            .ToDictionaryAsync(ing => ing.Id);

        var orderItems = new List<OrderItem>();

        foreach (var dto in orderItemDtos)
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

            // Handling Extra Ingredients
            foreach (var extra in dto.ExtraIngredients)
            {
                if (allIngredients.TryGetValue(extra.IngredientId, out var ingredient))
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

            // Handling Removed Ingredients
            foreach (var removedId in dto.RemovedIngredientIds)
            {
                if (allIngredients.TryGetValue(removedId, out var ingredient))
                {
                    orderItem.RemovedIngredients.Add(
                        new OrderItemIngredient
                        {
                            Id = removedId,
                            Name = ingredient.Name,
                            Price = 0,
                            Quantity = 1,
                        }
                    );
                }
            }

            orderItems.Add(orderItem);
        }

        return orderItems;
    }

    private async Task HandleDineInTransition(Order order, Guid? tableId)
    {
        if (tableId == null)
            throw new ArgumentException("TableId is required for Dine-In orders.");

        var table = await orderingContext.Tables.FirstOrDefaultAsync(t => t.Id == tableId);
        if (table == null)
            throw new ArgumentException("Specified table does not exist.");

        if (order.CustomerInformation != null)
        {
            orderingContext.CustomerInformation.Remove(order.CustomerInformation);
            order.CustomerInformation = null;
        }

        order.TableId = tableId;
        table.Status = TableStatus.Ongoing;

        orderingContext.Tables.Update(table);
    }

    private void HandleTakeawayTransition(
        Order order,
        string phoneNumber,
        string? additionalInstructions
    )
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("PhoneNumber is required for Takeaway orders.");

        if (order.TableId.HasValue)
            order.TableId = null;

        UpdateCustomerInformation(order, phoneNumber, additionalInstructions, null);
    }

    private void HandleDeliveryTransition(
        Order order,
        string phoneNumber,
        string? additionalInstructions,
        string address
    )
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Address is required for Delivery orders.");

        if (order.TableId.HasValue)
            order.TableId = null;

        UpdateCustomerInformation(order, phoneNumber, additionalInstructions, address);
    }

    private void UpdateCustomerInformation(
        Order order,
        string phoneNumber,
        string? additionalInstructions,
        string? address
    )
    {
        if (order.CustomerInformation == null)
        {
            order.CustomerInformation = new CustomerInformation
            {
                OrderId = order.Id,
                PhoneNumber = phoneNumber,
                AdditionalInstructions = additionalInstructions,
                Address = address,
                OrderCompletionType = OrderCompletionType.Immediate,
                PreferredPaymentMethod = PreferredPaymentMethod.Cash,
                ExpectedOrderCompletion = null,
            };
            orderingContext.CustomerInformation.Add(order.CustomerInformation);
        }
        else
        {
            order.CustomerInformation.PhoneNumber = phoneNumber;
            order.CustomerInformation.AdditionalInstructions = additionalInstructions;
            order.CustomerInformation.Address = address;
        }
    }

    private decimal RecalculateOrderTotal(Order order)
    {
        var total = order.OrderItems.Sum(oi => oi.Price);

        if (order.Discount > 0)
        {
            total = total * (1 - order.Discount / 100);
        }

        return total;
    }
}

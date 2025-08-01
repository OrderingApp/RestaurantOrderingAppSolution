using System.Net;
using Application.Contracts;
using Application.Dtos.Common;
using Application.Dtos.OrderItems;
using Application.Dtos.Orders;
using Application.Dtos.Orders.OrderDelivery;
using Application.Dtos.Orders.OrderDineIn;
using Application.Dtos.Orders.OrderTakeAway;
using Application.Helpers;
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

            dineInOrder.OrderItems = await OrderCalculationHelper.PopulateOrderItemsAsync(
                orderingContext,
                dineInOrderDto.OrderItems,
                dineInOrder.Id
            );

            dineInOrder.TotalAmount = OrderCalculationHelper.RecalculateOrderTotal(dineInOrder);

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


            takeawayOrder.OrderItems = await OrderCalculationHelper.PopulateOrderItemsAsync(
                orderingContext,
                takeawayOrderDto.OrderItems,
                takeawayOrder.Id
            );

            takeawayOrder.TotalAmount = OrderCalculationHelper.RecalculateOrderTotal(takeawayOrder);

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

            deliveryOrder.OrderItems = await OrderCalculationHelper.PopulateOrderItemsAsync(
                orderingContext,
                deliveryOrderDto.OrderItems,
                deliveryOrder.Id
            );

            deliveryOrder.TotalAmount = OrderCalculationHelper.RecalculateOrderTotal(deliveryOrder);

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
                && o.CreatedAt >= date
                && o.CreatedAt < nextDay
            );

            var closedOrdersQuery = orderingContext
                .Orders.Where(o =>
                    o.Type == orderType
                    && o.Status == OrderStatus.Closed
                    && o.CreatedAt >= date
                    && o.CreatedAt < nextDay
                )
                .OrderByDescending(o => o.CreatedAt)
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
            order.TotalAmount = OrderCalculationHelper.RecalculateOrderTotal(order);

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
                bool hasOtherOpenOrders = await orderingContext.Orders
                    .AnyAsync(o => o.TableId == order.TableId && o.Id != order.Id && o.Status != OrderStatus.Closed);

                if (!hasOtherOpenOrders)
                {
                    order.Table.Status = TableStatus.Available;
                    orderingContext.Tables.Update(order.Table);
                }
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
                bool hasOtherOpenOrders = await orderingContext.Orders
                    .AnyAsync(o => o.TableId == order.TableId && o.Id != order.Id && o.Status != OrderStatus.Closed);

                if (!hasOtherOpenOrders)
                {
                    order.Table.Status = TableStatus.Available;
                    orderingContext.Tables.Update(order.Table);
                }
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

            if (newStatus == OrderStatus.Closed && order.TableId != null)
            {
                var otherOpenOrdersExist = await orderingContext.Orders.AnyAsync(o =>
                    o.TableId == order.TableId &&
                    o.Id != order.Id &&
                    o.Status != OrderStatus.Closed
                );

                if (!otherOpenOrdersExist)
                {
                    order.Table!.Status = TableStatus.Available;
                    orderingContext.Tables.Update(order.Table);
                }
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

            var createdOrders = new List<Order>();

            foreach (var group in splitOrderDto.SplitGroups)
            {
                var itemsToMove = originalOrder.OrderItems
                    .Where(oi => group.OrderItemIds.Contains(oi.Id))
                    .ToList();

                if (!itemsToMove.Any())
                    continue;

                var newOrder = mapper.Map<Order, Order>(originalOrder);
                newOrder.Id = Guid.NewGuid();
                // To fix? datetime shouldnt be set to now
                newOrder.CreatedAt = DateTime.UtcNow;
                newOrder.OrderItems = mapper.Map<List<OrderItem>>(itemsToMove);
                newOrder.TotalAmount = OrderCalculationHelper.RecalculateOrderTotal(newOrder);

                // Remove from original
                originalOrder.OrderItems.RemoveAll(oi => group.OrderItemIds.Contains(oi.Id));

                await orderingContext.Orders.AddAsync(newOrder);
                createdOrders.Add(newOrder);
            }

            // Recalculate original order total
            originalOrder.TotalAmount = OrderCalculationHelper.RecalculateOrderTotal(originalOrder);
            await orderingContext.SaveChangesAsync();

            // Fire events
            foreach (var order in createdOrders)
            {
                var orderSplitEvent = mapper.Map<OrderSplitEvent>(order);
                await eventHandlerService.HandleEventAsync(orderSplitEvent);
            }

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

            targetOrder.TotalAmount = OrderCalculationHelper.RecalculateOrderTotal(targetOrder);

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
        SplitOrderGroupDto moveOrderItemsDto
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

            sourceOrder.TotalAmount = OrderCalculationHelper.RecalculateOrderTotal(sourceOrder);
            targetOrder.TotalAmount = OrderCalculationHelper.RecalculateOrderTotal(targetOrder);

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
}

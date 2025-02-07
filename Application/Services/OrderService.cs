using Application.Contracts;
using Application.Dtos.Common;
using Application.Dtos.OrderItems;
using Application.Dtos.Orders;
using Application.Dtos.Orders.OrderCreate;
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
using System.Net;

namespace Application.Services;

public class OrderService(RestaurantOrderingContext orderingContext, IEventHandlerService eventHandlerService, IMapper mapper) : IOrderService
{
    public async Task<ResultDto<OrderReadDto>> CreateDineInOrder(DineInOrderCreateDto dineInOrderDto)
    {
        try
        {
            var dineInOrder = mapper.Map<Order>(dineInOrderDto);

            var table = await orderingContext.Tables
                .FirstOrDefaultAsync(t => t.Id == dineInOrderDto.TableId);

            if (table == null)
                return ResultDto<OrderReadDto>
                    .Failure("Specified table does not exist.", HttpStatusCode.BadRequest);

            table.IsOccupied = true;

            dineInOrder.OrderItems = await PopulateOrderItemsAsync(dineInOrderDto.OrderItems, dineInOrder.Id);
            dineInOrder.TotalAmount = dineInOrder.OrderItems.Sum(oi => oi.Price);

            var result = await orderingContext.Orders.AddAsync(dineInOrder);
            await orderingContext.SaveChangesAsync();

            var createdOrderDto = mapper.Map<OrderReadDto>(dineInOrder);

            var orderCreatedEvent = mapper.Map<DineInOrderCreatedEvent>(result.Entity);
            await eventHandlerService.HandleEventAsync(orderCreatedEvent);

            return ResultDto<OrderReadDto>
                .Success(createdOrderDto, HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            return ResultDto<OrderReadDto>
                .Failure($"An error occurred: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ResultDto<OrderReadDto>> CreateTakeawayOrder(TakeawayOrderCreateDto takeawayOrderDto)
    {
        try
        {
            var takeawayOrder = mapper.Map<Order>(takeawayOrderDto);

            if(string.IsNullOrWhiteSpace(takeawayOrderDto.PhoneNumber))
                return ResultDto<OrderReadDto>
                    .Failure("Phone number is required for takeaway orders.", HttpStatusCode.BadRequest);

            takeawayOrder.CustomerInformation = mapper.Map<CustomerInformation>(takeawayOrderDto);
            takeawayOrder.CustomerInformation.OrderId = takeawayOrder.Id;

            takeawayOrder.OrderItems = await PopulateOrderItemsAsync(takeawayOrderDto.OrderItems, takeawayOrder.Id);
            takeawayOrder.TotalAmount = takeawayOrder.OrderItems.Sum(oi => oi.Price);

            var result = await orderingContext.Orders.AddAsync(takeawayOrder);
            await orderingContext.SaveChangesAsync();

            var orderReadDto = mapper.Map<OrderReadDto>(takeawayOrder);

            var orderCreatedEvent = mapper.Map<TakeawayOrderCreatedEvent>(result.Entity);
            await eventHandlerService.HandleEventAsync(orderCreatedEvent);

            return ResultDto<OrderReadDto>
                .Success(orderReadDto, HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            return ResultDto<OrderReadDto>
                .Failure($"An error occurred: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ResultDto<OrderReadDto>> CreateDeliveryOrder(DeliveryOrderCreateDto deliveryOrderDto)
    {
        try
        {
            var deliveryOrder = mapper.Map<Order>(deliveryOrderDto);

            deliveryOrder.CustomerInformation = mapper.Map<CustomerInformation>(deliveryOrderDto);
            deliveryOrder.CustomerInformation.OrderId = deliveryOrder.Id;

            deliveryOrder.OrderItems = await PopulateOrderItemsAsync(deliveryOrderDto.OrderItems, deliveryOrder.Id);
            deliveryOrder.TotalAmount = deliveryOrder.OrderItems.Sum(oi => oi.Price);

            var result = await orderingContext.Orders.AddAsync(deliveryOrder);
            await orderingContext.SaveChangesAsync();

            var orderReadDto = mapper.Map<OrderReadDto>(deliveryOrder);

            var orderCreatedEvent = mapper.Map<DeliveryOrderCreatedEvent>(result.Entity);
            await eventHandlerService.HandleEventAsync(orderCreatedEvent);

            return ResultDto<OrderReadDto>
                .Success(orderReadDto, HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            return ResultDto<OrderReadDto>
                .Failure($"An error occurred: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ResultDto<OrderReadDto>> SplitOrder(SplitOrderDto splitOrderDto, Guid orderId)
    {
        try
        {
            var originalOrder = await orderingContext.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.OrderItemIngredients)
                        .ThenInclude(oii => oii.Ingredient)
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (originalOrder == null)
                return ResultDto<OrderReadDto>.Failure("Order not found.", HttpStatusCode.NotFound);

            if (originalOrder.Payments.Any(p => p.PaymentStatus == PaymentStatus.Paid))
                return ResultDto<OrderReadDto>.Failure("Cannot split an order that has payments.", HttpStatusCode.BadRequest);

            var itemsToSplit = originalOrder.OrderItems
                .Where(oi => splitOrderDto.OrderItemIds.Contains(oi.Id))
                .ToList();

            if (!itemsToSplit.Any())
                return ResultDto<OrderReadDto>.Failure("No valid items selected for splitting.", HttpStatusCode.BadRequest);

            var newOrder = new Order
            {
                Id = Guid.NewGuid(),
                OrderDateTime = DateTime.UtcNow,
                OrderStatus = originalOrder.OrderStatus,
                TableId = originalOrder.TableId,
                OrderType = originalOrder.OrderType,
                OrderItems = mapper.Map<List<OrderItem>>(itemsToSplit)
            };

            originalOrder.OrderItems.RemoveAll(oi => itemsToSplit.Contains(oi));

            originalOrder.TotalAmount = Math.Max(0, originalOrder.OrderItems.Sum(item => item.Price));
            newOrder.TotalAmount = Math.Max(0, newOrder.OrderItems.Sum(item => item.Price));

            orderingContext.Orders.Add(newOrder);
            await orderingContext.SaveChangesAsync();

            var orderSplitEvent = mapper.Map<OrderSplitBillEvent>(newOrder);
            await eventHandlerService.HandleEventAsync(orderSplitEvent);

            return ResultDto<OrderReadDto>.Success(null, HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            return ResultDto<OrderReadDto>.Failure($"An error occurred: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ResultDto<OrderReadDto>> GetOrder(Guid id)
    {
        try
        {
            var order = await orderingContext.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.OrderItemIngredients)
                    .ThenInclude(oii => oii.Ingredient)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return ResultDto<OrderReadDto>
                    .Failure("Order not found", HttpStatusCode.NotFound);

            var orderDto = mapper.Map<OrderReadDto>(order);

            return ResultDto<OrderReadDto>
                .Success(orderDto, HttpStatusCode.OK);

        }
        catch (Exception ex)
        {
            return ResultDto<OrderReadDto>
                .Failure($"An error occurred: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ResultDto<List<OrderReadDto>>> GetAllOrders(OrderStatus? orderStatus)
    {
        try
        {
            var query = orderingContext.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.CustomerInformation)
                .AsQueryable();

            if (orderStatus.HasValue)
            {
                query = query.Where(o => o.OrderStatus == orderStatus.Value);
            }

            var orders = await query.ToListAsync();

            var orderDtos = mapper.Map<List<OrderReadDto>>(orders);

            return ResultDto<List<OrderReadDto>>
                .Success(orderDtos, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<List<OrderReadDto>>
                .Failure($"An error occurred: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ResultDto<List<OrderReadDto>>> GetOngoingOrdersByType(OrderType orderType)
    {
        try
        {
            var orders = await orderingContext.Orders
                .Where(o => o.OrderType == orderType && o.OrderStatus == OrderStatus.Ongoing)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.OrderItemIngredients)
                        .ThenInclude(oii => oii.Ingredient)
                .Include(o => o.CustomerInformation)
                .ToListAsync();

            var orderDtos = mapper.Map<List<OrderReadDto>>(orders);

            return ResultDto<List<OrderReadDto>>
                .Success(orderDtos, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<List<OrderReadDto>>
                .Failure($"An error occurred: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ResultDto<List<OrderReadDto>>> GetOngoingOrdersForTable(Guid tableId)
    {
        try
        {
            var ongoingOrders = await orderingContext.Orders
                .Where(o => o.TableId == tableId && o.OrderStatus == OrderStatus.Ongoing)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.CustomerInformation)
                .ToListAsync();

            if (!ongoingOrders.Any())
                return ResultDto<List<OrderReadDto>>
                    .Failure("No ongoing orders found for this table.", HttpStatusCode.NotFound);

            var orderDtos = mapper.Map<List<OrderReadDto>>(ongoingOrders);
            return ResultDto<List<OrderReadDto>>
                .Success(orderDtos, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<List<OrderReadDto>>
                .Failure($"An error occurred: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ResultDto<OrderReadDto>> ApplyOrderDiscount(decimal discountPercentage, Guid orderId)
    {
        try
        {
            var order = await orderingContext.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return ResultDto<OrderReadDto>.Failure("Order not found.", HttpStatusCode.NotFound);

            if (discountPercentage < 0 || discountPercentage > 100)
                return ResultDto<OrderReadDto>.Failure("Invalid discount percentage.", HttpStatusCode.BadRequest);

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
            return ResultDto<OrderReadDto>.Failure($"An error occurred: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ResultDto<OrderReadDto>> ChangeOrderTable(Guid orderId, Guid newTableId)
    {
        try
        {
            var order = await orderingContext.Orders
                .Include(o => o.Table)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return ResultDto<OrderReadDto>
                    .Failure("Order not found.", HttpStatusCode.NotFound);

            if (order.TableId == null)
                return ResultDto<OrderReadDto>
                    .Failure("This order is not associated with a table.", HttpStatusCode.BadRequest);

            var newTable = await orderingContext.Tables.FirstOrDefaultAsync(t => t.Id == newTableId);

            if (newTable == null)
                return ResultDto<OrderReadDto>
                    .Failure("Specified table does not exist.", HttpStatusCode.BadRequest);

            if (order.TableId.HasValue)
            {
                var currentTable = await orderingContext.Tables.FirstOrDefaultAsync(t => t.Id == order.TableId);
                if (currentTable != null)
                {
                    currentTable.IsOccupied = false;
                    orderingContext.Tables.Update(currentTable);
                }
            }

            newTable.IsOccupied = true;
            order.TableId = newTableId;

            orderingContext.Orders.Update(order);
            orderingContext.Tables.Update(newTable);

            await orderingContext.SaveChangesAsync();

            var updatedOrderDto = mapper.Map<OrderReadDto>(order);

            var orderTableChangeEvent = mapper.Map<OrderTableChangedEvent>(order);
            await eventHandlerService.HandleEventAsync(orderTableChangeEvent);

            return ResultDto<OrderReadDto>
                .Success(updatedOrderDto, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<OrderReadDto>
                .Failure($"An error occurred: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ResultDto<OrderReadDto>> UpdateOrderStatus(OrderStatus newStatus, Guid id)
    {
        try
        {
            var order = await orderingContext.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return ResultDto<OrderReadDto>
                    .Failure("Order not found", HttpStatusCode.NotFound);

            var previousOrderStatus = order.OrderStatus;

            order.OrderStatus = newStatus;
            await orderingContext.SaveChangesAsync();

            var orderStatusChangedEvent = mapper.Map<OrderStatusChangedEvent>((order, previousOrderStatus));
            await eventHandlerService.HandleEventAsync(orderStatusChangedEvent);

            var updatedOrderDto = mapper.Map<OrderReadDto>(order);
            return ResultDto<OrderReadDto>
                .Success(updatedOrderDto, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<OrderReadDto>
                .Failure($"An error occurred: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ResultDto<OrderReadDto>> UpdateOrderType(OrderType newOrderType, OrderUpdateTypeDto updateTypeDto, Guid orderId)
    {
        try
        {
            var order = await orderingContext.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.CustomerInformation)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return ResultDto<OrderReadDto>.Failure("Order not found.", HttpStatusCode.NotFound);

            if (order.OrderType == newOrderType)
                return ResultDto<OrderReadDto>.Failure("Order type is already set to the requested type.", HttpStatusCode.BadRequest);

            var previousOrderType = order.OrderType;

            switch (newOrderType)
            {
                case OrderType.DineIn:
                    await HandleDineInTransition(order, updateTypeDto.TableId);
                    break;

                case OrderType.Takeaway:
                    HandleTakeawayTransition(order, updateTypeDto.PhoneNumber!, updateTypeDto.AdditionalInstructions);
                    break;

                case OrderType.Delivery:
                    HandleDeliveryTransition(order, updateTypeDto.PhoneNumber!, updateTypeDto.AdditionalInstructions, updateTypeDto.Address!);
                    break;

                default:
                    return ResultDto<OrderReadDto>.Failure("Invalid order type.", HttpStatusCode.BadRequest);
            }

            order.OrderType = newOrderType;
            await orderingContext.SaveChangesAsync();

            var orderTypeChangeEvent = mapper.Map<OrderTypeChangeEvent>((order, previousOrderType));
            await eventHandlerService.HandleEventAsync(orderTypeChangeEvent);

            var updatedOrderDto = mapper.Map<OrderReadDto>(order);
            return ResultDto<OrderReadDto>.Success(updatedOrderDto, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<OrderReadDto>.Failure($"An error occurred: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ResultDto<bool>> DeleteOrder(Guid id)
    {
        try
        {
            var orderToDelete = await orderingContext.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.Id == id);

            if (orderToDelete == null)
            {
                return ResultDto<bool>
                    .Failure("order not found", HttpStatusCode.NotFound);
            }

            var result = orderingContext.Orders.Remove(orderToDelete);
            await orderingContext.SaveChangesAsync();

            var orderDeletedEvent = mapper.Map<OrderDeletedEvent>(result);
            await eventHandlerService.HandleEventAsync(orderDeletedEvent);

            return ResultDto<bool>
                .Success(true, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<bool>
                .Failure($"An error occurred: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    private async Task<List<OrderItem>> PopulateOrderItemsAsync(IEnumerable<OrderItemCreateDto> orderItemDtos, Guid orderId)
    {
        var menuItemIds = orderItemDtos
            .Select(oi => oi.MenuItemId)
            .Distinct();
        var menuItems = await orderingContext.MenuItems
            .Where(mi => menuItemIds.Contains(mi.Id))
            .ToDictionaryAsync(mi => mi.Id);

        var ingredientIds = orderItemDtos
            .SelectMany(oi => oi.Ingredients.Select(i => i.IngredientId))
            .Distinct();
        var ingredients = await orderingContext.Ingredients
            .Where(ing => ingredientIds.Contains(ing.Id))
            .ToDictionaryAsync(ing => ing.Id);

        var orderItems = new List<OrderItem>();
        foreach (var itemDto in orderItemDtos)
        {
            if (!menuItems.TryGetValue(itemDto.MenuItemId, out var menuItem))
            {
                throw new KeyNotFoundException($"MenuItem with ID {itemDto.MenuItemId} not found.");
            }

            var orderItem = mapper.Map<OrderItem>(itemDto);
            orderItem.Price = menuItem.Price;
            orderItem.OrderId = orderId;

            foreach (var ingredientDto in itemDto.Ingredients)
            {
                if (!ingredients.TryGetValue(ingredientDto.IngredientId, out var ingredient))
                {
                    throw new KeyNotFoundException($"Ingredient with ID {ingredientDto.IngredientId} not found.");
                }

                var orderItemIngredient = new OrderItemIngredient
                {
                    IngredientId = ingredient.Id,
                    Quantity = ingredientDto.Quantity,
                    OrderItemId = orderItem.Id
                };

                orderItem.OrderItemIngredients.Add(orderItemIngredient);
                orderItem.Price += ingredient.Price * ingredientDto.Quantity;
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

        table.IsOccupied = true;

        if (order.CustomerInformation != null)
        {
            orderingContext.CustomerInformations.Remove(order.CustomerInformation);
            order.CustomerInformation = null;
        }

        if (order.TableId.HasValue)
            await ClearTableOccupancy(order.TableId.Value);

        order.TableId = tableId;
        orderingContext.Tables.Update(table);
    }

    private void HandleTakeawayTransition(Order order, string phoneNumber, string? additionalInstructions)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("PhoneNumber is required for Takeaway orders.");

        if (order.TableId.HasValue)
            order.TableId = null;

        if (order.CustomerInformation == null)
        {
            order.CustomerInformation = new CustomerInformation
            {
                PhoneNumber = phoneNumber,
                AdditionalInstructions = additionalInstructions
            };
            orderingContext.CustomerInformations.Add(order.CustomerInformation);
        }
        else
        {
            order.CustomerInformation.PhoneNumber = phoneNumber;
            order.CustomerInformation.AdditionalInstructions = additionalInstructions;
            order.CustomerInformation.Address = null;
            orderingContext.CustomerInformations.Update(order.CustomerInformation);
        }
    }

    private void HandleDeliveryTransition(Order order, string phoneNumber, string? additionalInstructions, string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Address is required for Delivery orders.");

        if (order.TableId.HasValue)
            order.TableId = null;

        if (order.CustomerInformation == null)
        {
            order.CustomerInformation = new CustomerInformation
            {
                PhoneNumber = phoneNumber,
                AdditionalInstructions = additionalInstructions,
                Address = address
            };
            orderingContext.CustomerInformations.Add(order.CustomerInformation);
        }
        else
        {
            order.CustomerInformation.PhoneNumber = phoneNumber;
            order.CustomerInformation.AdditionalInstructions = additionalInstructions;
            order.CustomerInformation.Address = address;
            orderingContext.CustomerInformations.Update(order.CustomerInformation);
        }
    }

    private async Task ClearTableOccupancy(Guid tableId)
    {
        var table = await orderingContext.Tables.FirstOrDefaultAsync(t => t.Id == tableId);
        if (table != null)
        {
            table.IsOccupied = false;
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
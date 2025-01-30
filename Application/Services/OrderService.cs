using Application.Contracts;
using Application.Dtos.Common;
using Application.Dtos.OrderItems;
using Application.Dtos.Orders;
using Application.Dtos.Orders.OrderCreate;
using AutoMapper;
using Domain;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Application.Services;

public class OrderService(RestaurantOrderingContext orderingContext, IMapper mapper) : IOrderService
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

            if (table.IsOccupied)
                return ResultDto<OrderReadDto>
                    .Failure("The specified table is already occupied.", HttpStatusCode.Conflict);

            table.IsOccupied = true;


            dineInOrder.OrderItems = await PopulateOrderItemsAsync(dineInOrderDto.OrderItems, dineInOrder.Id);
            dineInOrder.TotalAmount = dineInOrder.OrderItems.Sum(oi => oi.Price * oi.Quantity);

            await orderingContext.Orders.AddAsync(dineInOrder);
            await orderingContext.SaveChangesAsync();

            var createdOrderDto = mapper.Map<OrderReadDto>(dineInOrder);

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
            takeawayOrder.TotalAmount = takeawayOrder.OrderItems.Sum(oi => oi.Price * oi.Quantity);

            await orderingContext.Orders.AddAsync(takeawayOrder);
            await orderingContext.SaveChangesAsync();

            var orderReadDto = mapper.Map<OrderReadDto>(takeawayOrder);

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
            deliveryOrder.TotalAmount = deliveryOrder.OrderItems.Sum(oi => oi.Price * oi.Quantity);

            await orderingContext.Orders.AddAsync(deliveryOrder);
            await orderingContext.SaveChangesAsync();

            var orderReadDto = mapper.Map<OrderReadDto>(deliveryOrder);

            //var orderCreatedEvent = new DeliveryOrderCreatedEvent
            //{
            //    AdditionalInstructions = deliveryOrderDto.AdditionalInstructions,
            //};

            //await eventsDatabaseContext.EventContexts.AddAsync(new EventContext
            //{
            //    CorrelationId = Guid.NewGuid(),
            //    DateTime = DateTime.Now,
            //    EventType = nameof(DeliveryOrderCreatedEvent),
            //    Id = Guid.NewGuid(),
            //    PayloadJson = JsonSerializer.Serialize(orderCreatedEvent)
            //});

            return ResultDto<OrderReadDto>
                .Success(orderReadDto, HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            return ResultDto<OrderReadDto>
                .Failure($"An error occurred: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ResultDto<OrderReadDto>> PayOrder(PaymentMethod paymentMethod, Guid orderId)
    {
        try
        {
            var order = await orderingContext.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return ResultDto<OrderReadDto>.Failure("Order not found.", HttpStatusCode.NotFound);

            if (order.PaymentStatus == PaymentStatus.Paid)
                return ResultDto<OrderReadDto>.Failure("Order is already fully paid.", HttpStatusCode.BadRequest);

            order.PaymentStatus = PaymentStatus.Paid;
            await orderingContext.SaveChangesAsync();

            var updatedOrder = mapper.Map<OrderReadDto>(order);
            return ResultDto<OrderReadDto>.Success(updatedOrder, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<OrderReadDto>.Failure($"An error occurred: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ResultDto<OrderReadDto>> SplitBill(SplitBillDto splitBillDto, Guid orderId)
    {
        try
        {
            var originalOrder = await orderingContext.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.OrderItemIngredients)
                        .ThenInclude(oii => oii.Ingredient)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (originalOrder == null)
                return ResultDto<OrderReadDto>.Failure("Order not found.", HttpStatusCode.NotFound);

            if (originalOrder.PaymentStatus == PaymentStatus.Paid || originalOrder.PaymentStatus == PaymentStatus.Cancelled)
                return ResultDto<OrderReadDto>.Failure("Cannot split a fully paid or canceled order.", HttpStatusCode.BadRequest);

            var itemsToSplit = originalOrder.OrderItems
                .Where(oi => splitBillDto.OrderItemIds.Contains(oi.Id))
                .ToList();

            if (!itemsToSplit.Any())
                return ResultDto<OrderReadDto>.Failure("No valid items selected for splitting.", HttpStatusCode.BadRequest);

            var newOrder = new Order
            {
                Id = Guid.NewGuid(),
                OrderDateTime = DateTime.UtcNow,
                OrderStatus = originalOrder.OrderStatus,
                PaymentStatus = PaymentStatus.Pending,
                TableId = originalOrder.TableId,
                OrderType = originalOrder.OrderType,
                OrderItems = new List<OrderItem>()
            };

            foreach (var item in itemsToSplit)
            {
                var newItem = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    MenuItemId = item.MenuItemId,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    SpecialInstructions = item.SpecialInstructions,
                    OrderItemIngredients = item.OrderItemIngredients.Select(ingredient => new OrderItemIngredient
                    {
                        IngredientId = ingredient.IngredientId,
                        Quantity = ingredient.Quantity
                    }).ToList()
                };

                newOrder.OrderItems.Add(newItem);

                originalOrder.OrderItems.Remove(item);
            }

            originalOrder.TotalAmount = originalOrder.OrderItems.Sum(item => item.Price * item.Quantity);

            newOrder.TotalAmount = newOrder.OrderItems.Sum(item => item.Price * item.Quantity);

            await orderingContext.Orders.AddAsync(newOrder);
            await orderingContext.SaveChangesAsync();


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

    public async Task<ResultDto<List<OrderReadDto>>> GetAllOrders(OrderStatus? orderStatus, PaymentStatus? paymentStatus)
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

            if (paymentStatus.HasValue)
            {
                query = query.Where(o => o.PaymentStatus == paymentStatus.Value);
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
            return ResultDto<OrderReadDto>.Success(updatedOrder, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<OrderReadDto>.Failure($"An error occurred: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ResultDto<OrderReadDto>> ApplyOrderItemDiscount(decimal discountPercentage, Guid orderId, Guid orderItemId)
    {
        try
        {
            var order = await orderingContext.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return ResultDto<OrderReadDto>.Failure("Order not found.", HttpStatusCode.NotFound);

            var item = order.OrderItems.FirstOrDefault(oi => oi.Id == orderItemId);
            if (item == null)
                return ResultDto<OrderReadDto>.Failure("Order item not found.", HttpStatusCode.NotFound);

            if (discountPercentage < 0 || discountPercentage > 100)
                return ResultDto<OrderReadDto>.Failure("Invalid discount percentage.", HttpStatusCode.BadRequest);

            item.Discount = discountPercentage;

            item.Price = item.Price * (1 - discountPercentage / 100);

            order.TotalAmount = RecalculateOrderTotal(order);

            await orderingContext.SaveChangesAsync();

            var updatedOrder = mapper.Map<OrderReadDto>(order);
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

            if (newTable.IsOccupied)
                return ResultDto<OrderReadDto>
                    .Failure("The specified table is already occupied.", HttpStatusCode.Conflict);

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

            order.OrderStatus = newStatus;
            await orderingContext.SaveChangesAsync();

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

            orderingContext.Orders.Remove(orderToDelete);
            await orderingContext.SaveChangesAsync();

            return ResultDto<bool>
                .Success(true, HttpStatusCode.NoContent);
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

        if (table.IsOccupied)
            throw new InvalidOperationException("Specified table is already occupied.");

        table.IsOccupied = true;

        if (order.CustomerInformation != null)
        {
            orderingContext.CustomerInformation.Remove(order.CustomerInformation);
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
            orderingContext.CustomerInformation.Add(order.CustomerInformation);
        }
        else
        {
            order.CustomerInformation.PhoneNumber = phoneNumber;
            order.CustomerInformation.AdditionalInstructions = additionalInstructions;
            order.CustomerInformation.Address = null;
            orderingContext.CustomerInformation.Update(order.CustomerInformation);
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
            orderingContext.CustomerInformation.Add(order.CustomerInformation);
        }
        else
        {
            order.CustomerInformation.PhoneNumber = phoneNumber;
            order.CustomerInformation.AdditionalInstructions = additionalInstructions;
            order.CustomerInformation.Address = address;
            orderingContext.CustomerInformation.Update(order.CustomerInformation);
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
        var total = order.OrderItems.Sum(oi => oi.Price * oi.Quantity);

        if (order.Discount > 0)
        {
            total = total * (1 - order.Discount / 100);
        }

        return total;
    }
}
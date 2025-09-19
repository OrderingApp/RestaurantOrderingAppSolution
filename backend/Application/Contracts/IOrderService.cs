using Application.Dtos.Common;
using Application.Dtos.Orders;
using Application.Dtos.Orders.OrderDelivery;
using Application.Dtos.Orders.OrderDineIn;
using Application.Dtos.Orders.OrderTakeAway;
using Domain;

namespace Application.Contracts;

public interface IOrderService
{
    // Create orders
    Task<ResultDto<OrderReadDto>> CreateDineInOrder(DineInOrderCreateDto dineInOrderDto);
    Task<ResultDto<OrderReadDto>> CreateTakeawayOrder(TakeawayOrderCreateDto takeawayOrderDto);
    Task<ResultDto<OrderReadDto>> CreateDeliveryOrder(DeliveryOrderCreateDto deliveryOrderDto);

    // Get Orders
    Task<ResultDto<OrderReadDto>> GetOrder(Guid id);
    Task<ResultDto<List<OrderReadDto>>> GetOrders(OrderStatus? orderStatus);
    Task<ResultDto<List<NonDineInOrderSummaryDto>>> GetOngoingAndClosedNonDineInOrders(
        OrderType orderType,
        DateTime? date = null
    );
    Task<ResultDto<List<OrderSummaryDto>>> GetOngoingOrdersForTable(Guid tableId);

    // Update Orders
    Task<ResultDto<OrderReadDto>> ApplyOrderDiscount(Guid id, decimal discountPercentage);
    Task<ResultDto<OrderReadDto>> ChangeOrderTable(Guid id, Guid newTableId);
    Task<ResultDto<OrderReadDto>> CloseOrder(Guid id);
    Task<ResultDto<OrderReadDto>> UpdateOrderStatus(Guid id, OrderStatus newStatus);
    Task<ResultDto<OrderReadDto>> UpdateOrderType(Guid id, OrderUpdateTypeDto updateTypeDto);

    // Split/Join Order
    Task<ResultDto<OrderReadDto>> SplitOrder(Guid id, MoveOrderItemsDto splitOrderDto);
    Task<ResultDto<OrderReadDto>> JoinOrder(Guid sourceOrderId, Guid targetOrderId);
    Task<ResultDto<OrderReadDto>> MoveOrderItems(
        Guid sourceOrderId,
        Guid targetOrderId,
        SplitOrderGroupDto moveOrderItemsDto
    );

    // Delete Orders
    Task<ResultDto<bool>> DeleteOrder(Guid id);
}

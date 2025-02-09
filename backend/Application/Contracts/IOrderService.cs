using Application.Dtos.Common;
using Application.Dtos.Orders;
using Application.Dtos.Orders.OrderDelivery;
using Application.Dtos.Orders.OrderDineIn;
using Application.Dtos.Orders.OrderTakeAway;
using Domain;

namespace Application.Contracts;

public interface IOrderService
{
    Task<ResultDto<OrderReadDto>> CreateDineInOrder(DineInOrderCreateDto dineInOrderDto);
    Task<ResultDto<OrderReadDto>> CreateTakeawayOrder(TakeawayOrderCreateDto takeawayOrderDto);
    Task<ResultDto<OrderReadDto>> CreateDeliveryOrder(DeliveryOrderCreateDto deliveryOrderDto);
    Task<ResultDto<OrderReadDto>> SplitOrder(SplitOrderDto splitOrderDto, Guid orderId);
    Task<ResultDto<OrderReadDto>> GetOrder(Guid id);
    Task<ResultDto<List<OrderReadDto>>> GetAllOrders(OrderStatus? orderStatus);
    Task<ResultDto<List<TakeawayOrderSummaryReadDto>>> GetOngoingOrdersForTakeaway();
    Task<ResultDto<List<DeliveryOrderSummaryReadDto>>> GetOngoingOrdersForDelivery();
    Task<ResultDto<List<OrderReadDto>>> GetOngoingOrdersForTable(Guid tableId);
    Task<ResultDto<OrderReadDto>> ApplyOrderDiscount(decimal discountPercentage, Guid orderId);
    Task<ResultDto<OrderReadDto>> ChangeOrderTable(Guid orderId, Guid newTableId);
    Task<ResultDto<OrderReadDto>> UpdateOrderStatus(OrderStatus newStatus, Guid id);
    Task<ResultDto<OrderReadDto>> UpdateOrderType(OrderType newOrderType, OrderUpdateTypeDto updateTypeDto, Guid orderId);
    Task<ResultDto<bool>> DeleteOrder(Guid id);
}
using Application.Dtos.Common;
using Application.Dtos.OrderItems;
using Application.Dtos.Orders;
using Application.Dtos.Orders.OrderCreate;
using Domain;

namespace Application.Contracts;

public interface IOrderService
{
    Task<ResultDto<OrderReadDto>> CreateDineInOrder(DineInOrderCreateDto dineInOrderDto);
    Task<ResultDto<OrderReadDto>> CreateTakeawayOrder(TakeawayOrderCreateDto takeawayOrderDto);
    Task<ResultDto<OrderReadDto>> CreateDeliveryOrder(DeliveryOrderCreateDto deliveryOrderDto);
    //Task<ResultDto<OrderReadDto>> PayOrder(PaymentMethod paymentMethod, Guid orderId);
    Task<ResultDto<OrderReadDto>> SplitBill(SplitBillDto splitBillDto, Guid orderId);
    Task<ResultDto<OrderReadDto>> GetOrder(Guid id);
    Task<ResultDto<List<OrderReadDto>>> GetAllOrders(OrderStatus? orderStatus);
    Task<ResultDto<List<OrderReadDto>>> GetOngoingOrdersByType(OrderType orderType);
    Task<ResultDto<List<OrderReadDto>>> GetOngoingOrdersForTable(Guid tableId);
    Task<ResultDto<OrderReadDto>> ApplyOrderDiscount(decimal discountPercentage, Guid orderId);
    Task<ResultDto<OrderReadDto>> ChangeOrderTable(Guid orderId, Guid newTableId);
    Task<ResultDto<OrderReadDto>> UpdateOrderStatus(OrderStatus newStatus, Guid id);
    Task<ResultDto<OrderReadDto>> UpdateOrderType(OrderType newOrderType, OrderUpdateTypeDto updateTypeDto, Guid orderId);
    Task<ResultDto<bool>> DeleteOrder(Guid id);
}
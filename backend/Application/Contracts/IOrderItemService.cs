using Application.Dtos.Common;
using Application.Dtos.OrderItems;
using Application.Dtos.Orders;
using Domain;

namespace Application.Contracts;

public interface IOrderItemService
{
    Task<ResultDto<OrderReadDto>> AddOrderItemsToOrder(
        Guid orderId,
        List<OrderItemCreateDto> orderItemDtos
    );
    Task<ResultDto<OrderItemReadDto>> GetOrderItem(Guid orderId, Guid id);
    Task<ResultDto<List<OrderItemsListDto>>> GetOrderItems(Guid orderId);
    Task<ResultDto<OrderItemReadDto>> UpdateOrderItem(
        Guid orderId,
        Guid id,
        OrderItemUpdateDto updateDto
    );
    Task<ResultDto<bool>> UpdateOrderItemStatus(Guid orderId, Guid id, OrderItemStatus status);
    Task<ResultDto<bool>> DeleteOrderItem(Guid orderId, Guid id);
}

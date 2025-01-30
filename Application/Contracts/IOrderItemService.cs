using Application.Dtos.Common;
using Application.Dtos.OrderItems;
using Application.Dtos.Orders;

namespace Application.Contracts;

public interface IOrderItemService
{
    Task<ResultDto<OrderReadDto>> AddOrderItem(OrderItemCreateDto orderItemDto, Guid orderId);
    Task<ResultDto<OrderReadDto>> AddOrderItems(IEnumerable<OrderItemCreateDto> orderItemDtos, Guid orderId);
    Task<ResultDto<OrderItemReadDto>> GetOrderItem(Guid id);
    Task<ResultDto<List<OrderItemsListDto>>> GetAllOrderItems(Guid orderId);
    Task<ResultDto<OrderReadDto>> ApplyOrderItemDiscount(decimal discountPercentage, Guid orderId, Guid orderItemId);
    Task<ResultDto<OrderItemReadDto>> UpdateOrderItem(OrderItemUpdateDto updateDto, Guid orderItemId, Guid orderId);
    Task<ResultDto<bool>> DeleteOrderItem(Guid id);
}

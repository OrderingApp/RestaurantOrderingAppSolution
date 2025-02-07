using Application.Contracts;
using Application.Dtos.OrderItems;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class OrderItemController(IOrderItemService orderItemService) : BaseApiController
{
    [HttpPost("{orderId}/items")]
    public async Task<IActionResult> AddOrderItems([FromBody] IEnumerable<OrderItemCreateDto> orderItemDtos, Guid orderId) =>
        HandleResult(await orderItemService.AddOrderItems(orderItemDtos, orderId));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderItem(Guid id) =>
        HandleResult(await orderItemService.GetOrderItem(id));

    [HttpGet("{orderId}/order-items")]
    public async Task<IActionResult> GetAllOrderItems(Guid orderId) =>
        HandleResult(await orderItemService.GetAllOrderItems(orderId));

    [HttpPut("{orderId}/order-items/{orderItemId}/apply-discount")]
    public async Task<IActionResult> ApplyOrderItemDiscount(decimal discount, Guid orderId, Guid orderItemId) =>
        HandleResult(await orderItemService.ApplyOrderItemDiscount(discount, orderId, orderItemId));

    [HttpPut("{orderId}/items/{orderItemId}")]
    public async Task<IActionResult> UpdateOrderItem([FromBody] OrderItemUpdateDto updateDto, Guid orderItemId, Guid orderId) =>
        HandleResult(await orderItemService.UpdateOrderItem(updateDto, orderItemId, orderId));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrderItem(Guid id) =>
        HandleResult(await orderItemService.DeleteOrderItem(id));
}

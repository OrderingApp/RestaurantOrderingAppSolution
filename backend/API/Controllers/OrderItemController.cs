using Application.Contracts;
using Application.Dtos.OrderItems;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Manages order items within an order.
/// </summary>
public class OrderItemController(IOrderItemService orderItemService) : BaseApiController
{
    /// <summary>
    /// Adds items to an existing order.
    /// </summary>
    /// <param name="orderId">The ID of the order to which items will be added.</param>
    /// <param name="orderItemDtos">A list of order items to add.</param>
    /// <returns>The updated order with added items.</returns>
    /// <response code="201">If the order items were successfully added.</response>
    /// <response code="400">If the request is invalid.</response>
    [HttpPost("{orderId}/items")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> AddOrderItems([FromBody] IEnumerable<OrderItemCreateDto> orderItemDtos, Guid orderId) =>
        HandleResult(await orderItemService.AddOrderItems(orderItemDtos, orderId));

    /// <summary>
    /// Retrieves a specific order item by ID.
    /// </summary>
    /// <param name="id">The unique order item ID.</param>
    /// <returns>The requested order item.</returns>
    /// <response code="200">Returns the order item.</response>
    /// <response code="404">If the order item is not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetOrderItem(Guid id) =>
        HandleResult(await orderItemService.GetOrderItem(id));

    /// <summary>
    /// Retrieves all items in a specific order.
    /// </summary>
    /// <param name="orderId">The ID of the order.</param>
    /// <returns>A list of order items.</returns>
    /// <response code="200">Returns the list of order items.</response>
    [HttpGet("{orderId}/order-items")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetAllOrderItems(Guid orderId) =>
        HandleResult(await orderItemService.GetAllOrderItems(orderId));

    /// <summary>
    /// Applies a discount to a specific order item.
    /// </summary>
    /// <param name="orderId">The ID of the order containing the item.</param>
    /// <param name="orderItemId">The ID of the order item.</param>
    /// <param name="discount">The discount percentage to apply.</param>
    /// <returns>The updated order item with the discount applied.</returns>
    /// <response code="200">If the discount was applied successfully.</response>
    /// <response code="400">If the request is invalid.</response>
    [HttpPut("{orderId}/order-items/{orderItemId}/apply-discount")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> ApplyOrderItemDiscount(decimal discount, Guid orderId, Guid orderItemId) =>
        HandleResult(await orderItemService.ApplyOrderItemDiscount(discount, orderId, orderItemId));

    /// <summary>
    /// Updates an order item.
    /// </summary>
    /// <param name="orderId">The ID of the order containing the item.</param>
    /// <param name="orderItemId">The ID of the order item to update.</param>
    /// <param name="updateDto">The updated order item details.</param>
    /// <returns>The updated order item.</returns>
    /// <response code="200">If the update was successful.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="404">If the order item is not found.</response>
    [HttpPut("{orderId}/items/{orderItemId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateOrderItem([FromBody] OrderItemUpdateDto updateDto, Guid orderItemId, Guid orderId) =>
        HandleResult(await orderItemService.UpdateOrderItem(updateDto, orderItemId, orderId));

    /// <summary>
    /// Deletes an order item.
    /// </summary>
    /// <param name="id">The ID of the order item to delete.</param>
    /// <returns>No content.</returns>
    /// <response code="204">If the deletion was successful.</response>
    /// <response code="404">If the order item is not found.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteOrderItem(Guid id) =>
        HandleResult(await orderItemService.DeleteOrderItem(id));
}

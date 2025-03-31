using Application.Contracts;
using Application.Dtos.OrderItems;
using Application.Dtos.Orders;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Manages order items within an order.
/// </summary>
[Route("api/orders/{orderId}/order-items")]
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
    [HttpPost]
    [ProducesResponseType(typeof(OrderReadDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<OrderReadDto>> AddOrderItems(
        [FromRoute] Guid orderId,
        [FromBody] List<OrderItemCreateDto> orderItemDtos
    ) => HandleResult(await orderItemService.AddOrderItems(orderId, orderItemDtos));

    /// <summary>
    /// Retrieves a specific order item by ID.
    /// </summary>
    /// <param name="orderId">The order ID.</param>
    /// <param name="id">The unique order item ID.</param>
    /// <returns>The requested order item.</returns>
    /// <response code="200">Returns the order item.</response>
    /// <response code="404">If the order item is not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OrderItemReadDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<OrderItemReadDto>> GetOrderItem(
        [FromRoute] Guid orderId,
        [FromRoute] Guid id
    ) => HandleResult(await orderItemService.GetOrderItem(orderId, id));

    /// <summary>
    /// Retrieves all items in a specific order.
    /// </summary>
    /// <param name="orderId">The ID of the order.</param>
    /// <returns>A list of order items.</returns>
    /// <response code="200">Returns the list of order items.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<OrderItemsListDto>), 200)]
    public async Task<ActionResult<List<OrderItemsListDto>>> GetOrderItems(
        [FromRoute] Guid orderId
    ) => HandleResult(await orderItemService.GetOrderItems(orderId));

    /// <summary>
    /// Updates an order item.
    /// </summary>
    /// <param name="orderId">The ID of the order containing the item.</param>
    /// <param name="id">The ID of the order item to update.</param>
    /// <param name="updateDto">The updated order item details.</param>
    /// <returns>The updated order item.</returns>
    /// <response code="200">If the update was successful.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="404">If the order item is not found.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(OrderItemReadDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<OrderItemReadDto>> UpdateOrderItem(
        [FromRoute] Guid orderId,
        [FromRoute] Guid id,
        [FromBody] OrderItemUpdateDto updateDto
    ) => HandleResult(await orderItemService.UpdateOrderItem(orderId, id, updateDto));

    /// <summary>
    /// Updates the status of an order item.
    /// </summary>
    /// <param name="orderId">The order ID.</param>
    /// <param name="id">The order item ID.</param>
    /// <param name="status">The new order item status.</param>
    /// <returns>Success or failure.</returns>
    /// <response code="200">If the update was successful.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="404">If the order item is not found.</response>
    [HttpPatch("{id}/status")]
    [ProducesResponseType(typeof(bool), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<bool>> UpdateOrderItemStatus(
        [FromRoute] Guid orderId,
        [FromRoute] Guid id,
        [FromBody] OrderItemStatus status
    ) => HandleResult(await orderItemService.UpdateOrderItemStatus(orderId, id, status));

    /// <summary>
    /// Deletes an order item.
    /// </summary>
    /// <param name="orderId">The ID of the order.</param>
    /// <param name="id">The ID of the order item to delete.</param>
    /// <returns>No content.</returns>
    /// <response code="204">If the deletion was successful.</response>
    /// <response code="404">If the order item is not found.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteOrderItem(
        [FromRoute] Guid orderId,
        [FromRoute] Guid id
    ) => HandleResult(await orderItemService.DeleteOrderItem(orderId, id));
}

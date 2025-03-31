using Application.Contracts;
using Application.Dtos.Orders;
using Application.Dtos.Orders.OrderDelivery;
using Application.Dtos.Orders.OrderDineIn;
using Application.Dtos.Orders.OrderTakeAway;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Manages orders including dine-in, takeaway, and delivery.
/// </summary>
[Route("api/orders")]
public class OrderController(IOrderService orderService) : BaseApiController
{
    /// <summary>
    /// Creates a new dine-in order.
    /// </summary>
    /// <param name="dineInOrderDto">The dine-in order details.</param>
    /// <response code="201">Returns the newly created order.</response>
    /// <response code="400">If the input is invalid.</response>
    [HttpPost("dinein")]
    [ProducesResponseType(typeof(OrderReadDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<OrderReadDto>> CreateDineInOrder([FromBody] DineInOrderCreateDto dineInOrderDto) =>
        HandleResult(await orderService.CreateDineInOrder(dineInOrderDto));

    /// <summary>
    /// Creates a new takeaway order.
    /// </summary>
    /// <param name="takeawayOrderDto">The takeaway order details.</param>
    /// <response code="201">Returns the newly created order.</response>
    /// <response code="400">If the input is invalid.</response>
    [HttpPost("takeaway")]
    [ProducesResponseType(typeof(OrderReadDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<OrderReadDto>> CreateTakeawayOrder([FromBody] TakeawayOrderCreateDto takeawayOrderDto) =>
        HandleResult(await orderService.CreateTakeawayOrder(takeawayOrderDto));

    /// <summary>
    /// Creates a new delivery order.
    /// </summary>
    /// <param name="deliveryOrderDto">The delivery order details.</param>
    /// <response code="201">Returns the newly created order.</response>
    /// <response code="400">If the input is invalid.</response>
    [HttpPost("delivery")]
    [ProducesResponseType(typeof(OrderReadDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<OrderReadDto>> CreateDeliveryOrder([FromBody] DeliveryOrderCreateDto deliveryOrderDto) =>
        HandleResult(await orderService.CreateDeliveryOrder(deliveryOrderDto));

    /// <summary>
    /// Retrieves a specific order by ID.
    /// </summary>
    /// <param name="id">The unique order ID.</param>
    /// <response code="200">Returns the order.</response>
    /// <response code="404">If the order is not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OrderReadDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<OrderReadDto>> GetOrder([FromRoute] Guid id) =>
        HandleResult(await orderService.GetOrder(id));

    /// <summary>
    /// Retrieves all orders, optionally filtering by status.
    /// </summary>
    /// <param name="orderStatus">Optional filter by order status.</param>
    /// <response code="200">Returns the list of orders.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<OrderReadDto>), 200)]
    public async Task<ActionResult<List<OrderReadDto>>> GetOrders([FromQuery] OrderStatus? orderStatus) =>
        HandleResult(await orderService.GetOrders(orderStatus));

    /// <summary>
    /// Retrieves all ongoing non-dine-in orders based on order type.
    /// </summary>
    /// <param name="orderType">The type of the order (Takeaway or Delivery).</param>
    /// <response code="200">Returns the list of non-dine-in orders.</response>
    [HttpGet("non-dinein-orders")]
    [ProducesResponseType(typeof(List<NonDineInOrderSummaryDto>), 200)]
    public async Task<ActionResult<List<NonDineInOrderSummaryDto>>> GetOngoingNonDineInOrders([FromQuery] OrderType orderType, [FromQuery] DateTime date) =>
        HandleResult(await orderService.GetOngoingNonDineInOrders(orderType, date));

    /// <summary>
    /// Applies a discount to an existing order.
    /// </summary>
    /// <param name="id">The ID of the order.</param>
    /// <param name="discount">The discount percentage to apply.</param>
    /// <response code="200">Returns the updated order with the applied discount.</response>
    /// <response code="404">If the order is not found.</response>
    [HttpPatch("{id}/apply-discount")]
    [ProducesResponseType(typeof(OrderReadDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<OrderReadDto>> ApplyOrderDiscount([FromRoute] Guid id, [FromBody] decimal discount) =>
        HandleResult(await orderService.ApplyOrderDiscount(id, discount));

    /// <summary>
    /// Changes the table assigned to a dine-in order.
    /// </summary>
    /// <param name="id">The ID of the order.</param>
    /// <param name="newTableId">The new table ID.</param>
    [HttpPatch("{id}/change-table")]
    [ProducesResponseType(typeof(OrderReadDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<OrderReadDto>> ChangeOrderTable([FromRoute] Guid id, [FromBody] Guid newTableId) =>
        HandleResult(await orderService.ChangeOrderTable(id, newTableId));

    /// <summary>
    /// Closes an order by ID.
    /// </summary>
    [HttpPatch("{id}/close")]
    [ProducesResponseType(typeof(OrderReadDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<OrderReadDto>> CloseOrder([FromRoute] Guid id) =>
        HandleResult(await orderService.CloseOrder(id));

    /// <summary>
    /// Updates the status of an order.
    /// </summary>
    [HttpPatch("{id}/status")]
    [ProducesResponseType(typeof(OrderReadDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<OrderReadDto>> UpdateOrderStatus([FromRoute] Guid id, [FromBody] OrderStatus newStatus) =>
        HandleResult(await orderService.UpdateOrderStatus(id, newStatus));

    /// <summary>
    /// Updates the type of an order (DineIn, Takeaway, Delivery).
    /// </summary>
    /// <param name="id">The order ID.</param>
    /// <param name="updateTypeDto">Additional details required for the update.</param>
    /// <returns>The updated order with the new type.</returns>
    /// <response code="200">Returns the updated order with the new type.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="404">If the order is not found.</response>
    [HttpPatch("{id}/type")]
    [ProducesResponseType(typeof(OrderReadDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<OrderReadDto>> UpdateOrderType([FromRoute] Guid id, [FromBody] OrderUpdateTypeDto updateTypeDto) =>
        HandleResult(await orderService.UpdateOrderType(id, updateTypeDto));

    /// <summary>
    /// Splits an existing order, moving specific items to a new order.
    /// </summary>
    [HttpPost("{id}/split")]
    [ProducesResponseType(typeof(OrderReadDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<OrderReadDto>> SplitOrder([FromRoute] Guid id, [FromBody] MoveOrderItemsDto splitOrderDto) =>
        HandleResult(await orderService.SplitOrder(id, splitOrderDto));

    /// <summary>
    /// Joins an existing order into another order, merging all items and removing the source order.
    /// </summary>
    /// <param name="sourceOrderId">The ID of the order to be merged.</param>
    /// <param name="targetOrderId">The ID of the order that will receive the merged items.</param>
    /// <returns>The updated target order details.</returns>
    [HttpPost("{sourceOrderId}/join/{targetOrderId}")]
    [ProducesResponseType(typeof(OrderReadDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<OrderReadDto>> JoinOrder(Guid sourceOrderId, Guid targetOrderId) =>
        HandleResult(await orderService.JoinOrder(sourceOrderId, targetOrderId));

    /// <summary>
    /// Moves specific order items from one order to another.
    /// </summary>
    /// <param name="sourceOrderId">The ID of the order from which items will be moved.</param>
    /// <param name="targetOrderId">The ID of the order that will receive the items.</param>
    /// <param name="moveOrderItemsDto">List of order item IDs to move.</param>
    /// <returns>The updated target order details.</returns>
    [HttpPost("{sourceOrderId}/move-items/{targetOrderId}")]
    [ProducesResponseType(typeof(OrderReadDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<OrderReadDto>> MoveOrderItems(Guid sourceOrderId, Guid targetOrderId, [FromBody] MoveOrderItemsDto moveOrderItemsDto) =>
        HandleResult(await orderService.MoveOrderItems(sourceOrderId, targetOrderId, moveOrderItemsDto));

    /// <summary>
    /// Deletes an order by ID.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteOrder([FromRoute] Guid id) =>
        HandleResult(await orderService.DeleteOrder(id));
}
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
public class OrderController(IOrderService orderService) : BaseApiController
{
    /// <summary>
    /// Creates a new dine-in order.
    /// </summary>
    /// <param name="dineInOrderDto">The dine-in order details.</param>
    /// <returns>The created dine-in order.</returns>
    /// <response code="201">Returns the newly created order.</response>
    /// <response code="400">If the input is invalid.</response>
    [HttpPost("dinein")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> CreateDineInOrder([FromBody] DineInOrderCreateDto dineInOrderDto) =>
        HandleResult(await orderService.CreateDineInOrder(dineInOrderDto));

    /// <summary>
    /// Creates a new takeaway order.
    /// </summary>
    /// <param name="takeawayOrderDto">The takeaway order details.</param>
    /// <returns>The created takeaway order.</returns>
    [HttpPost("takeaway")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> CreateTakeawayOrder([FromBody] TakeawayOrderCreateDto takeawayOrderDto) =>
        HandleResult(await orderService.CreateTakeawayOrder(takeawayOrderDto));

    /// <summary>
    /// Creates a new delivery order.
    /// </summary>
    /// <param name="deliveryOrderDto">The delivery order details.</param>
    /// <returns>The created delivery order.</returns>
    [HttpPost("delivery")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> CreateDeliveryOrder([FromBody] DeliveryOrderCreateDto deliveryOrderDto) =>
        HandleResult(await orderService.CreateDeliveryOrder(deliveryOrderDto));

    /// <summary>
    /// Splits an existing order, moving specific items to a new order.
    /// </summary>
    /// <param name="orderId">The ID of the order to split.</param>
    /// <param name="splitOrderDto">Details of the items to move.</param>
    /// <returns>The new split order.</returns>
    [HttpPost("{orderId}/split")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> SplitOrder([FromBody] SplitOrderDto splitOrderDto, Guid orderId) =>
        HandleResult(await orderService.SplitOrder(splitOrderDto, orderId));

    /// <summary>
    /// Retrieves a specific order by ID.
    /// </summary>
    /// <param name="id">The unique order ID.</param>
    /// <returns>The requested order.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetOrder(Guid id) =>
        HandleResult(await orderService.GetOrder(id));

    /// <summary>
    /// Retrieves all orders, optionally filtering by status.
    /// </summary>
    /// <param name="orderStatus">Optional filter by order status.</param>
    /// <returns>A list of orders.</returns>
    [HttpGet]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetAllOrders([FromQuery] OrderStatus? orderStatus) =>
        HandleResult(await orderService.GetAllOrders(orderStatus));

    /// <summary>
    /// Applies a discount to an existing order.
    /// </summary>
    /// <param name="orderId">The ID of the order.</param>
    /// <param name="discount">The discount percentage to apply.</param>
    [HttpPut("{orderId}/apply-discount")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> ApplyOrderDiscount(decimal discount, Guid orderId) =>
        HandleResult(await orderService.ApplyOrderDiscount(discount, orderId));

    /// <summary>
    /// Changes the table assigned to a dine-in order.
    /// </summary>
    /// <param name="orderId">The ID of the order.</param>
    /// <param name="newTableId">The new table ID.</param>
    [HttpPut("{orderId}/change-table")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> ChangeOrderTable(Guid orderId, [FromBody] Guid newTableId) =>
        HandleResult(await orderService.ChangeOrderTable(orderId, newTableId));

    /// <summary>
    /// Updates the status of an order.
    /// </summary>
    /// <param name="id">The order ID.</param>
    /// <param name="newStatus">The new order status.</param>
    [HttpPut("{id}/status")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> UpdateOrderStatus([FromBody] OrderStatus newStatus, Guid id) =>
        HandleResult(await orderService.UpdateOrderStatus(newStatus, id));

    /// <summary>
    /// Updates the type of an order (DineIn, Takeaway, Delivery).
    /// </summary>
    /// <param name="orderId">The order ID.</param>
    /// <param name="newOrderType">The new order type.</param>
    /// <param name="updateTypeDto">Additional details required for the update.</param>
    [HttpPut("{orderId}/type")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> UpdateOrderType([FromQuery] OrderType newOrderType, [FromBody] OrderUpdateTypeDto updateTypeDto, Guid orderId) =>
        HandleResult(await orderService.UpdateOrderType(newOrderType, updateTypeDto, orderId));

    /// <summary>
    /// Deletes an order by ID.
    /// </summary>
    /// <param name="id">The order ID to delete.</param>
    /// <returns>No content.</returns>
    /// <response code="204">If the deletion was successful.</response>
    /// <response code="404">If the order is not found.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteOrder(Guid id) =>
        HandleResult(await orderService.DeleteOrder(id));
}

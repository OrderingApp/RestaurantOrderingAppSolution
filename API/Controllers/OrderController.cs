using Application.Contracts;
using Application.Dtos.Orders;
using Application.Dtos.Orders.OrderDelivery;
using Application.Dtos.Orders.OrderDineIn;
using Application.Dtos.Orders.OrderTakeAway;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class OrderController(IOrderService orderService) : BaseApiController
{
    [HttpPost("dinein")]
    public async Task<IActionResult> CreateDineInOrder([FromBody] DineInOrderCreateDto dineInOrderDto) =>
        HandleResult(await orderService.CreateDineInOrder(dineInOrderDto));

    [HttpPost("takeaway")]
    public async Task<IActionResult> CreateTakeawayOrder([FromBody] TakeawayOrderCreateDto takeawayOrderDto) =>
        HandleResult(await orderService.CreateTakeawayOrder(takeawayOrderDto));

    [HttpPost("delivery")]
    public async Task<IActionResult> CreateDeliveryOrder([FromBody] DeliveryOrderCreateDto deliveryOrderDto) =>
        HandleResult(await orderService.CreateDeliveryOrder(deliveryOrderDto));

    [HttpPost("{orderId}/split")]
    public async Task<IActionResult> SplitOrder([FromBody] SplitOrderDto splitOrderDto, Guid orderId) =>
        HandleResult(await orderService.SplitOrder(splitOrderDto, orderId));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(Guid id) =>
        HandleResult(await orderService.GetOrder(id));

    [HttpGet]
    public async Task<IActionResult> GetAllOrders([FromQuery] OrderStatus? orderStatus) =>
        HandleResult(await orderService.GetAllOrders(orderStatus));

    [HttpGet("type/{orderType}/ongoing")]
    public async Task<IActionResult> GetOngoingOrdersByType(OrderType orderType) =>
        HandleResult(await orderService.GetOngoingOrdersByType(orderType));

    [HttpGet("table/{tableId}/ongoing")]
    public async Task<IActionResult> GetOngoingOrdersForTable(Guid tableId) =>
        HandleResult(await orderService.GetOngoingOrdersForTable(tableId));

    [HttpPut("{orderId}/apply-discount")]
    public async Task<IActionResult> ApplyOrderDiscount(decimal discount, Guid orderId) =>
        HandleResult(await orderService.ApplyOrderDiscount(discount, orderId));

    [HttpPut("{orderId}/change-table")]
    public async Task<IActionResult> ChangeOrderTable(Guid orderId, [FromBody] Guid newTableId) =>
        HandleResult(await orderService.ChangeOrderTable(orderId, newTableId));

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateOrderStatus([FromBody] OrderStatus newStatus, Guid id) =>
        HandleResult(await orderService.UpdateOrderStatus(newStatus, id));

    [HttpPut("{orderId}/type")]
    public async Task<IActionResult> UpdateOrderType([FromQuery] OrderType newOrderType, [FromBody] OrderUpdateTypeDto updateTypeDto, Guid orderId) =>
        HandleResult(await orderService.UpdateOrderType(newOrderType, updateTypeDto, orderId));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(Guid id) =>
        HandleResult(await orderService.DeleteOrder(id));
}

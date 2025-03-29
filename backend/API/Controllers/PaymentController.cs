using Application.Contracts;
using Application.Dtos.Payments;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Manages payments for orders.
/// </summary>
[Route("orders/{orderId}/payments")]
public class PaymentController(IPaymentService paymentService) : BaseApiController
{
    /// <summary>
    /// Creates a new payment for a specific order.
    /// </summary>
    /// <param name="orderId">The ID of the order being paid.</param>
    /// <param name="paymentDto">The payment details.</param>
    /// <returns>The created payment.</returns>
    /// <response code="201">If the payment was successfully created.</response>
    /// <response code="400">If the request is invalid.</response>
    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> AddPayment(
        [FromRoute] Guid orderId,
        [FromBody] PaymentCreateDto paymentDto
    ) => HandleResult(await paymentService.AddPayment(orderId, paymentDto));

    /// <summary>
    /// Retrieves all payments for a specific order.
    /// </summary>
    /// <param name="orderId">The ID of the order.</param>
    /// <returns>A list of payments associated with the order.</returns>
    /// <response code="200">Returns the list of payments.</response>
    [HttpGet]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetAllOrderPayments([FromRoute] Guid orderId) =>
        HandleResult(await paymentService.GetAllOrderPayments(orderId));

    /// <summary>
    /// Change status to refunded of payment.
    /// </summary>
    /// <param name="id">The ID of the payment.</param>
    /// <param name="orderId">The ID of the order.</param>
    /// <returns>changed status.</returns>
    /// <response code="200">Returns the list of payments.</response>
    [HttpPatch("{id}")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> MarkPaymentAsRefunded(
        [FromRoute] Guid id,
        [FromRoute] Guid orderId
    ) => HandleResult(await paymentService.MarkPaymentAsRefunded(id, orderId));
}

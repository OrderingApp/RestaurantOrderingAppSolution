using Application.Contracts;
using Application.Dtos.Payments;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Manages payments for orders.
/// </summary>
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
    [HttpPost("{orderId}")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> CreatePayment([FromBody] PaymentCreateDto paymentDto, Guid orderId) =>
        HandleResult(await paymentService.CreatePayment(paymentDto, orderId));

    /// <summary>
    /// Retrieves all payments for a specific order.
    /// </summary>
    /// <param name="orderId">The ID of the order.</param>
    /// <returns>A list of payments associated with the order.</returns>
    /// <response code="200">Returns the list of payments.</response>
    [HttpGet("/api/orders/{orderId}/payments")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetAllOrderPayments(Guid orderId) =>
        HandleResult(await paymentService.GetAllOrderPayments(orderId));

    /// <summary>
    /// Updates the status of a specific payment.
    /// </summary>
    /// <param name="paymentId">The ID of the payment to update.</param>
    /// <param name="paymentStatus">The new status of the payment.</param>
    /// <returns>The updated payment status.</returns>
    /// <response code="200">If the update was successful.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="404">If the payment is not found.</response>
    [HttpPatch("{paymentId}/status")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdatePaymentStatus([FromBody] PaymentStatus paymentStatus, Guid paymentId) =>
        HandleResult(await paymentService.UpdatePaymentStatus(paymentStatus, paymentId));
}

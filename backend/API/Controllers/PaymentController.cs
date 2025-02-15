using Application.Contracts;
using Application.Dtos.Payments;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Manages payments for orders.
/// </summary>
[Route("/orders/{orderId}/payments/")]
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
    public async Task<IActionResult> AddPayment([FromBody] PaymentCreateDto paymentDto, Guid orderId) =>
        HandleResult(await paymentService.AddPayment(paymentDto, orderId));

    /// <summary>
    /// Retrieves all payments for a specific order.
    /// </summary>
    /// <param name="orderId">The ID of the order.</param>
    /// <returns>A list of payments associated with the order.</returns>
    /// <response code="200">Returns the list of payments.</response>
    [HttpGet]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetAllOrderPayments(Guid orderId) =>
        HandleResult(await paymentService.GetAllOrderPayments(orderId));
}

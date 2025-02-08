using Application.Contracts;
using Application.Dtos.Payments;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class PaymentController(IPaymentService paymentService) : BaseApiController
{
    [HttpPost("{orderId}")]
    public async Task<IActionResult> CreatePayment([FromBody] PaymentCreateDto paymentDto, Guid orderId) =>
        HandleResult(await paymentService.CreatePayment(paymentDto, orderId));

    [HttpGet("/api/orders/{orderId}/payments")]
    public async Task<IActionResult> GetAllOrderPayments(Guid orderId) =>
        HandleResult(await paymentService.GetAllOrderPayments(orderId));

    [HttpPatch("{paymentId}/status")]
    public async Task<IActionResult> UpdatePaymentStatus([FromBody] PaymentStatus paymentStatus, Guid paymentId) =>
        HandleResult(await paymentService.UpdatePaymentStatus(paymentStatus, paymentId));

}

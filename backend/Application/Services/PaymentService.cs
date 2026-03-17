using System.Net;
using Application.Contracts;
using Application.Dtos.Common;
using Application.Dtos.Payments;
using AutoMapper;
using Domain;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using RestaurantOrdering.Events.Application.Contracts;
using RestaurantOrdering.Events.Domain.Payments;

namespace Application.Services;

public class PaymentService(
    RestaurantOrderingContext orderingContext,
    IEventHandlerService eventHandlerService,
    IMapper mapper
) : IPaymentService
{
    public async Task<ResultDto<PaymentReadDto>> AddPayment(
        Guid orderId,
        PaymentCreateDto paymentDto
    )
    {
        try
        {
            var order = await orderingContext
                .Orders.Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return ResultDto<PaymentReadDto>.Failure(
                    "Order not found",
                    HttpStatusCode.NotFound
                );

            if (order.Status == OrderStatus.Closed || order.Status == OrderStatus.Cancelled)
                return ResultDto<PaymentReadDto>.Failure(
                    "Cannot add a payment to an order that is closed or cancelled.",
                    HttpStatusCode.BadRequest
                );

            var activelyPaid = order.Payments.Where(p => !p.IsRefunded).Sum(p => p.Amount);

            if (activelyPaid + paymentDto.Amount > order.TotalAmount)
                return ResultDto<PaymentReadDto>.Failure(
                    "Payment exceeds order total.",
                    HttpStatusCode.BadRequest
                );

            var payment = mapper.Map<Payment>(paymentDto);
            payment.OrderId = orderId;

            orderingContext.Payments.Add(payment);

            order.PaidAmount = activelyPaid + paymentDto.Amount;
            order.PaymentStatus = DerivePaymentStatus(order.PaidAmount, order.TotalAmount);

            await orderingContext.SaveChangesAsync();

            var paymentReadDto = mapper.Map<PaymentReadDto>(payment);

            var paymentCreatedEvent = mapper.Map<PaymentCreatedEvent>(payment);
            await eventHandlerService.HandleEventAsync(paymentCreatedEvent);

            return ResultDto<PaymentReadDto>.Success(paymentReadDto, HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            return ResultDto<PaymentReadDto>.Failure(
                $"An error occurred: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ResultDto<List<PaymentReadDto>>> GetAllOrderPayments(Guid orderId)
    {
        try
        {
            var orderExists = await orderingContext.Orders.AnyAsync(o => o.Id == orderId);
            if (!orderExists)
                return ResultDto<List<PaymentReadDto>>.Failure(
                    "Order not found",
                    HttpStatusCode.NotFound
                );

            var payments = await orderingContext
                .Payments.Where(p => p.OrderId == orderId)
                .ToListAsync();

            var paymentReadDtos = mapper.Map<List<PaymentReadDto>>(payments);

            return ResultDto<List<PaymentReadDto>>.Success(paymentReadDtos, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<List<PaymentReadDto>>.Failure(
                $"An error occurred: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ResultDto<PaymentReadDto>> MarkPaymentAsRefunded(Guid id, Guid orderId)
    {
        try
        {
            var order = await orderingContext
                .Orders.Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return ResultDto<PaymentReadDto>.Failure(
                    "Order not found",
                    HttpStatusCode.NotFound
                );

            var payment = order.Payments.FirstOrDefault(p => p.Id == id);

            if (payment == null)
                return ResultDto<PaymentReadDto>.Failure(
                    "Payment not found",
                    HttpStatusCode.NotFound
                );

            payment.IsRefunded = true;

            order.PaidAmount = order.Payments.Where(p => !p.IsRefunded).Sum(p => p.Amount);
            order.PaymentStatus = DerivePaymentStatus(order.PaidAmount, order.TotalAmount);

            await orderingContext.SaveChangesAsync();

            var paymentReadDto = mapper.Map<PaymentReadDto>(payment);

            return ResultDto<PaymentReadDto>.Success(paymentReadDto, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<PaymentReadDto>.Failure(
                $"An error occurred: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    private static PaymentStatus DerivePaymentStatus(decimal paidAmount, decimal totalAmount)
    {
        if (paidAmount <= 0) return PaymentStatus.Unpaid;
        if (paidAmount < totalAmount) return PaymentStatus.PartiallyPaid;
        return PaymentStatus.Paid;
    }
}

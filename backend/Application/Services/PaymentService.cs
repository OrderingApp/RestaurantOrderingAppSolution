using Application.Contracts;
using Application.Dtos.Common;
using Application.Dtos.Payments;
using AutoMapper;
using Domain;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using RestaurantOrdering.Events.Application.Contracts;
using RestaurantOrdering.Events.Domain.Payments;
using System.Net;

namespace Application.Services;

public class PaymentService(RestaurantOrderingContext orderingContext, IEventHandlerService eventHandlerService, IMapper mapper) : IPaymentService
{
    public async Task<ResultDto<PaymentReadDto>> CreatePayment(PaymentCreateDto paymentDto, Guid orderId)
    {
        try
        {
            var order = await orderingContext.Orders
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return ResultDto<PaymentReadDto>.Failure("Order not found", HttpStatusCode.NotFound);

            var totalPaid = order.Payments.Where(p => p.PaymentStatus == PaymentStatus.Paid).Sum(p => p.Amount);

            if (totalPaid + paymentDto.Amount > order.TotalAmount)
                return ResultDto<PaymentReadDto>.Failure("Payment exceeds order total.", HttpStatusCode.BadRequest);

            var payment = mapper.Map<Payment>(paymentDto);
            payment.OrderId = orderId;

            orderingContext.Payments.Add(payment);
            await orderingContext.SaveChangesAsync();

            var paymentReadDto = mapper.Map<PaymentReadDto>(payment);

            var paymentCreatedEvent = mapper.Map<PaymentCreatedEvent>(payment);
            await eventHandlerService.HandleEventAsync(paymentCreatedEvent);

            return ResultDto<PaymentReadDto>.Success(paymentReadDto, HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            return ResultDto<PaymentReadDto>
                .Failure($"An error occurred: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ResultDto<List<PaymentReadDto>>> GetAllOrderPayments(Guid orderId)
    {
        try
        {
            var orderExists = await orderingContext.Orders.AnyAsync(o => o.Id == orderId);
            if (!orderExists)
                return ResultDto<List<PaymentReadDto>>.Failure("Order not found", HttpStatusCode.NotFound);

            var payments = await orderingContext.Payments
                .Where(p => p.OrderId == orderId)
                .ToListAsync();

            var paymentReadDtos = mapper.Map<List<PaymentReadDto>>(payments);

            return ResultDto<List<PaymentReadDto>>.Success(paymentReadDtos, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<List<PaymentReadDto>>
                .Failure($"An error occurred: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ResultDto<PaymentReadDto>> UpdatePaymentStatus(PaymentStatus paymentStatus, Guid paymentId)
    {
        try
        {
            var payment = await orderingContext.Payments.FindAsync(paymentId);
            if (payment == null)
                return ResultDto<PaymentReadDto>.Failure("Payment not found", HttpStatusCode.NotFound);

            if (payment.PaymentStatus == PaymentStatus.Paid && paymentStatus == PaymentStatus.Pending)
                return ResultDto<PaymentReadDto>.Failure("Cannot revert a paid payment to pending.", HttpStatusCode.BadRequest);

            if(payment.PaymentStatus == PaymentStatus.Cancelled)
            return ResultDto<PaymentReadDto>.Failure("Cannot update a cancelled payment.", HttpStatusCode.BadRequest);

            var previousPaymentStatus = payment.PaymentStatus;

            payment.PaymentStatus = paymentStatus;
            if (paymentStatus == PaymentStatus.Paid)
                payment.PaidAt = DateTime.UtcNow;

            await orderingContext.SaveChangesAsync();

            var paymentReadDto = mapper.Map<PaymentReadDto>(payment);

            var statusChangedEvent = mapper.Map<PaymentStatusChangedEvent>((payment, previousPaymentStatus));
            await eventHandlerService.HandleEventAsync(statusChangedEvent);

            return ResultDto<PaymentReadDto>.Success(paymentReadDto, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<PaymentReadDto>
                .Failure($"An error occurred: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }
}

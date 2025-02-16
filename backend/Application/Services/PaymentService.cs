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
    public async Task<ResultDto<PaymentReadDto>> AddPayment(PaymentCreateDto paymentDto, Guid orderId)
    {
        try
        {
            var order = await orderingContext.Orders
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return ResultDto<PaymentReadDto>.Failure("Order not found", HttpStatusCode.NotFound);

            if (order.Status != OrderStatus.PendingPayment)
                return ResultDto<PaymentReadDto>.Failure("Order must be in Pending Payment status to add payment.", HttpStatusCode.BadRequest);

            var totalPaid = order.Payments.Sum(p => p.Amount);

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
}

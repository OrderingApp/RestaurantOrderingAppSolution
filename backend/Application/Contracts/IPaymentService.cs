using Application.Dtos.Common;
using Application.Dtos.Payments;
using Domain;

namespace Application.Contracts;

public interface IPaymentService
{
    Task<ResultDto<PaymentReadDto>> CreatePayment(PaymentCreateDto paymentDto, Guid orderId);
    Task<ResultDto<List<PaymentReadDto>>> GetAllOrderPayments(Guid orderId);
}

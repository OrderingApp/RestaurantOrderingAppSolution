using Application.Dtos.Common;
using Application.Dtos.Payments;

namespace Application.Contracts;

public interface IPaymentService
{
    Task<ResultDto<PaymentReadDto>> AddPayment(Guid orderId, PaymentCreateDto paymentDto);
    Task<ResultDto<List<PaymentReadDto>>> GetAllOrderPayments(Guid orderId);
    Task<ResultDto<PaymentReadDto>> MarkPaymentAsRefunded(Guid id, Guid orderId);
}

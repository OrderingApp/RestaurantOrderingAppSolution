using Application.Dtos.Common;
using Application.Dtos.Payments;

namespace Application.Contracts;

public interface IPaymentService
{
    Task<ResultDto<PaymentReadDto>> AddPayment(PaymentCreateDto paymentDto, Guid orderId);
    Task<ResultDto<List<PaymentReadDto>>> GetAllOrderPayments(Guid orderId);
}

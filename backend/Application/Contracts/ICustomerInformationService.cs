using Application.Dtos.Common;
using Application.Dtos.CustomerInformations;

namespace Application.Contracts;

public interface ICustomerInformationService
{
    Task<ResultDto<CustomerInformationReadDto>> CreateCustomerInformation(CustomerInformationCreateDto customerInformationCreateDto, Guid orderId);
    Task<ResultDto<CustomerInformationReadDto>> GetCustomerInformation(Guid id);
    Task<ResultDto<List<CustomerInformationReadDto>>> GetAllCustomerInformations();
    Task<ResultDto<CustomerInformationReadDto>> UpdateCustomerInformation(CustomerInformationUpdateDto customerInformationUpdateDto, Guid id);
    Task<ResultDto<bool>> DeleteCustomerInformation(Guid id); 
}

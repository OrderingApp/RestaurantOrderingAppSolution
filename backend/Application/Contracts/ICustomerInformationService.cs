using Application.Dtos.Common;
using Application.Dtos.CustomerInformations;

namespace Application.Contracts;

public interface ICustomerInformationService
{
    Task<ResultDto<CustomerInformationReadDto>> GetCustomerInformation(Guid id);
    Task<ResultDto<CustomerInformationReadDto>> UpdateCustomerInformation(CustomerInformationUpdateDto customerInformationUpdateDto, Guid id);
}

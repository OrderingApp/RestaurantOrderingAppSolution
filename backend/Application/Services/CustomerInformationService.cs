using Application.Contracts;
using Application.Dtos.Common;
using Application.Dtos.CustomerInformations;
using AutoMapper;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using RestaurantOrdering.Events.Application.Contracts;
using RestaurantOrdering.Events.Domain.CustomerInformations;
using System.Net;

namespace Application.Services;

public class CustomerInformationService(RestaurantOrderingContext orderingContext, IEventHandlerService eventHandlerService, IMapper mapper) : ICustomerInformationService
{
    public async Task<ResultDto<CustomerInformationReadDto>> GetCustomerInformation(Guid id)
    {
        try
        {
            var customerInformation = await orderingContext.CustomerInformations
                .FirstOrDefaultAsync(ci => ci.Id == id);

            if (customerInformation == null)
            {
                return ResultDto<CustomerInformationReadDto>
                    .Failure("Customer information not found.", HttpStatusCode.NotFound);
            }

            var customerInformationDto = mapper.Map<CustomerInformationReadDto>(customerInformation);

            return ResultDto<CustomerInformationReadDto>
                .Success(customerInformationDto, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<CustomerInformationReadDto>
                .Failure($"An error occurred: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ResultDto<CustomerInformationReadDto>> UpdateCustomerInformation(CustomerInformationUpdateDto customerInformationUpdateDto, Guid id)
    {
        try
        {
            var customerInformation = await orderingContext.CustomerInformations
                .FirstOrDefaultAsync(ci => ci.Id == id);

            if (customerInformation == null)
            {
                return ResultDto<CustomerInformationReadDto>
                    .Failure("Customer information not found.", HttpStatusCode.NotFound);
            }

            mapper.Map(customerInformationUpdateDto, customerInformation);
            await orderingContext.SaveChangesAsync();

            var updatedCustomerInformationDto = mapper.Map<CustomerInformationReadDto>(customerInformation);

            var customerInformationUpdatedEvent = mapper.Map<CustomerInformationUpdatedEvent>(customerInformation);
            await eventHandlerService.HandleEventAsync(customerInformationUpdatedEvent);

            return ResultDto<CustomerInformationReadDto>
                .Success(updatedCustomerInformationDto, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<CustomerInformationReadDto>
                .Failure($"An error occurred: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }
}

using Application.Contracts;
using Application.Dtos.Common;
using Application.Dtos.CustomerInformations;
using AutoMapper;
using Domain;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using RestaurantOrdering.Events.Application;
using RestaurantOrdering.Events.Application.Contracts;
using RestaurantOrdering.Events.Domain.CustomerInformations;
using RestaurantOrdering.Events.Domain.Orders.CreatingOrder;
using System.Net;

namespace Application.Services;

public class CustomerInformationService(RestaurantOrderingContext orderingContext, IEventHandlerService eventHandlerService, IMapper mapper) : ICustomerInformationService
{
    public async Task<ResultDto<CustomerInformationReadDto>> CreateCustomerInformation(CustomerInformationCreateDto customerInformationCreateDto, Guid orderId)
    {
        try
        {
            var order = await orderingContext.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return ResultDto<CustomerInformationReadDto>.Failure("Order not found.", HttpStatusCode.NotFound);

            if (order.OrderType == OrderType.DineIn)
                return ResultDto<CustomerInformationReadDto>.Failure("Customer information is not required for DineIn orders.", HttpStatusCode.BadRequest);

            var customerInformation = mapper.Map<CustomerInformation>(customerInformationCreateDto);
            customerInformation.OrderId = orderId;

            var result = await orderingContext.CustomerInformation.AddAsync(customerInformation);
            await orderingContext.SaveChangesAsync();

            var customerInformationCreatedEvent = mapper.Map<CustomerInformationCreatedEvent>(result.Entity);
            await eventHandlerService.HandleEventAsync(customerInformationCreatedEvent);

            var customerInformationDto = mapper.Map<CustomerInformationReadDto>(customerInformation);

            return ResultDto<CustomerInformationReadDto>
                .Success(customerInformationDto, HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            return ResultDto<CustomerInformationReadDto>
                .Failure($"An error occurred: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ResultDto<CustomerInformationReadDto>> GetCustomerInformation(Guid id)
    {
        try
        {
            var customerInformation = await orderingContext.CustomerInformation
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

    public async Task<ResultDto<List<CustomerInformationReadDto>>> GetAllCustomerInformations()
    {
        try
        {
            var customerInformations = await orderingContext.CustomerInformation.ToListAsync();

            var customerInformationsDto = mapper.Map<List<CustomerInformationReadDto>>(customerInformations);

            return ResultDto<List<CustomerInformationReadDto>>
                .Success(customerInformationsDto, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<List<CustomerInformationReadDto>>
                .Failure($"An error occurred: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ResultDto<CustomerInformationReadDto>> UpdateCustomerInformation(CustomerInformationUpdateDto customerInformationUpdateDto, Guid id)
    {
        try
        {
            var customerInformation = await orderingContext.CustomerInformation
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

    public async Task<ResultDto<bool>> DeleteCustomerInformation(Guid id)
    {
        try
        {
            var customerInformation = await orderingContext.CustomerInformation
                .FirstOrDefaultAsync(ci => ci.Id == id);

            if (customerInformation == null)
            {
                return ResultDto<bool>
                    .Failure("Customer information not found.", HttpStatusCode.NotFound);
            }

            orderingContext.CustomerInformation.Remove(customerInformation);
            await orderingContext.SaveChangesAsync();

            var customerInformationDeletedEvent = mapper.Map<CustomerInformationDeletedEvent>(customerInformation);
            await eventHandlerService.HandleEventAsync(customerInformationDeletedEvent);

            return ResultDto<bool>
                .Success(true, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<bool>
                .Failure($"An error occurred: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }
}

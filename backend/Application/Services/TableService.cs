using Application.Contracts;
using Application.Dtos.Common;
using Application.Dtos.Tables;
using AutoMapper;
using Domain;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using RestaurantOrdering.Events.Application.Contracts;
using RestaurantOrdering.Events.Domain.Tables;
using System.Net;

namespace Application.Services;

public class TableService(RestaurantOrderingContext orderingContext, IEventHandlerService eventHandlerService, IMapper mapper) : ITableService
{
    public async Task<ResultDto<TableReadDto>> CreateTable(TableCreateDto tableCreateDto)
    {
        try
        {
            var areaExists = await orderingContext.Areas.AnyAsync(a => a.Id == tableCreateDto.AreaId);
            if (!areaExists)
            {
                return ResultDto<TableReadDto>
                    .Failure("The specified AreaId does not exist.", HttpStatusCode.BadRequest);
            }

            var table = mapper.Map<Table>(tableCreateDto);

            await orderingContext.Tables.AddAsync(table);
            await orderingContext.SaveChangesAsync();

            var createdTableDto = mapper.Map<TableReadDto>(table);

            var tableCreatedEvent = mapper.Map<TableCreatedEvent>(table);
            await eventHandlerService.HandleEventAsync(tableCreatedEvent);

            return ResultDto<TableReadDto>
                .Success(createdTableDto, HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            return ResultDto<TableReadDto>
                .Failure($"An error occurred: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ResultDto<TableSummaryDto>> GetTable(Guid id)
    {
        try
        {
            var table = await orderingContext.Tables
                .Include(t => t.Orders)
                    .ThenInclude(o => o.OrderItems)
                .Include(o => o.Orders)
                    .ThenInclude(p => p.Payments)
                .FirstOrDefaultAsync(t => t.Id == id && t.IsUsed && !t.IsDeleted);

            if (table == null)
                return ResultDto<TableSummaryDto>
                    .Failure("Table not found or deleted.", HttpStatusCode.NotFound);

            var tableDto = mapper.Map<TableSummaryDto>(table);

            return ResultDto<TableSummaryDto>
                .Success(tableDto, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<TableSummaryDto>
                .Failure($"An error occurred: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ResultDto<List<TableReadDto>>> GetTables()
    {
        try
        {
            var tables = await orderingContext.Tables
            .Include(t => t.Reservations)
            .ToListAsync();

            var tablesDto = mapper.Map<List<TableReadDto>>(tables);

            return ResultDto<List<TableReadDto>>
                .Success(tablesDto, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<List<TableReadDto>>
                .Failure($"An error occurred: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ResultDto<TableReadDto>> UpdateTable(Guid id, TableUpdateDto tableUpdateDto)
    {
        try
        {
            var tableToUpdate = await orderingContext.Tables.FindAsync(id);

            if (tableToUpdate == null)
                return ResultDto<TableReadDto>
                    .Failure("Table not found.", HttpStatusCode.NotFound);

            mapper.Map(tableUpdateDto, tableToUpdate);

            await orderingContext.SaveChangesAsync();

            var updatedTable = mapper.Map<TableReadDto>(tableToUpdate);

            var tableUpdatedEvent = mapper.Map<TableUpdatedEvent>(tableToUpdate);
            await eventHandlerService.HandleEventAsync(tableUpdatedEvent);

            return ResultDto<TableReadDto>
                .Success(updatedTable, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<TableReadDto>
                .Failure($"An error occurred: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ResultDto<TableReadDto>> UpdateStatus(Guid id, TableStatus tableStatus)
    {
        try
        {
            var table = await orderingContext.Tables.FindAsync(id);

            if (table == null || table.IsDeleted)
                return ResultDto<TableReadDto>
                    .Failure("Table not found or has been deleted.", HttpStatusCode.NotFound);

            if (table.Status == tableStatus)
                return ResultDto<TableReadDto>
                    .Failure("Table already has the requested status.", HttpStatusCode.BadRequest);

            var previousStatus = table.Status;

            table.Status = tableStatus;

            await orderingContext.SaveChangesAsync();

            var updatedTableDto = mapper.Map<TableReadDto>(table);

            var tableStatusUpdatedEvent = mapper.Map<TableStatusUpdatedEvent>((table, tableStatus));
            await eventHandlerService.HandleEventAsync(tableStatusUpdatedEvent);

            return ResultDto<TableReadDto>
                .Success(updatedTableDto, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<TableReadDto>
                .Failure($"An error occurred: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ResultDto<bool>> DeleteTable(Guid id)
    {
        try
        {
            var table = await orderingContext.Tables.FindAsync(id);
            if(table == null)
                return ResultDto<bool>
                    .Failure("Table not found.", HttpStatusCode.NotFound);

            table.IsDeleted = true;
            //table.IsOccupied = false;

            orderingContext.Tables.Update(table);
            await orderingContext.SaveChangesAsync();

            var tableDeletedEvent = mapper.Map<TableDeletedEvent>(table);
            await eventHandlerService.HandleEventAsync(tableDeletedEvent);

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

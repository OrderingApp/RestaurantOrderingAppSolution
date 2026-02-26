using System.Net;
using System;
using Application.Contracts;
using Application.Dtos.Areas;
using Application.Dtos.Common;
using AutoMapper;
using Domain;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using RestaurantOrdering.Events.Application.Contracts;

namespace Application.Services;

public class AreaService(
    RestaurantOrderingContext orderingContext,
    IEventHandlerService eventHandlerService,
    IMapper mapper
) : IAreaService
{
    public async Task<ResultDto<AreaReadDto>> CreateArea(AreaCreateDto areaCreateDto, Guid userId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(areaCreateDto.Name))
                return ResultDto<AreaReadDto>.Failure("Area name is required", HttpStatusCode.BadRequest);

            if (await orderingContext.Areas.AnyAsync(a => a.Name == areaCreateDto.Name))
                return ResultDto<AreaReadDto>.Failure("Area name already exists", HttpStatusCode.Conflict);

            var newArea = mapper.Map<Area>(areaCreateDto);
            orderingContext.Areas.Add(newArea);
            await orderingContext.SaveChangesAsync();


            var areaReadDto = mapper.Map<AreaReadDto>(newArea);
            return ResultDto<AreaReadDto>.Success(areaReadDto, HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            return ResultDto<AreaReadDto>.Failure(
                $"An error occured: {ex.Message}",
                HttpStatusCode.BadRequest
            );
        }
    }

    public async Task<ResultDto<AreaReadDto>> GetArea(Guid id)
    {
        try
        {
            var area = await orderingContext
                .Areas.Include(a => a.Tables)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (area == null)
                return ResultDto<AreaReadDto>.Failure("Area not found", HttpStatusCode.NotFound);

            var areaReadDto = mapper.Map<AreaReadDto>(area);
            return ResultDto<AreaReadDto>.Success(areaReadDto, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<AreaReadDto>.Failure(
                $"An error occured: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ResultDto<List<AreaReadDto>>> GetAreas(DateTime? date = null)
    {
        try
        {
            if (date.HasValue)
            {
                var start = date.Value.Date;
                var end = start.AddDays(1);

                var filteredAreas = await orderingContext.Areas
                    .Include(a => a.Tables)
                        .ThenInclude(t => t.Reservations.Where(r => r.ScheduledFor >= start && r.ScheduledFor < end))
                    .AsNoTracking()
                    .ToListAsync();
                var filteredAreaReadDtos = mapper.Map<List<AreaReadDto>>(filteredAreas);
                return ResultDto<List<AreaReadDto>>.Success(filteredAreaReadDtos, HttpStatusCode.OK);
            }

            var areas = await orderingContext.Areas
                .Include(a => a.Tables)
                    .ThenInclude(t => t.Reservations)
                .AsNoTracking()
                .ToListAsync();

            var areaReadDtos = mapper.Map<List<AreaReadDto>>(areas);
            return ResultDto<List<AreaReadDto>>.Success(areaReadDtos, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<List<AreaReadDto>>.Failure(
                $"An error occured: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ResultDto<AreaReadDto>> UpdateArea(Guid id, AreaUpdateDto areaUpdateDto)
    {
        try
        {
            var area = await orderingContext.Areas.FindAsync(id);
            if (area == null)
                return ResultDto<AreaReadDto>.Failure("Area not found", HttpStatusCode.NotFound);

            mapper.Map(areaUpdateDto, area);

            await orderingContext.SaveChangesAsync();

            var areaReadDto = mapper.Map<AreaReadDto>(area);
            return ResultDto<AreaReadDto>.Success(areaReadDto, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<AreaReadDto>.Failure(
                $"An error occured: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ResultDto<bool>> DeleteArea(Guid id)
    {
        try
        {
            var area = await orderingContext.Areas.FindAsync(id);
            if (area == null)
                return ResultDto<bool>.Failure("Area not found", HttpStatusCode.NotFound);

            orderingContext.Areas.Remove(area);
            await orderingContext.SaveChangesAsync();

            return ResultDto<bool>.Success(true, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<bool>.Failure(
                $"An error occured: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }
}

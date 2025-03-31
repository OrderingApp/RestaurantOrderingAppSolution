using System.Net;
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
                HttpStatusCode.InternalServerError
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

    public async Task<ResultDto<List<AreaReadDto>>> GetAreas()
    {
        try
        {
            var areas = await orderingContext.Areas.Include(a => a.Tables).ToListAsync();

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

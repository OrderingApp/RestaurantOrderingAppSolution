using System.Net;
using Application.Contracts;
using Application.Dtos.Common;
using Application.Dtos.SubCategories;
using AutoMapper;
using Domain;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using RestaurantOrdering.Events.Application.Contracts;
using RestaurantOrdering.Events.Domain.SubCategories;

namespace Application.Services;

public class SubCategoryService(
    RestaurantOrderingContext orderingContext,
    IEventHandlerService eventHandlerService,
    IMapper mapper
) : ISubCategoryService
{
    public async Task<ResultDto<SubCategoryReadDto>> CreateSubCategory(
        SubCategoryCreateDto subCategoryCreateDto
    )
    {
        try
        {
            var menuCategoryExists = await orderingContext.MenuCategories.AnyAsync(mc =>
                mc.Id == subCategoryCreateDto.MenuCategoryId && !mc.IsDeleted
            );

            if (!menuCategoryExists)
                return ResultDto<SubCategoryReadDto>.Failure(
                    "Invalid MenuCategoryId. The referenced menu category does not exist.",
                    HttpStatusCode.BadRequest
                );

            var subCategory = mapper.Map<SubCategory>(subCategoryCreateDto);

            await orderingContext.SubCategories.AddAsync(subCategory);
            await orderingContext.SaveChangesAsync();

            var createdSubCategory = mapper.Map<SubCategoryReadDto>(subCategory);

            var subCategoryCreatedEvent = mapper.Map<SubCategoryCreatedEvent>(subCategory);
            await eventHandlerService.HandleEventAsync(subCategoryCreatedEvent);

            return ResultDto<SubCategoryReadDto>.Success(
                createdSubCategory,
                HttpStatusCode.Created
            );
        }
        catch (Exception ex)
        {
            return ResultDto<SubCategoryReadDto>.Failure(
                $"An error occured: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ResultDto<SubCategoryReadDto>> GetSubCategory(Guid id)
    {
        try
        {
            var subCategory = await orderingContext
                .SubCategories.AsNoTracking()
                .FirstOrDefaultAsync(sc => sc.Id == id);

            if (subCategory == null)
                return ResultDto<SubCategoryReadDto>.Failure(
                    "SubCategory not found.",
                    HttpStatusCode.NotFound
                );

            var subCategoryDto = mapper.Map<SubCategoryReadDto>(subCategory);

            return ResultDto<SubCategoryReadDto>.Success(subCategoryDto, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<SubCategoryReadDto>.Failure(
                $"An error occured: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ResultDto<List<SubCategoryReadDto>>> GetSubCategories()
    {
        try
        {
            var subCategories = await orderingContext
                .SubCategories.Where(mc => mc.IsUsed && !mc.IsDeleted)
                .AsNoTracking()
                .ToListAsync();

            var subCategoryDtos = mapper.Map<List<SubCategoryReadDto>>(subCategories);

            return ResultDto<List<SubCategoryReadDto>>.Success(subCategoryDtos, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<List<SubCategoryReadDto>>.Failure(
                $"An error occurred: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ResultDto<SubCategoryReadDto>> UpdateSubCategory(
        Guid id,
        SubCategoryUpdateDto subCategoryUpdateDto
    )
    {
        try
        {
            var subCategoryToUpdate = await orderingContext.SubCategories.FindAsync(id);

            if (subCategoryToUpdate == null)
                return ResultDto<SubCategoryReadDto>.Failure(
                    "subCategory not found or has been deleted.",
                    HttpStatusCode.NotFound
                );

            var menuCategoryExists = await orderingContext.MenuCategories.AnyAsync(mc =>
                mc.Id == subCategoryUpdateDto.MenuCategoryId && !mc.IsDeleted
            );

            if (!menuCategoryExists)
                return ResultDto<SubCategoryReadDto>.Failure(
                    "Invalid MenuCategoryId. The referenced menu category does not exist.",
                    HttpStatusCode.BadRequest
                );

            mapper.Map(subCategoryUpdateDto, subCategoryToUpdate);
            await orderingContext.SaveChangesAsync();

            var updatedSubCategory = mapper.Map<SubCategoryReadDto>(subCategoryToUpdate);

            var subCategoryUpdatedEvent = mapper.Map<SubCategoryUpdatedEvent>(subCategoryToUpdate);
            await eventHandlerService.HandleEventAsync(subCategoryUpdatedEvent);

            return ResultDto<SubCategoryReadDto>.Success(updatedSubCategory, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<SubCategoryReadDto>.Failure(
                $"An error occurred: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ResultDto<bool>> DeleteSubCategory(Guid id)
    {
        try
        {
            var subCategory = await orderingContext.SubCategories.FindAsync(id);

            if (subCategory == null)
                return ResultDto<bool>.Failure("subCategory not found.", HttpStatusCode.NotFound);

            if (subCategory.IsDeleted)
                return ResultDto<bool>.Failure(
                    "subCategory has already been deleted.",
                    HttpStatusCode.BadRequest
                );

            subCategory.IsDeleted = true;
            subCategory.IsUsed = false;

            await orderingContext.SaveChangesAsync();

            var subCategoryDeletedEvent = mapper.Map<SubCategoryDeletedEvent>(subCategory);
            await eventHandlerService.HandleEventAsync(subCategoryDeletedEvent);

            return ResultDto<bool>.Success(true, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<bool>.Failure(
                $"An error occurred: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }
}

using System.Net;
using Application.Contracts;
using Application.Dtos.Common;
using Application.Dtos.IngredientCategories;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using RestaurantOrdering.Events.Application.Contracts;

namespace Application.Services;

public class IngredientCategoryService(
    RestaurantOrderingContext orderingContext,
    IEventHandlerService eventHandlerService,
    IMapper mapper
) : IIngredientCategoryService
{
    public async Task<ResultDto<IngredientCategoryReadDto>> CreateIngredientCategory(IngredientCategoryCreateDto dto)
    {
        try
        {
            var entity = mapper.Map<IngredientCategory>(dto);
            await orderingContext.IngredientCategories.AddAsync(entity);
            await orderingContext.SaveChangesAsync();

            var read = mapper.Map<IngredientCategoryReadDto>(entity);

            // Event handling for ingredient category creation can be added later
            return ResultDto<IngredientCategoryReadDto>.Success(read, HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            return ResultDto<IngredientCategoryReadDto>.Failure(
                $"An error occurred: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ResultDto<List<IngredientCategoryReadDto>>> GetAllIngredientCategories()
    {
        try
        {
            var entities = await orderingContext.IngredientCategories
                .Where(ic => ic.IsUsed && !ic.IsDeleted)
                .Include(ic => ic.Ingredients)
                    .ThenInclude(i => i.IngredientTagRels)
                        .ThenInclude(rel => rel.Tag)
                .Include(ic => ic.Ingredients)
                    .ThenInclude(i => i.IngredientAllergenRels)
                        .ThenInclude(rel => rel.Allergen)
                .ToListAsync();

            if (!entities.Any())
                return ResultDto<List<IngredientCategoryReadDto>>.Failure("No ingredient categories found", HttpStatusCode.NotFound);

            var list = mapper.Map<List<IngredientCategoryReadDto>>(entities);
            return ResultDto<List<IngredientCategoryReadDto>>.Success(list, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<List<IngredientCategoryReadDto>>.Failure($"An error occurred: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ResultDto<IngredientCategoryReadDto>> GetIngredientCategory(Guid id)
    {
        try
        {
            var entity = await orderingContext.IngredientCategories
                .Where(ic => ic.Id == id && ic.IsUsed && !ic.IsDeleted)
                .Include(ic => ic.Ingredients)
                    .ThenInclude(i => i.IngredientTagRels)
                        .ThenInclude(rel => rel.Tag)
                .Include(ic => ic.Ingredients)
                    .ThenInclude(i => i.IngredientAllergenRels)
                        .ThenInclude(rel => rel.Allergen)
                .FirstOrDefaultAsync();

            if (entity == null)
                return ResultDto<IngredientCategoryReadDto>.Failure("Ingredient category not found.", HttpStatusCode.NotFound);

            var dto = mapper.Map<IngredientCategoryReadDto>(entity);
            return ResultDto<IngredientCategoryReadDto>.Success(dto, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<IngredientCategoryReadDto>.Failure($"An error occurred: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ResultDto<IngredientCategoryReadDto>> UpdateIngredientCategory(IngredientCategoryUpdateDto dto, Guid id)
    {
        try
        {
            var entity = await orderingContext.IngredientCategories.FindAsync(id);
            if (entity == null)
                return ResultDto<IngredientCategoryReadDto>.Failure("Ingredient category not found.", HttpStatusCode.NotFound);

            mapper.Map(dto, entity);
            await orderingContext.SaveChangesAsync();

            var updated = mapper.Map<IngredientCategoryReadDto>(entity);
            return ResultDto<IngredientCategoryReadDto>.Success(updated, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<IngredientCategoryReadDto>.Failure($"An error occurred: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ResultDto<bool>> DeleteIngredientCategory(Guid id)
    {
        try
        {
            var entity = await orderingContext.IngredientCategories.FindAsync(id);
            if (entity == null)
                return ResultDto<bool>.Failure("Ingredient category not found.", HttpStatusCode.NotFound);

            entity.IsDeleted = true;
            entity.IsUsed = false;
            await orderingContext.SaveChangesAsync();

            // Event handling for deletion can be added later
            return ResultDto<bool>.Success(true, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<bool>.Failure($"An error occurred: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }
}

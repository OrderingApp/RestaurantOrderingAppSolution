using System.Net;
using Application.Contracts;
using Application.Dtos.Common;
using Application.Dtos.Ingredients;
using Application.Dtos.IngredientCategories;
using AutoMapper;
using Domain;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using RestaurantOrdering.Events.Application.Contracts;
using RestaurantOrdering.Events.Domain.Ingredients;

namespace Application.Services;

public class IngredientService(
    RestaurantOrderingContext orderingContext,
    IEventHandlerService eventHandlerService,
    IMapper mapper
) : IIngredientService
{
    public async Task<ResultDto<IngredientReadDto>> CreateIngredient(
        IngredientCreateDto ingredientCreateDto
    )
    {
        try
        {
            var ingredient = mapper.Map<Ingredient>(ingredientCreateDto);

            // If a CategoryId was provided on DTO, it will be mapped to ingredient.CategoryId

            await orderingContext.Ingredients.AddAsync(ingredient);
            await orderingContext.SaveChangesAsync();

            var createdIngredient = await orderingContext.Ingredients
                .Include(i => i.IngredientTagRels)
                .ThenInclude(rel => rel.Tag)
                .Include(i => i.IngredientAllergenRels)
                .ThenInclude(rel => rel.Allergen)
                .Include(i => i.Category)
                .FirstOrDefaultAsync(i => i.Id == ingredient.Id);

            var createdDto = mapper.Map<IngredientReadDto>(createdIngredient);

            var ingredientCreatedEvent = mapper.Map<IngredientCreatedEvent>(ingredient);
            await eventHandlerService.HandleEventAsync(ingredientCreatedEvent);

            return ResultDto<IngredientReadDto>.Success(createdDto, HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            return ResultDto<IngredientReadDto>.Failure(
                $"An error occured: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    // To fix? create dto for tags?
    public async Task<ResultDto<List<IngredientReadDto>>> GetIngredients(List<string>? tags = null)
    {
        try
        {
            var query = orderingContext 
                .Ingredients.Include(i => i.IngredientTagRels)
                .ThenInclude(rel => rel.Tag)
                .Include(i => i.IngredientAllergenRels)
                .ThenInclude(rel => rel.Allergen)
                .Include(i => i.Category)
                .Where(i => i.CanBeUsedAsExtra && !i.IsDeleted)
                .AsQueryable();

            if (tags != null && tags.Any())
            {
                var lowerTags = tags.Select(tag => tag.ToLower()).ToList();

                query = query.Where(i =>
                    i.IngredientTagRels.Any(rel => lowerTags.Contains(rel.Tag.Name.ToLower()))
                );
            }

            var ingredients = await query.ToListAsync();
            var ingredientDtos = mapper.Map<List<IngredientReadDto>>(ingredients);

            return ResultDto<List<IngredientReadDto>>.Success(ingredientDtos, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<List<IngredientReadDto>>.Failure(
                $"An error occurred: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ResultDto<IngredientReadDto>> GetIngredient(Guid id)
    {
        try
        {
            var ingredient = await orderingContext.Ingredients.FirstOrDefaultAsync(i => i.Id == id);

            if (ingredient == null)
                return ResultDto<IngredientReadDto>.Failure(
                    "Ingredient not found.",
                    HttpStatusCode.NotFound
                );

            var ingredientDto = mapper.Map<IngredientReadDto>(ingredient);

            return ResultDto<IngredientReadDto>.Success(ingredientDto, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<IngredientReadDto>.Failure(
                $"An error occurred: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ResultDto<IngredientReadDto>> AddTagsToIngredient(Guid id, List<Guid> tagIds)
    {
        try
        {
            var ingredient = await orderingContext
                .Ingredients.Include(i => i.IngredientTagRels)
                .ThenInclude(rel => rel.Tag)
                .Include(i => i.IngredientAllergenRels)
                .ThenInclude(rel => rel.Allergen)
                .Include(i => i.Category)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (ingredient == null)
                return ResultDto<IngredientReadDto>.Failure(
                    "Ingredient not found.",
                    HttpStatusCode.NotFound
                );

            var existingTags = ingredient.IngredientTagRels.Select(rel => rel.TagId).ToList();
            var newTags = tagIds.Except(existingTags).ToList();

            foreach (var tagId in newTags)
            {
                orderingContext.IngredientTagRels.Add(
                    new IngredientTagRel { IngredientId = id, TagId = tagId }
                );
            }

            await orderingContext.SaveChangesAsync();

            var updatedIngredient = await orderingContext
                .Ingredients.Include(i => i.IngredientTagRels)
                .ThenInclude(rel => rel.Tag)
                .Include(i => i.IngredientAllergenRels)
                .ThenInclude(rel => rel.Allergen)
                .Include(i => i.Category)
                .FirstOrDefaultAsync(i => i.Id == id);

            var updatedIngredientDto = mapper.Map<IngredientReadDto>(updatedIngredient);
            return ResultDto<IngredientReadDto>.Success(updatedIngredientDto, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<IngredientReadDto>.Failure(
                $"An error occurred: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ResultDto<IngredientReadDto>> AddAllergensToIngredient(
        Guid id,
        List<Guid> allergenIds
    )
    {
        try
        {
            var ingredient = await orderingContext
                .Ingredients.Include(i => i.IngredientTagRels)
                .ThenInclude(rel => rel.Tag)
                .Include(i => i.IngredientAllergenRels)
                .ThenInclude(rel => rel.Allergen)
                .Include(i => i.Category)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (ingredient == null)
                return ResultDto<IngredientReadDto>.Failure(
                    "Ingredient not found.",
                    HttpStatusCode.NotFound
                );

            var existingAllergens = ingredient
                .IngredientAllergenRels.Select(rel => rel.AllergenId)
                .ToList();
            var newAllergens = allergenIds.Except(existingAllergens).ToList();

            foreach (var allergenId in newAllergens)
            {
                orderingContext.IngredientAllergenRels.Add(
                    new IngredientAllergenRel { IngredientId = id, AllergenId = allergenId }
                );
            }

            await orderingContext.SaveChangesAsync();

            var updatedIngredient = await orderingContext
                .Ingredients.Include(i => i.IngredientTagRels)
                .ThenInclude(rel => rel.Tag)
                .Include(i => i.IngredientAllergenRels)
                .ThenInclude(rel => rel.Allergen)
                .Include(i => i.Category)
                .FirstOrDefaultAsync(i => i.Id == id);

            var updatedIngredientDto = mapper.Map<IngredientReadDto>(updatedIngredient);
            return ResultDto<IngredientReadDto>.Success(updatedIngredientDto, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<IngredientReadDto>.Failure(
                $"An error occurred: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ResultDto<IngredientReadDto>> UpdateIngredient(
        Guid id,
        IngredientUpdateDto ingredientUpdateDto
    )
    {
        try
        {
            var ingredient = await orderingContext.Ingredients.FindAsync(id);

            if (ingredient == null)
                return ResultDto<IngredientReadDto>.Failure(
                    "Ingredient not found.",
                    HttpStatusCode.NotFound
                );

            mapper.Map(ingredientUpdateDto, ingredient);
            await orderingContext.SaveChangesAsync();

            var updatedIngredientDto = mapper.Map<IngredientReadDto>(ingredient);

            var ingredientUpdatedEvent = mapper.Map<IngredientUpdatedEvent>(ingredient);
            await eventHandlerService.HandleEventAsync(ingredientUpdatedEvent);

            return ResultDto<IngredientReadDto>.Success(updatedIngredientDto, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<IngredientReadDto>.Failure(
                $"An error occurred: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ResultDto<bool>> DeleteIngredient(Guid id)
    {
        try
        {
            var ingredient = await orderingContext
                .Ingredients.Include(i => i.MenuItemIngredientRels)
                .Include(i => i.IngredientTagRels)
                .Include(i => i.IngredientAllergenRels)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (ingredient == null)
                return ResultDto<bool>.Failure("Ingredient not found.", HttpStatusCode.NotFound);

            ingredient.IsDeleted = true;
            ingredient.CanBeUsedAsExtra = false;

            orderingContext.MenuItemIngredientRels.RemoveRange(ingredient.MenuItemIngredientRels);
            orderingContext.IngredientTagRels.RemoveRange(ingredient.IngredientTagRels);
            orderingContext.IngredientAllergenRels.RemoveRange(ingredient.IngredientAllergenRels);

            await orderingContext.SaveChangesAsync();

            return ResultDto<bool>.Success(true, HttpStatusCode.NoContent);
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

using System.Net;
using Application.Contracts;
using Application.Dtos.Common;
using Application.Dtos.Ingredients;
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

            await orderingContext.Ingredients.AddAsync(ingredient);
            await orderingContext.SaveChangesAsync();

            var createdIngredient = mapper.Map<IngredientReadDto>(ingredient);

            var ingredientCreatedEvent = mapper.Map<IngredientCreatedEvent>(ingredient);
            await eventHandlerService.HandleEventAsync(ingredientCreatedEvent);

            return ResultDto<IngredientReadDto>.Success(createdIngredient, HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            return ResultDto<IngredientReadDto>.Failure(
                $"An error occured: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ResultDto<List<IngredientReadDto>>> GetIngredients(List<string>? tags = null)
    {
        try
        {
            var query = orderingContext
                .Ingredients.Include(i => i.IngredientTagRels)
                .ThenInclude(rel => rel.Tag)
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
                .FirstOrDefaultAsync(i => i.Id == id);

            if (ingredient == null)
                return ResultDto<bool>.Failure("Ingredient not found.", HttpStatusCode.NotFound);

            ingredient.IsDeleted = true;
            ingredient.CanBeUsedAsExtra = false;

            orderingContext.MenuItemIngredientRels.RemoveRange(ingredient.MenuItemIngredientRels);
            orderingContext.IngredientTagRels.RemoveRange(ingredient.IngredientTagRels);

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

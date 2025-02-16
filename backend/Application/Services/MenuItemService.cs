using Application.Contracts;
using Application.Dtos.Common;
using Application.Dtos.MenuItems;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using RestaurantOrdering.Events.Application.Contracts;
using RestaurantOrdering.Events.Domain.MenuItems;
using System.Net;

namespace Application.Services;

public class MenuItemService(RestaurantOrderingContext orderingContext, IEventHandlerService eventHandlerService, IMapper mapper) : IMenuItemService
{
    public async Task<ResultDto<MenuItemReadDto>> CreateMenuItem(MenuItemCreateDto menuItemCreateDto)
    {
        try
        {
            var menuItem = mapper.Map<MenuItem>(menuItemCreateDto);

            if (menuItemCreateDto.IngredientIds.Any())
            {
                var ingredients = await orderingContext.Ingredients
                    .Where(i => menuItemCreateDto.IngredientIds.Contains(i.Id))
                    .ToListAsync();

                menuItem.MenuItemIngredientRels = ingredients.Select(ingredient => new MenuItemIngredientRel
                {
                    MenuItemId = menuItem.Id,
                    IngredientId = ingredient.Id
                }).ToList();
            }

            await orderingContext.MenuItems.AddAsync(menuItem);
            await orderingContext.SaveChangesAsync();

            var createdMenuItem = mapper.Map<MenuItemReadDto>(menuItem);

            var menuItemCreatedEvent = mapper.Map<MenuItemCreatedEvent>(menuItem);
            await eventHandlerService.HandleEventAsync(menuItemCreatedEvent);

            return ResultDto<MenuItemReadDto>
                .Success(createdMenuItem, HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            return ResultDto<MenuItemReadDto>
                .Failure($"An error occurred: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ResultDto<MenuItemReadDto>> GetMenuItem(Guid id)
    {
        try
        {
            var menuItem = await orderingContext.MenuItems
                .Include(mi => mi.MenuCategory)
                .Include(mi => mi.MenuItemIngredientRels)
                    .ThenInclude(rel => rel.Ingredient)
                        .ThenInclude(ing => ing.IngredientTagRels)
                            .ThenInclude(tagRel => tagRel.Tag)
                .FirstOrDefaultAsync(mi => mi.Id == id);

            if (menuItem == null)
                return ResultDto<MenuItemReadDto>
                    .Failure("Menu item not found.", HttpStatusCode.NotFound);

            var menuItemDto = mapper.Map<MenuItemReadDto>(menuItem);

            return ResultDto<MenuItemReadDto>
                .Success(menuItemDto, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<MenuItemReadDto>
                .Failure($"An error occurred while fetching the menu item: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ResultDto<List<MenuItemReadDto>>> GetFilteredMenuItems(Guid? categoryId = null, List<Guid>? ingredientIds = null, List<string>? tags = null)
    {
        try
        {
            var query = orderingContext.MenuItems
                .Where(mi => mi.IsUsed && !mi.IsDeleted)
                .AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(mi => mi.MenuCategoryId == categoryId.Value);
            }

            if (ingredientIds != null && ingredientIds.Any())
            {
                query = query.Where(mi => mi.MenuItemIngredientRels
                    .Any(rel => ingredientIds.Contains(rel.IngredientId)));
            }

            if (tags != null && tags.Any())
            {
                var lowerTags = tags.Select(tag => tag.ToLower()).ToList();
                query = query.Where(mi => mi.MenuItemIngredientRels
                    .Any(rel => rel.Ingredient.IngredientTagRels
                        .Any(tagRel => lowerTags.Contains(tagRel.Tag.Name.ToLower()))));
            }

            var menuItems = await query
                .ProjectTo<MenuItemReadDto>(mapper.ConfigurationProvider)
                .ToListAsync();

            return ResultDto<List<MenuItemReadDto>>.Success(menuItems, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<List<MenuItemReadDto>>.Failure($"An error occurred: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }


    public async Task<ResultDto<MenuItemReadDto>> UpdateMenuItem(Guid id, MenuItemUpdateDto menuItemUpdateDto)
    {
        try
        {
            var menuItem = await orderingContext.MenuItems
                .Include(mi => mi.MenuItemIngredientRels)
                .FirstOrDefaultAsync(mi => mi.Id == id);

            if (menuItem == null)
                return ResultDto<MenuItemReadDto>
                    .Failure("Menu item not found.", HttpStatusCode.NotFound);

            mapper.Map(menuItemUpdateDto, menuItem);

            if (menuItemUpdateDto.IngredientIds?.Any() == true)
            {
                orderingContext.MenuItemIngredientRels.RemoveRange(menuItem.MenuItemIngredientRels);

                var ingredients = await orderingContext.Ingredients
                    .Where(i => menuItemUpdateDto.IngredientIds.Contains(i.Id))
                    .ToListAsync();

                menuItem.MenuItemIngredientRels = ingredients.Select(ingredient => new MenuItemIngredientRel
                {
                    MenuItemId = menuItem.Id,
                    IngredientId = ingredient.Id
                }).ToList();
            }

            await orderingContext.SaveChangesAsync();

            var updatedMenuItemDto = mapper.Map<MenuItemReadDto>(menuItem);

            var menuItemUpdatedEvent = mapper.Map<MenuItemUpdatedEvent>(menuItem);
            await eventHandlerService.HandleEventAsync(menuItemUpdatedEvent);

            return ResultDto<MenuItemReadDto>
                .Success(updatedMenuItemDto, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<MenuItemReadDto>
                .Failure($"An error occurred: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ResultDto<bool>> DeleteMenuItem(Guid id)
    {
        try
        {
            var menuItemToDelete = await orderingContext.MenuItems.FindAsync(id);

            if (menuItemToDelete == null)
                return ResultDto<bool>
                    .Failure("Menu item not found.", HttpStatusCode.NotFound);

            if (menuItemToDelete.IsDeleted)
                return ResultDto<bool>
                    .Failure("MenuItem has already been deleted.", HttpStatusCode.BadRequest);

            menuItemToDelete.IsDeleted = true;
            menuItemToDelete.IsUsed = false;

            await orderingContext.SaveChangesAsync();

            var menuItemDeletedEvent = mapper.Map<MenuItemDeletedEvent>(menuItemToDelete);
            await eventHandlerService.HandleEventAsync(menuItemDeletedEvent);

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

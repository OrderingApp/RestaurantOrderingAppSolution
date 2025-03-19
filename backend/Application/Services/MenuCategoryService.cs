using Application.Contracts;
using Application.Dtos.Common;
using Application.Dtos.MenuCategories;
using AutoMapper;
using Domain;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using RestaurantOrdering.Events.Application.Contracts;
using RestaurantOrdering.Events.Domain.MenuCategories;
using System.Net;

namespace Application.Services;

public class MenuCategoryService(RestaurantOrderingContext orderingContext, IEventHandlerService eventHandlerService, IMapper mapper) : IMenuCategoryService
{
    public async Task<ResultDto<MenuCategoryReadDto>> CreateMenuCategory(MenuCategoryCreateDto menuCategoryCreateDto)
    {
        try
        {
            var menuCategory = mapper.Map<MenuCategory>(menuCategoryCreateDto);

            await orderingContext.MenuCategories.AddAsync(menuCategory);
            await orderingContext.SaveChangesAsync();

            var createdMenuCategory = mapper.Map<MenuCategoryReadDto>(menuCategory);

            var menuCategoryCreatedEvent = mapper.Map<MenuCategoryCreatedEvent>(menuCategory);
            await eventHandlerService.HandleEventAsync(menuCategoryCreatedEvent);

            return ResultDto<MenuCategoryReadDto>
                .Success(createdMenuCategory, HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            return ResultDto<MenuCategoryReadDto>
                .Failure($"An error occurred: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ResultDto<MenuCategoryReadDto>> GetMenuCategory(Guid id)
    {
        try
        {
            var menuCategory = await orderingContext.MenuCategories
                .FirstOrDefaultAsync(mc => mc.Id == id);

            if (menuCategory == null)
                return ResultDto<MenuCategoryReadDto>
                    .Failure("MenuCategory not found.", HttpStatusCode.NotFound);

            var menuCategoryDto = mapper.Map<MenuCategoryReadDto>(menuCategory);

            return ResultDto<MenuCategoryReadDto>
                .Success(menuCategoryDto, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<MenuCategoryReadDto>
                .Failure($"An error occurred: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ResultDto<List<MenuCategoryReadDto>>> GetMenuCategories()
    {
        try
        {
            var menuCategories = await orderingContext.MenuCategories
                .Where(mc => mc.IsUsed && !mc.IsDeleted)
                .Include(mc => mc.SubCategories)
                .ToListAsync();

            var menuCategoryDtos = mapper.Map<List<MenuCategoryReadDto>>(menuCategories);

            return ResultDto<List<MenuCategoryReadDto>>
                .Success(menuCategoryDtos, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<List<MenuCategoryReadDto>>
                .Failure($"An error occurred: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<PagedResultDto<MenuCategoryHierarchyReadDto>> GetMenuCategoriesWithHierarchy(GetMenuCategoryHierarchyRequest request)
    {
        try
        {
            var query = orderingContext.MenuCategories
                .Where(mc => mc.IsUsed && !mc.IsDeleted)
                .Include(mc => mc.SubCategories.OrderBy(sc => sc.SequenceNumber))
                .Include(mc => mc.MenuItems.OrderBy(mi => mi.SequenceNumber))
                    .ThenInclude(mi => mi.MenuItemIngredientRels)
                        .ThenInclude(mii => mii.Ingredient)
                            .ThenInclude(i => i.IngredientTagRels)
                                .ThenInclude(it => it.Tag)
                .OrderBy(mc => mc.SequenceNumber)
                .AsQueryable();

            if (request.MenuCategoryId.HasValue)
            {
                query = query.Where(mc => mc.Id == request.MenuCategoryId.Value);
            }
            if (request.SubCategoryId.HasValue)
            {
                query = query.Where(mc => mc.SubCategories.Any(sc => sc.Id == request.SubCategoryId.Value));
            }

            if (request.TagIds != null && request.TagIds.Any())
            {
                query = query.Where(mc => mc.MenuItems
                    .Any(mi => mi.MenuItemIngredientRels
                        .Any(mii => mii.Ingredient.IngredientTagRels
                            .Any(it => request.TagIds.Contains(it.TagId)))));
            }

            var totalItems = await query.CountAsync();

            bool pageLimit = false;

            if (pageLimit)
            {
                if (request.Page.HasValue && request.PageSize.HasValue)
                {
                    request.Page = Math.Max(request.Page.Value, 0);
                    request.PageSize = Math.Clamp(request.PageSize.Value, 1, 100);

                    query = query
                        .Skip(request.Page.Value * request.PageSize.Value)
                        .Take(request.PageSize.Value);
                }
            }

            var menuCategories = await query.ToListAsync();

            var menuCategoryDtos = mapper.Map<List<MenuCategoryHierarchyReadDto>>(menuCategories);

            return new PagedResultDto<MenuCategoryHierarchyReadDto>(
                menuCategoryDtos,
                totalItems,
                request.Page ?? 0,
                request.PageSize ?? totalItems
            );

        }
        catch (Exception ex)
        {
            return new PagedResultDto<MenuCategoryHierarchyReadDto>(
                new List<MenuCategoryHierarchyReadDto>(),
                0,
                0,
                0
            )
            {
                IsSuccess = false,
                ErrorMessage = $"An error occurred: {ex.Message}",
                HttpStatusCode = HttpStatusCode.InternalServerError
            };
        }
    }

    public async Task<ResultDto<MenuCategoryReadDto>> UpdateMenuCategory(Guid id, MenuCategoryUpdateDto menuCategoryUpdateDto)
    {
        try
        {
            var menuCategoryToUpdate = await orderingContext.MenuCategories.FindAsync(id);

            if (menuCategoryToUpdate == null)
                return ResultDto<MenuCategoryReadDto>
                    .Failure("MenuCategory not found or has been deleted.", HttpStatusCode.NotFound);

            mapper.Map(menuCategoryUpdateDto, menuCategoryToUpdate);
            await orderingContext.SaveChangesAsync();

            var updatedMenuCategory = mapper.Map<MenuCategoryReadDto>(menuCategoryToUpdate);

            var menuCategoryUpdatedEvent = mapper.Map<MenuCategoryUpdatedEvent>(menuCategoryToUpdate);
            await eventHandlerService.HandleEventAsync(menuCategoryUpdatedEvent);

            return ResultDto<MenuCategoryReadDto>
                .Success(updatedMenuCategory, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<MenuCategoryReadDto>
                .Failure($"An error occurred: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ResultDto<bool>> DeleteMenuCategory(Guid id)
    {
        try
        {
            var menuCategory = await orderingContext.MenuCategories.FindAsync(id);

            if (menuCategory == null)
                return ResultDto<bool>
                    .Failure("MenuCategory not found.", HttpStatusCode.NotFound);

            if (menuCategory.IsDeleted)
                return ResultDto<bool>
                    .Failure("MenuCategory has already been deleted.", HttpStatusCode.BadRequest);

            menuCategory.IsDeleted = true;
            menuCategory.IsUsed = false;

            await orderingContext.SaveChangesAsync();

            var menuCategoryDeletedEvent = mapper.Map<MenuCategoryDeletedEvent>(menuCategory);
            await eventHandlerService.HandleEventAsync(menuCategoryDeletedEvent);

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

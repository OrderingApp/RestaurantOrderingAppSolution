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

    public async Task<ResultDto<List<MenuCategoryReadDto>>> GetAllMenuCategories()
    {
        try
        {
            var menuCategories = await orderingContext.MenuCategories
                .Include(mc => mc.MenuItems)
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

    public async Task<ResultDto<MenuCategoryReadDto>> GetMenuCategory(Guid id)
    {
        try
        {
            var menuCategory = await orderingContext.MenuCategories
                .Include(mc => mc.MenuItems)
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

    public async Task<ResultDto<MenuCategoryReadDto>> UpdateMenuCategory(MenuCategoryUpdateDto menuCategoryUpdateDto, Guid id)
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

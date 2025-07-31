using System.Net;
using Application.Contracts;
using Application.Dtos.Common;
using Application.Dtos.MenuCategories;
using AutoMapper;
using Domain;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using RestaurantOrdering.Events.Application.Contracts;
using RestaurantOrdering.Events.Domain.MenuCategories;

namespace Application.Services;

public class MenuCategoryService(
    RestaurantOrderingContext orderingContext,
    IEventHandlerService eventHandlerService,
    IMapper mapper
) : IMenuCategoryService
{
    public async Task<ResultDto<MenuCategoryReadDto>> CreateMenuCategory(
        MenuCategoryCreateDto menuCategoryCreateDto
    )
    {
        try
        {
            int sequenceNumber;

            if (menuCategoryCreateDto.SequenceNumber.HasValue)
            {
                var sequenceResult = await InsertMenuCategoryIntoSequenceAsync(null, menuCategoryCreateDto.SequenceNumber.Value);
                if (!sequenceResult.IsSuccess)
                {
                    return ResultDto<MenuCategoryReadDto>.Failure(sequenceResult.ErrorMessage!, sequenceResult.HttpStatusCode);
                }

                sequenceNumber = sequenceResult.Data!;
            }
            else
            {
                // Get the next available position (no gaps)
                var maxSequence = await orderingContext.MenuCategories
                    .Where(c => !c.IsDeleted)
                    .MaxAsync(c => (int?)c.SequenceNumber) ?? 0;

                sequenceNumber = maxSequence + 1;
            }

            var menuCategory = mapper.Map<MenuCategory>(menuCategoryCreateDto);
            menuCategory.SequenceNumber = sequenceNumber;

            await orderingContext.MenuCategories.AddAsync(menuCategory);
            await orderingContext.SaveChangesAsync();

            var createdMenuCategory = mapper.Map<MenuCategoryReadDto>(menuCategory);
            var menuCategoryCreatedEvent = mapper.Map<MenuCategoryCreatedEvent>(menuCategory);
            await eventHandlerService.HandleEventAsync(menuCategoryCreatedEvent);

            return ResultDto<MenuCategoryReadDto>.Success(createdMenuCategory, HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            return ResultDto<MenuCategoryReadDto>.Failure(
                $"An error occurred: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }



    public async Task<ResultDto<MenuCategoryReadDto>> GetMenuCategory(Guid id)
    {
        try
        {
            var menuCategory = await orderingContext.MenuCategories.FirstOrDefaultAsync(mc =>
                mc.Id == id
            );

            if (menuCategory == null)
                return ResultDto<MenuCategoryReadDto>.Failure(
                    "MenuCategory not found.",
                    HttpStatusCode.NotFound
                );

            var menuCategoryDto = mapper.Map<MenuCategoryReadDto>(menuCategory);

            return ResultDto<MenuCategoryReadDto>.Success(menuCategoryDto, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<MenuCategoryReadDto>.Failure(
                $"An error occurred: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ResultDto<List<MenuCategoryReadDto>>> GetMenuCategories()
    {
        try
        {
            var menuCategories = await orderingContext
                .MenuCategories.Where(mc => mc.IsUsed && !mc.IsDeleted)
                .Include(mc => mc.SubCategories)
                .OrderBy(mc => mc.SequenceNumber)
                .ToListAsync();

            var menuItems = await orderingContext
                .MenuItems.Where(mi => mi.IsUsed && !mi.IsDeleted)
                .ToListAsync();

            foreach (var category in menuCategories)
            {
                category.SubCategories = category
                    .SubCategories.OrderBy(sc => sc.SequenceNumber)
                    .ToList();
            }

            var menuCategoryDtos = mapper.Map<List<MenuCategoryReadDto>>(menuCategories);

            foreach (var categoryDto in menuCategoryDtos)
            {
                categoryDto.TotalItems = menuItems.Count(mi => mi.MenuCategoryId == categoryDto.Id);

                foreach (var subCategoryDto in categoryDto.SubCategories)
                {
                    subCategoryDto.TotalItems = menuItems.Count(mi =>
                        mi.SubCategoryId == subCategoryDto.Id
                    );
                }
            }

            return ResultDto<List<MenuCategoryReadDto>>.Success(
                menuCategoryDtos,
                HttpStatusCode.OK
            );
        }
        catch (Exception ex)
        {
            return ResultDto<List<MenuCategoryReadDto>>.Failure(
                $"An error occurred: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    // Check if you need both methods
    public async Task<PagedResultDto<MenuCategoryHierarchyReadDto>> GetMenuCategoriesWithHierarchy(
        GetMenuCategoryHierarchyRequest request
    )
    {
        try
        {
            var query = orderingContext
                .MenuCategories.Where(mc => mc.IsUsed && !mc.IsDeleted)
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
                query = query.Where(mc =>
                    mc.SubCategories.Any(sc => sc.Id == request.SubCategoryId.Value)
                );
            }

            if (request.TagIds != null && request.TagIds.Any())
            {
                query = query.Where(mc =>
                    mc.MenuItems.Any(mi =>
                        mi.MenuItemIngredientRels.Any(mii =>
                            mii.Ingredient.IngredientTagRels.Any(it =>
                                request.TagIds.Contains(it.TagId)
                            )
                        )
                    )
                );
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
                HttpStatusCode = HttpStatusCode.InternalServerError,
            };
        }
    }

    public async Task<ResultDto<MenuCategoryReadDto>> UpdateMenuCategory(
        Guid id,
        MenuCategoryUpdateDto menuCategoryUpdateDto
    )
    {
        try
        {
            var menuCategoryToUpdate = await orderingContext.MenuCategories.FindAsync(id);

            if (menuCategoryToUpdate == null)
                return ResultDto<MenuCategoryReadDto>.Failure(
                    "MenuCategory not found or has been deleted.",
                    HttpStatusCode.NotFound
                );


            // Handle sequence change if requested
            if (menuCategoryUpdateDto.SequenceNumber.HasValue)
            {
                var sequenceResult = await InsertMenuCategoryIntoSequenceAsync(
                    id,
                    menuCategoryUpdateDto.SequenceNumber.Value
                );

                if (!sequenceResult.IsSuccess)
                {
                    return ResultDto<MenuCategoryReadDto>.Failure(
                        sequenceResult.ErrorMessage!,
                        HttpStatusCode.BadRequest
                    );
                }

                menuCategoryToUpdate.SequenceNumber = sequenceResult.Data!;
            }

            // Handle other fields
            mapper.Map(menuCategoryUpdateDto, menuCategoryToUpdate);

            await orderingContext.SaveChangesAsync();

            var updatedMenuCategory = mapper.Map<MenuCategoryReadDto>(menuCategoryToUpdate);
            var menuCategoryUpdatedEvent = mapper.Map<MenuCategoryUpdatedEvent>(menuCategoryToUpdate);
            await eventHandlerService.HandleEventAsync(menuCategoryUpdatedEvent);

            return ResultDto<MenuCategoryReadDto>.Success(updatedMenuCategory, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<MenuCategoryReadDto>.Failure(
                $"An error occurred: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ResultDto<bool>> DeleteMenuCategory(Guid id)
    {
        try
        {
            var menuCategory = await orderingContext.MenuCategories.FindAsync(id);

            if (menuCategory == null)
                return ResultDto<bool>.Failure("MenuCategory not found.", HttpStatusCode.NotFound);

            if (menuCategory.IsDeleted)
                return ResultDto<bool>.Failure(
                    "MenuCategory has already been deleted.",
                    HttpStatusCode.BadRequest
                );

            menuCategory.IsDeleted = true;
            menuCategory.IsUsed = false;

            await orderingContext.SaveChangesAsync();

            await NormalizeMenuCategorySequenceAsync();

            var menuCategoryDeletedEvent = mapper.Map<MenuCategoryDeletedEvent>(menuCategory);
            await eventHandlerService.HandleEventAsync(menuCategoryDeletedEvent);

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

    private async Task<ResultDto<int>> InsertMenuCategoryIntoSequenceAsync(
    Guid? updatingId, // null for Create, actual Id for Update
    int requestedSequence
)
    {
        if (requestedSequence < 1)
        {
            return ResultDto<int>.Failure("SequenceNumber must be 1 or greater.", HttpStatusCode.BadRequest);
        }

        var activeCategories = await orderingContext.MenuCategories
            .Where(c => !c.IsDeleted && (!updatingId.HasValue || c.Id != updatingId.Value))
            .OrderBy(c => c.SequenceNumber)
            .ToListAsync();

        int maxAllowed = activeCategories.Count + 1;

        if (requestedSequence > maxAllowed)
        {
            return ResultDto<int>.Failure(
                $"Invalid SequenceNumber. The allowed range is 1 to {maxAllowed}.",
                HttpStatusCode.BadRequest
            );
        }

        // Insert placeholder or updated category into list
        activeCategories.Insert(requestedSequence - 1, null!); // null as placeholder

        // Reassign 1..N
        for (int i = 0; i < activeCategories.Count; i++)
        {
            if (activeCategories[i] != null)
                activeCategories[i].SequenceNumber = i + 1;
        }

        // Save shifted categories
        await orderingContext.SaveChangesAsync();

        // Return the correct sequence number for the new/updated item
        return ResultDto<int>.Success(requestedSequence, HttpStatusCode.OK);
    }


    private async Task NormalizeMenuCategorySequenceAsync()
    {
        var activeCategories = await orderingContext.MenuCategories
            .Where(c => !c.IsDeleted) // only normalize active ones
            .OrderBy(c => c.SequenceNumber)
            .ToListAsync();

        int current = 1;
        foreach (var category in activeCategories)
        {
            category.SequenceNumber = current++;
        }

        await orderingContext.SaveChangesAsync();
    }

}

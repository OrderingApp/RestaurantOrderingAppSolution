using System.Net;
using Application.Contracts;
using Application.Dtos.Allergens;
using Application.Dtos.Common;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using RestaurantOrdering.Events.Application.Contracts;
using RestaurantOrdering.Events.Domain.Allergens;

namespace Application.Services;

public class AllergenService(
    RestaurantOrderingContext orderingContext,
    IEventHandlerService eventHandlerService,
    IMapper mapper
) : IAllergenService
{
    public async Task<ResultDto<AllergenReadDto>> CreateAllergen(
        AllergenCreateDto allergenCreateDto
    )
    {
        try
        {
            var allergen = mapper.Map<Allergen>(allergenCreateDto);

            await orderingContext.Allergens.AddAsync(allergen);
            await orderingContext.SaveChangesAsync();

            var createdAllergen = mapper.Map<AllergenReadDto>(allergen);

            var allergenCreatedEvent = mapper.Map<AllergenCreatedEvent>(allergen);
            await eventHandlerService.HandleEventAsync(allergenCreatedEvent);

            return ResultDto<AllergenReadDto>.Success(createdAllergen, HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            return ResultDto<AllergenReadDto>.Failure(
                $"An error occurred: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ResultDto<List<AllergenReadDto>>> GetAllAllergens()
    {
        try
        {
            var allergens = await orderingContext
                .Allergens.Where(a => a.IsUsed && !a.IsDeleted)
                .ProjectTo<AllergenReadDto>(mapper.ConfigurationProvider)
                .ToListAsync();

            if (!allergens.Any())
            {
                return ResultDto<List<AllergenReadDto>>.Failure(
                    "No allergens found",
                    HttpStatusCode.NotFound
                );
            }

            return ResultDto<List<AllergenReadDto>>.Success(allergens, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<List<AllergenReadDto>>.Failure(
                $"An error occurred: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ResultDto<AllergenReadDto>> GetAllergen(Guid id)
    {
        try
        {
            var allergen = await orderingContext.Allergens.FirstOrDefaultAsync(a => a.Id == id);

            if (allergen == null)
                return ResultDto<AllergenReadDto>.Failure(
                    "Allergen not found.",
                    HttpStatusCode.NotFound
                );

            var allergenDto = mapper.Map<AllergenReadDto>(allergen);

            return ResultDto<AllergenReadDto>.Success(allergenDto, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<AllergenReadDto>.Failure(
                $"An error occurred: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ResultDto<AllergenReadDto>> UpdateAllergen(
        AllergenUpdateDto allergenUpdateDto,
        Guid id
    )
    {
        try
        {
            var allergen = await orderingContext.Allergens.FindAsync(id);

            if (allergen == null)
                return ResultDto<AllergenReadDto>.Failure(
                    "Allergen not found.",
                    HttpStatusCode.NotFound
                );

            mapper.Map(allergenUpdateDto, allergen);
            await orderingContext.SaveChangesAsync();

            var updatedAllergen = mapper.Map<AllergenReadDto>(allergen);

            var allergenUpdatedEvent = mapper.Map<AllergenUpdatedEvent>(allergen);
            await eventHandlerService.HandleEventAsync(allergenUpdatedEvent);

            return ResultDto<AllergenReadDto>.Success(updatedAllergen, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<AllergenReadDto>.Failure(
                $"An error occurred: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ResultDto<bool>> DeleteAllergen(Guid id)
    {
        try
        {
            var allergen = await orderingContext.Allergens.FindAsync(id);

            if (allergen == null)
                return ResultDto<bool>.Failure("Allergen not found.", HttpStatusCode.NotFound);

            allergen.IsDeleted = true;
            allergen.IsUsed = false;
            await orderingContext.SaveChangesAsync();

            var allergenDeletedEvent = mapper.Map<AllergenDeletedEvent>(allergen);
            await eventHandlerService.HandleEventAsync(allergenDeletedEvent);

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

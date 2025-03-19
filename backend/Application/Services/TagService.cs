using Application.Contracts;
using Application.Dtos.Common;
using Application.Dtos.Tags;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using RestaurantOrdering.Events.Application.Contracts;
using RestaurantOrdering.Events.Domain.Tags;
using System.Net;

namespace Application.Services;

public class TagService(RestaurantOrderingContext orderingContext, IEventHandlerService eventHandlerService, IMapper mapper) : ITagService
{
    public async Task<ResultDto<TagReadDto>> CreateTag(TagCreateDto tagCreateDto)
    {
        try
        {
            var tag = mapper.Map<Tag>(tagCreateDto);

            await orderingContext.Tags.AddAsync(tag);
            await orderingContext.SaveChangesAsync();

            var createdTag = mapper.Map<TagReadDto>(tag);

            var tagCreatedEvent = mapper.Map<TagCreatedEvent>(tag);
            await eventHandlerService.HandleEventAsync(tagCreatedEvent);

            return ResultDto<TagReadDto>
                .Success(createdTag, HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            return ResultDto<TagReadDto>
                .Failure($"An error occurred: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ResultDto<List<TagReadDto>>> GetAllTags()
    {
        try
        {
            var tags = await orderingContext.Tags
                        .Where(t => t.IsUsed && !t.IsDeleted)
                        .ProjectTo<TagReadDto>(mapper.ConfigurationProvider)
                        .ToListAsync();

            if (!tags.Any())
            {
                return ResultDto<List<TagReadDto>>
                    .Failure("No tags found", HttpStatusCode.NotFound);
            }

            return ResultDto<List<TagReadDto>>.Success(tags, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<List<TagReadDto>>
                .Failure($"An error occurred: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ResultDto<TagReadDto>> GetTag(Guid id)
    {
        try
        {
            var tag = await orderingContext.Tags
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tag == null)
                return ResultDto<TagReadDto>
                    .Failure("Tag not found.", HttpStatusCode.NotFound);

            var tagDto = mapper.Map<TagReadDto>(tag);

            return ResultDto<TagReadDto>
                .Success(tagDto, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<TagReadDto>
                .Failure($"An error occurred: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ResultDto<TagReadDto>> UpdateTag(TagUpdateDto tagUpdateDto, Guid id)
    {
        try
        {
            var tag = await orderingContext.Tags.FindAsync(id);

            if (tag == null)
                return ResultDto<TagReadDto>
                    .Failure("Tag not found.", HttpStatusCode.NotFound);

            mapper.Map(tagUpdateDto, tag);
            await orderingContext.SaveChangesAsync();

            var updatedTag = mapper.Map<TagReadDto>(tag);

            var tagUpdatedEvent = mapper.Map<TagUpdatedEvent>(tag);
            await eventHandlerService.HandleEventAsync(tagUpdatedEvent);

            return ResultDto<TagReadDto>
                .Success(updatedTag, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<TagReadDto>
                .Failure($"An error occurred: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ResultDto<bool>> DeleteTag(Guid id)
    {
        try
        {
            var tag = await orderingContext.Tags.FindAsync(id);

            if (tag == null)
                return ResultDto<bool>
                    .Failure("Tag not found.", HttpStatusCode.NotFound);

            tag.IsDeleted = true;
            tag.IsUsed = false;
            await orderingContext.SaveChangesAsync();

            var tagDeletedEvent = mapper.Map<TagDeletedEvent>(tag);
            await eventHandlerService.HandleEventAsync(tagDeletedEvent);

            return ResultDto<bool>.Success(true, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<bool>
                .Failure($"An error occurred: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }
}

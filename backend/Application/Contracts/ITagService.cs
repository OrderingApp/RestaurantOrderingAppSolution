using Application.Dtos.Common;
using Application.Dtos.Tags;

namespace Application.Contracts;

public interface ITagService
{
    Task<ResultDto<TagReadDto>> CreateTag(TagCreateDto tagCreateDto);
    Task<ResultDto<List<TagReadDto>>> GetAllTags();
    Task<ResultDto<TagReadDto>> GetTag(Guid id);
    Task<ResultDto<TagReadDto>> UpdateTag(TagUpdateDto tagUpdateDto, Guid id);
    Task<ResultDto<bool>> DeleteTag(Guid id);
}
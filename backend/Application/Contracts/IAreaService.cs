using Application.Dtos.Areas;
using Application.Dtos.Common;

namespace Application.Contracts;

public interface IAreaService
{
    Task<ResultDto<AreaReadDto>> CreateArea(AreaCreateDto areaCreateDto);
    Task<ResultDto<AreaReadDto>> GetArea(Guid id);
    Task<ResultDto<List<AreaReadDto>>> GetAreas();
    Task<ResultDto<AreaReadDto>> UpdateArea(Guid id, AreaUpdateDto areaUpdateDto);
    Task<ResultDto<bool>> DeleteArea(Guid id);
}

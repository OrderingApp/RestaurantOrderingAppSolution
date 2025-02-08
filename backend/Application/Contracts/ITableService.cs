using Application.Dtos.Common;
using Application.Dtos.Tables;

namespace Application.Contracts;

public interface ITableService
{
    Task<ResultDto<TableReadDto>> CreateTable(TableCreateDto tableCreateDto);
    Task<ResultDto<TableSummaryDto>> GetTable(Guid id);
    Task<ResultDto<List<TableReadDto>>> GetAllTables();
    Task<ResultDto<TableReadDto>> UpdateTable(TableUpdateDto tableUpdateDto, Guid id);
    Task<ResultDto<TableReadDto>> UpdateOccupancy(TableOccupancyDto tableOccupancyDto, Guid id);
    Task<ResultDto<bool>> DeleteTable(Guid id);
}

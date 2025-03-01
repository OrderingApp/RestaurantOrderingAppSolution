using Application.Dtos.Common;
using Application.Dtos.Tables;
using Domain;

namespace Application.Contracts;

public interface ITableService
{
    Task<ResultDto<TableReadDto>> CreateTable(TableCreateDto tableCreateDto);
    Task<ResultDto<TableSummaryDto>> GetTable(Guid id);
    Task<ResultDto<List<TableReadDto>>> GetTables();
    Task<ResultDto<TableReadDto>> UpdateTable(Guid id, TableUpdateDto tableUpdateDto);
    Task<ResultDto<TableReadDto>> UpdateStatus(Guid id, TableStatus tableStatus);
    Task<ResultDto<bool>> DeleteTable(Guid id);
}

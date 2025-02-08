using Application.Contracts;
using Application.Dtos.Tables;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class TableController(ITableService tableService) : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> CreateTable([FromBody] TableCreateDto tableCreateDto) =>
        HandleResult(await tableService.CreateTable(tableCreateDto));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTable(Guid id) =>
        HandleResult(await tableService.GetTable(id));

    [HttpGet]
    public async Task<IActionResult> GetAllTables() =>
        HandleResult(await tableService.GetAllTables());

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTable([FromBody] TableUpdateDto tableUpdateDto, Guid id) =>
        HandleResult(await tableService.UpdateTable(tableUpdateDto, id));

    [HttpPut("{id}/updateOccupancy")]
    public async Task<IActionResult> UpdateOccupancy([FromBody] TableOccupancyDto tableOccupancyDto, Guid id) =>
        HandleResult(await tableService.UpdateOccupancy(tableOccupancyDto, id));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTable(Guid id) =>
        HandleResult(await tableService.DeleteTable(id));
}

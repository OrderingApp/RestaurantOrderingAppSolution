using Application.Contracts;
using Application.Dtos.CustomerInformations;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class CustomerInformationController(ICustomerInformationService customerInformationService) : BaseApiController
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCustomerInformation(Guid id) =>
        HandleResult(await customerInformationService.GetCustomerInformation(id));

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCustomerInformation(Guid id, [FromBody] CustomerInformationUpdateDto customerInformationUpdateDto) =>
        HandleResult(await customerInformationService.UpdateCustomerInformation(customerInformationUpdateDto, id));
}

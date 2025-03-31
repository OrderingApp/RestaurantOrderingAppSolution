using Application.Contracts;
using Application.Dtos.CustomerInformations;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Manages customer information, including retrieval and updates.
/// </summary>
[Route("api/customers/{id}/information")]
public class CustomerInformationController(ICustomerInformationService customerInformationService)
    : BaseApiController
{
    /// <summary>
    /// Retrieves customer information by ID.
    /// </summary>
    /// <param name="id">The unique identifier of the customer.</param>
    /// <returns>The customer information.</returns>
    /// <response code="200">Returns the customer information.</response>
    /// <response code="404">If the customer is not found.</response>
    [HttpGet]
    [ProducesResponseType(typeof(CustomerInformationReadDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<CustomerInformationReadDto>> GetCustomerInformation(
        [FromRoute] Guid id
    ) => HandleResult(await customerInformationService.GetCustomerInformation(id));

    /// <summary>
    /// Updates customer information.
    /// </summary>
    /// <param name="id">The unique identifier of the customer.</param>
    /// <param name="customerInformationUpdateDto">The updated customer information.</param>
    /// <returns>The updated customer information.</returns>
    /// <response code="200">If the update was successful.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="404">If the customer is not found.</response>
    [HttpPut]
    [ProducesResponseType(typeof(CustomerInformationReadDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<CustomerInformationReadDto>> UpdateCustomerInformation(
        [FromRoute] Guid id,
        [FromBody] CustomerInformationUpdateDto customerInformationUpdateDto
    ) =>
        HandleResult(
            await customerInformationService.UpdateCustomerInformation(
                id,
                customerInformationUpdateDto
            )
        );
}

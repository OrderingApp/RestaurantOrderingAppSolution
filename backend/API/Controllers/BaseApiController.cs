using System.Security.Claims;
using Application.Dtos.Common;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
public abstract class BaseApiController : ControllerBase
{
    protected ActionResult HandleResult<T>(ResultDto<T> result)
    {
        if (result == null)
            return NotFound();

        if (result.IsSuccess && result.Data != null)
            return StatusCode((int)result.HttpStatusCode, result.Data);

        if (result.IsSuccess && result.Data == null)
            return NoContent();

        return StatusCode((int)result.HttpStatusCode, new { result.ErrorMessage });
    }

    [NonAction]
    public Guid GetUserId()
    {
        var userIdClaim =
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;

        if (Guid.TryParse(userIdClaim, out var userId))
            return userId;

        return Guid.Empty;
    }
}

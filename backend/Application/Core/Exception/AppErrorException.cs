using System.Net;

namespace Application.Core;

public class AppErrorException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public string? ErrorCode { get; }
    public IDictionary<string, string[]>? Errors { get; }

    public AppErrorException(HttpStatusCode statusCode, string message, string? errorCode = null, IDictionary<string, string[]>? errors = null)
        : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
        Errors = errors;
    }
}

public class NotFoundException(string message) : AppErrorException(HttpStatusCode.NotFound, message);
public class ConflictException(string message) : AppErrorException(HttpStatusCode.Conflict, message);
public class BadRequestException(string message) : AppErrorException(HttpStatusCode.BadRequest, message);

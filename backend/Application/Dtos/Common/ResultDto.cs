using System.Net;

namespace Application.Dtos.Common;

public class ResultDto<T>
{
    public bool IsSuccess { get; set; } = true;
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }
    public HttpStatusCode HttpStatusCode { get; set; }

    public static ResultDto<T> Success(T data, HttpStatusCode httpStatusCode) => 
        new ResultDto<T> { IsSuccess = true, Data = data, HttpStatusCode = httpStatusCode };

    public static ResultDto<T> Failure(string errorMessage, HttpStatusCode httpStatusCode) => 
        new ResultDto<T> { IsSuccess = false, ErrorMessage = errorMessage, HttpStatusCode = httpStatusCode };
}

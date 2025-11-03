using Application.Core;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

namespace API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger,
        IHostEnvironment env
    )
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await Handle(context, ex);
        }
    }

    private async Task Handle(HttpContext ctx, Exception ex)
    {
        var traceId = ctx.TraceIdentifier;
        var problem = BuildProblemDetails(ctx, ex, traceId);

        var level = problem.Status is >= 500 ? LogLevel.Error
                   : problem.Status is 404 or 409 ? LogLevel.Warning
                   : LogLevel.Information;

        _logger.Log(level, ex, "Unhandled exception. Status: {Status}, TraceId: {TraceId}", problem.Status, traceId);

        ctx.Response.ContentType = "application/problem+json";
        ctx.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;

        var json = JsonSerializer.Serialize(problem, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        await ctx.Response.WriteAsync(json);
    }

    private ProblemDetails BuildProblemDetails(HttpContext ctx, Exception ex, string traceId)
    {
        ProblemDetails problem;

        switch (ex)
        {
            // 1) Twoje rzucalne wyjątki biznesowe
            case AppErrorException appEx:
                problem = new ProblemDetails
                {
                    Title = appEx.Message,
                    Status = (int)appEx.StatusCode,
                    Type = "about:blank"
                };
                if (appEx.ErrorCode is not null)
                    problem.Extensions["errorCode"] = appEx.ErrorCode;
                if (appEx.Errors is not null)
                    problem.Extensions["errors"] = appEx.Errors;
                break;

            // 2) Walidacja (FluentValidation)
            case ValidationException vex:
                var errors = vex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                problem = new ProblemDetails
                {
                    Title = "Validation failed",
                    Status = StatusCodes.Status400BadRequest,
                    Type = "about:blank"
                };
                problem.Extensions["errors"] = errors;
                break;

            // 3) EF Core
            case DbUpdateConcurrencyException:
                problem = new ProblemDetails
                {
                    Title = "Concurrency conflict",
                    Status = StatusCodes.Status409Conflict,
                    Type = "about:blank"
                };
                break;

            case DbUpdateException:
                problem = new ProblemDetails
                {
                    Title = "Database update failed",
                    Status = StatusCodes.Status409Conflict,
                    Type = "about:blank"
                };
                break;

            // 4) Klient przerwał
            case OperationCanceledException:
                problem = new ProblemDetails
                {
                    Title = "Request was canceled by the client",
                    Status = 499, // nie ma w HttpStatusCode, ale często używany
                    Type = "about:blank"
                };
                break;

            // 5) Fallback
            default:
                problem = new ProblemDetails
                {
                    Title = "Unexpected error",
                    Status = StatusCodes.Status500InternalServerError,
                    Type = "about:blank"
                };
                break;
        }

        problem.Extensions["traceId"] = traceId;

        if (_env.IsDevelopment())
        {
            problem.Extensions["exception"] = ex.GetType().Name;
            problem.Extensions["message"] = ex.Message;
            problem.Extensions["stackTrace"] = ex.StackTrace;
        }

        return problem;
    }
}

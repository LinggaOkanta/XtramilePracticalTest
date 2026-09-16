using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Practical_Test_Xtramile.Application.Common.Exceptions;

namespace Practical_Test_Xtramile.Presentation.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);

        context.Response.ContentType = "application/problem+json";

        switch (exception)
        {
            case ValidationException validationEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var validationProblemDetails = new ValidationProblemDetails(validationEx.Errors)
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Validation Failed",
                    Detail = "One or more validation errors occurred.",
                    Instance = context.Request.Path
                };
                await context.Response.WriteAsync(JsonSerializer.Serialize(validationProblemDetails));
                break;

            case NotFoundException notFoundEx:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                var notFoundProblemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Resource Not Found",
                    Detail = notFoundEx.Message,
                    Instance = context.Request.Path
                };
                await context.Response.WriteAsync(JsonSerializer.Serialize(notFoundProblemDetails));
                break;

            case ArgumentException argEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var argProblemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Bad Request",
                    Detail = argEx.Message,
                    Instance = context.Request.Path
                };
                await context.Response.WriteAsync(JsonSerializer.Serialize(argProblemDetails));
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "An error occurred while processing your request.",
                    Detail = exception.Message,
                    Instance = context.Request.Path
                };
                await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
                break;
        }
    }
}

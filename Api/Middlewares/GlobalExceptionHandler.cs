using Common.CommonResponse;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;


namespace Api.Middlewares;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var response = new ApiResponse<object>
        {
            Success = false,
            Data = null
        };

        if (exception is ValidationException validationException)
        {
            response.StatusCode = StatusCodes.Status400BadRequest;
            var errors = validationException.Errors.Select(e => e.ErrorMessage);
            response.Message = string.Join(" | ", errors);
        }
        else
        {
            logger.LogError(exception, "An unhandled server exception occurred.");

            response.StatusCode = StatusCodes.Status500InternalServerError;
            response.Message = "An unexpected server error occurred. Please try again later.";
        }

        httpContext.Response.StatusCode = response.StatusCode;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
        
        return true;
    }
}
using Bookstore.Api.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Api.Middleware;

public sealed class BookstoreExceptionHandler : IExceptionHandler
{
    private readonly ILogger<BookstoreExceptionHandler> _logger;

    public BookstoreExceptionHandler(ILogger<BookstoreExceptionHandler> logger) => _logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken ct)
    {
        var (statusCode, title, detail) = exception switch
        {
            NotFoundException nf => (StatusCodes.Status404NotFound, "Not Found", nf.Message),
            BadRequestException br => (StatusCodes.Status400BadRequest, "Bad Request", br.Message),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error", "An unexpected error occurred.")
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
            _logger.LogError(exception, "Unhandled exception processing {Path}", httpContext.Request.Path);
        else
            _logger.LogWarning("{ExceptionType} handled for {Path}: {Message}", exception.GetType().Name, httpContext.Request.Path, exception.Message);

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        }, ct);

        return true;
    }
}

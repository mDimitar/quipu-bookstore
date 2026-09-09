using System.Text.Json;
using Bookstore.Api.Exceptions;
using Bookstore.Api.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Bookstore.Api.Tests.Middleware;

public class BookstoreExceptionHandlerTests
{
    private static async Task<(int StatusCode, ProblemDetails Body)> HandleAsync(Exception exception)
    {
        var handler = new BookstoreExceptionHandler(NullLogger<BookstoreExceptionHandler>.Instance);
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var handled = await handler.TryHandleAsync(context, exception, CancellationToken.None);
        Assert.True(handled);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await JsonSerializer.DeserializeAsync<ProblemDetails>(context.Response.Body);
        return (context.Response.StatusCode, body!);
    }

    [Fact]
    public async Task NotFoundException_Returns404WithMessage()
    {
        var (statusCode, body) = await HandleAsync(new NotFoundException("Book", 42));

        Assert.Equal(StatusCodes.Status404NotFound, statusCode);
        Assert.Equal("Book with id '42' was not found.", body.Detail);
    }

    [Fact]
    public async Task BadRequestException_Returns400WithMessage()
    {
        var (statusCode, body) = await HandleAsync(new BadRequestException("Author with id '99' does not exist."));

        Assert.Equal(StatusCodes.Status400BadRequest, statusCode);
        Assert.Equal("Author with id '99' does not exist.", body.Detail);
    }

    [Fact]
    public async Task UnhandledException_Returns500WithGenericMessage()
    {
        var (statusCode, body) = await HandleAsync(new InvalidOperationException("some internal detail"));

        Assert.Equal(StatusCodes.Status500InternalServerError, statusCode);
        Assert.Equal("An unexpected error occurred.", body.Detail);
        Assert.DoesNotContain("some internal detail", body.Detail);
    }
}

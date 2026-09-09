using Bookstore.Api.Exceptions;
using Xunit;

namespace Bookstore.Api.Tests.Exceptions;

public class NotFoundExceptionTests
{
    [Fact]
    public void Constructor_WithEntityNameAndKey_FormatsMessage()
    {
        var ex = new NotFoundException("Book", 42);
        Assert.Equal("Book with id '42' was not found.", ex.Message);
    }
}

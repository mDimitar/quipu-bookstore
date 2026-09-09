using Bookstore.Api.Data;
using Bookstore.Api.Dtos.Requests;
using Bookstore.Api.Entities;
using Bookstore.Api.Exceptions;
using Bookstore.Api.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Bookstore.Api.Tests.Services;

public class BookServiceTests
{
    private static BookstoreDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<BookstoreDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        return new BookstoreDbContext(options);
    }

    [Fact]
    public async Task GetByIdAsync_UnknownId_ThrowsNotFoundException()
    {
        await using var context = CreateContext(nameof(GetByIdAsync_UnknownId_ThrowsNotFoundException));
        var service = new BookService(context);

        await Assert.ThrowsAsync<NotFoundException>(() => service.GetByIdAsync(999, CancellationToken.None));
    }

    [Fact]
    public async Task CreateAsync_UnknownAuthorId_ThrowsBadRequestException()
    {
        await using var context = CreateContext(nameof(CreateAsync_UnknownAuthorId_ThrowsBadRequestException));
        var service = new BookService(context);
        var request = new CreateBookRequest { AuthorId = 1, Title = "Valid Title" };

        await Assert.ThrowsAsync<BadRequestException>(() => service.CreateAsync(request, CancellationToken.None));
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_PersistsAndReturnsBook()
    {
        await using var context = CreateContext(nameof(CreateAsync_ValidRequest_PersistsAndReturnsBook));
        context.Authors.Add(new Author { AuthorId = 1, Name = "Test Author" });
        await context.SaveChangesAsync();
        var service = new BookService(context);
        var request = new CreateBookRequest { AuthorId = 1, Title = "Valid Title", SubTitle = "Sub" };

        var result = await service.CreateAsync(request, CancellationToken.None);

        Assert.Equal("Valid Title", result.Title);
        Assert.Equal("Test Author", result.Author.Name);
    }

    [Fact]
    public async Task DeleteAsync_ExistingBook_RemovesIt()
    {
        await using var context = CreateContext(nameof(DeleteAsync_ExistingBook_RemovesIt));
        context.Authors.Add(new Author { AuthorId = 1, Name = "Test Author" });
        context.Books.Add(new Book { BookId = 1, AuthorId = 1, Title = "To Delete" });
        await context.SaveChangesAsync();
        var service = new BookService(context);

        await service.DeleteAsync(1, CancellationToken.None);

        Assert.Empty(context.Books);
    }
}

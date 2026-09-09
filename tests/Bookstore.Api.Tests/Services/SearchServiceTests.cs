using Bookstore.Api.Data;
using Bookstore.Api.Entities;
using Bookstore.Api.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Bookstore.Api.Tests.Services;

public class SearchServiceTests
{
    private static BookstoreDbContext CreateSeededContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<BookstoreDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        var context = new BookstoreDbContext(options);

        var tolkien = new Author { AuthorId = 1, Name = "J.R.R. Tolkien" };
        var asimov = new Author { AuthorId = 2, Name = "Isaac Asimov" };
        context.Authors.AddRange(tolkien, asimov);
        context.Books.AddRange(
            new Book { BookId = 1, AuthorId = 1, Title = "The Fellowship of the Ring", Author = tolkien },
            new Book { BookId = 2, AuthorId = 1, Title = "The Two Towers", Author = tolkien },
            new Book { BookId = 3, AuthorId = 2, Title = "Foundation", Author = asimov }
        );
        context.SaveChanges();
        return context;
    }

    [Fact]
    public async Task SearchBooksAsync_FiltersByTitleCaseInsensitively()
    {
        await using var context = CreateSeededContext(nameof(SearchBooksAsync_FiltersByTitleCaseInsensitively));
        var service = new SearchService(context);

        var result = await service.SearchBooksAsync(title: "the two", author: null, page: 1, pageSize: 20, CancellationToken.None);

        Assert.Equal(1, result.TotalCount);
        Assert.Equal("The Two Towers", result.Items[0].Title);
    }

    [Fact]
    public async Task SearchBooksAsync_FiltersByAuthorName()
    {
        await using var context = CreateSeededContext(nameof(SearchBooksAsync_FiltersByAuthorName));
        var service = new SearchService(context);

        var result = await service.SearchBooksAsync(title: null, author: "asimov", page: 1, pageSize: 20, CancellationToken.None);

        Assert.Equal(1, result.TotalCount);
        Assert.Equal("Foundation", result.Items[0].Title);
    }

    [Fact]
    public async Task SearchBooksAsync_PaginatesResults()
    {
        await using var context = CreateSeededContext(nameof(SearchBooksAsync_PaginatesResults));
        var service = new SearchService(context);

        var page1 = await service.SearchBooksAsync(title: null, author: null, page: 1, pageSize: 2, CancellationToken.None);
        var page2 = await service.SearchBooksAsync(title: null, author: null, page: 2, pageSize: 2, CancellationToken.None);

        Assert.Equal(3, page1.TotalCount);
        Assert.Equal(2, page1.Items.Count);
        Assert.Equal(1, page2.Items.Count);
        Assert.Equal(2, page1.TotalPages);
    }

    [Fact]
    public async Task SearchBooksAsync_PageSizeAboveMax_IsCapped()
    {
        await using var context = CreateSeededContext(nameof(SearchBooksAsync_PageSizeAboveMax_IsCapped));
        var service = new SearchService(context);

        var result = await service.SearchBooksAsync(title: null, author: null, page: 1, pageSize: 1000, CancellationToken.None);

        Assert.Equal(100, result.PageSize);
    }
}

using Bookstore.Api.Data;
using Bookstore.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Bookstore.Api.Tests.Data;

public class BookstoreDbContextTests
{
    [Fact]
    public void ModelCreating_MapsAuthorAndBookEntityTypes()
    {
        var options = new DbContextOptionsBuilder<BookstoreDbContext>()
            .UseInMemoryDatabase(databaseName: nameof(ModelCreating_MapsAuthorAndBookEntityTypes))
            .Options;

        using var context = new BookstoreDbContext(options);

        Assert.Contains(context.Model.GetEntityTypes(), e => e.ClrType == typeof(Author));
        Assert.Contains(context.Model.GetEntityTypes(), e => e.ClrType == typeof(Book));
    }
}

using Bookstore.Api.Data;
using Bookstore.Api.Dtos.Responses;
using Bookstore.Api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Api.Services;

public class SearchService : ISearchService
{
    private const int MaxPageSize = 100;
    private readonly BookstoreDbContext _dbContext;

    public SearchService(BookstoreDbContext dbContext) => _dbContext = dbContext;

    public async Task<PagedResult<BookResponse>> SearchBooksAsync(string? title, string? author, int page, int pageSize, CancellationToken ct)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 20 : Math.Min(pageSize, MaxPageSize);

        var query = _dbContext.Books.Include(b => b.Author).AsQueryable();

        if (!string.IsNullOrWhiteSpace(title))
        {
            var normalizedTitle = title.ToUpperInvariant();
            query = query.Where(b => b.Title.ToUpper().Contains(normalizedTitle));
        }

        if (!string.IsNullOrWhiteSpace(author))
        {
            var normalizedAuthor = author.ToUpperInvariant();
            query = query.Where(b => b.Author.Name.ToUpper().Contains(normalizedAuthor));
        }

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderBy(b => b.BookId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<BookResponse>(items.Select(b => b.ToResponse()).ToList(), page, pageSize, totalCount);
    }
}

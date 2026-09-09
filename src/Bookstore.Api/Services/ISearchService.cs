using Bookstore.Api.Dtos.Responses;

namespace Bookstore.Api.Services;

public interface ISearchService
{
    Task<PagedResult<BookResponse>> SearchBooksAsync(string? title, string? author, int page, int pageSize, CancellationToken ct);
}

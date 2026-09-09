using Bookstore.Api.Dtos.Requests;
using Bookstore.Api.Dtos.Responses;

namespace Bookstore.Api.Services;

public interface IBookService
{
    Task<IReadOnlyList<BookResponse>> GetAllAsync(CancellationToken ct);
    Task<BookResponse> GetByIdAsync(int bookId, CancellationToken ct);
    Task<BookResponse> CreateAsync(CreateBookRequest request, CancellationToken ct);
    Task<BookResponse> UpdateAsync(int bookId, UpdateBookRequest request, CancellationToken ct);
    Task DeleteAsync(int bookId, CancellationToken ct);
    Task<IReadOnlyList<AuthorResponse>> GetAllAuthorsAsync(CancellationToken ct);
}

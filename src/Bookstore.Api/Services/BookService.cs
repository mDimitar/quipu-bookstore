using Bookstore.Api.Data;
using Bookstore.Api.Dtos.Requests;
using Bookstore.Api.Dtos.Responses;
using Bookstore.Api.Entities;
using Bookstore.Api.Exceptions;
using Bookstore.Api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Api.Services;

public class BookService : IBookService
{
    private readonly BookstoreDbContext _dbContext;

    public BookService(BookstoreDbContext dbContext) => _dbContext = dbContext;

    public async Task<IReadOnlyList<BookResponse>> GetAllAsync(CancellationToken ct)
    {
        var books = await _dbContext.Books.Include(b => b.Author).ToListAsync(ct);
        return books.Select(b => b.ToResponse()).ToList();
    }

    public async Task<BookResponse> GetByIdAsync(int bookId, CancellationToken ct)
    {
        var book = await _dbContext.Books.Include(b => b.Author).FirstOrDefaultAsync(b => b.BookId == bookId, ct)
            ?? throw new NotFoundException("Book", bookId);
        return book.ToResponse();
    }

    public async Task<BookResponse> CreateAsync(CreateBookRequest request, CancellationToken ct)
    {
        var authorExists = await _dbContext.Authors.AnyAsync(a => a.AuthorId == request.AuthorId, ct);
        if (!authorExists)
            throw new BadRequestException($"Author with id '{request.AuthorId}' does not exist.");

        var book = new Book { AuthorId = request.AuthorId, Title = request.Title, SubTitle = request.SubTitle };
        _dbContext.Books.Add(book);
        await _dbContext.SaveChangesAsync(ct);

        return await GetByIdAsync(book.BookId, ct);
    }

    public async Task<BookResponse> UpdateAsync(int bookId, UpdateBookRequest request, CancellationToken ct)
    {
        var book = await _dbContext.Books.FirstOrDefaultAsync(b => b.BookId == bookId, ct)
            ?? throw new NotFoundException("Book", bookId);

        var authorExists = await _dbContext.Authors.AnyAsync(a => a.AuthorId == request.AuthorId, ct);
        if (!authorExists)
            throw new BadRequestException($"Author with id '{request.AuthorId}' does not exist.");

        book.AuthorId = request.AuthorId;
        book.Title = request.Title;
        book.SubTitle = request.SubTitle;
        await _dbContext.SaveChangesAsync(ct);

        return await GetByIdAsync(book.BookId, ct);
    }

    public async Task DeleteAsync(int bookId, CancellationToken ct)
    {
        var book = await _dbContext.Books.FirstOrDefaultAsync(b => b.BookId == bookId, ct)
            ?? throw new NotFoundException("Book", bookId);

        _dbContext.Books.Remove(book);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<AuthorResponse>> GetAllAuthorsAsync(CancellationToken ct)
    {
        var authors = await _dbContext.Authors.ToListAsync(ct);
        return authors.Select(a => a.ToResponse()).ToList();
    }
}

using Bookstore.Api.Dtos.Responses;
using Bookstore.Api.Entities;

namespace Bookstore.Api.Mapping;

public static class BookMappingExtensions
{
    public static BookResponse ToResponse(this Book book) =>
        new(book.BookId, book.Title, book.SubTitle, book.Author.ToResponse());

    public static AuthorResponse ToResponse(this Author author) =>
        new(author.AuthorId, author.Name);
}

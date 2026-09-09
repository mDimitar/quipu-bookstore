namespace Bookstore.Api.Dtos.Responses;

public record BookResponse(int BookId, string Title, string? SubTitle, AuthorResponse Author);

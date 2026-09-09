namespace Bookstore.Api.Entities;

public class Book
{
    public int BookId { get; set; }
    public int AuthorId { get; set; }
    public string Title { get; set; } = null!;
    public string? SubTitle { get; set; }
    public Author Author { get; set; } = null!;
}

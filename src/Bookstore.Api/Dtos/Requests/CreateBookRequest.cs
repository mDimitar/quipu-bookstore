using System.ComponentModel.DataAnnotations;

namespace Bookstore.Api.Dtos.Requests;

public record CreateBookRequest
{
    [Required] public int AuthorId { get; init; }
    [Required, MinLength(3), MaxLength(100)] public string Title { get; init; } = "";
    [MaxLength(200)] public string? SubTitle { get; init; }
}

using Bookstore.Api.Dtos.Requests;
using Bookstore.Api.Dtos.Responses;
using Bookstore.Api.Services;
using Bookstore.Shared.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Api.Controllers;

[ApiController]
[Route("api")]
[Authorize(Policy = PolicyNames.CrudAccess)]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService) => _bookService = bookService;

    [HttpGet("books")]
    public async Task<ActionResult<IReadOnlyList<BookResponse>>> GetAll(CancellationToken ct) =>
        Ok(await _bookService.GetAllAsync(ct));

    [HttpGet("books/{bookId:int}")]
    public async Task<ActionResult<BookResponse>> GetById(int bookId, CancellationToken ct) =>
        Ok(await _bookService.GetByIdAsync(bookId, ct));

    [HttpPost("books")]
    public async Task<ActionResult<BookResponse>> Create(CreateBookRequest request, CancellationToken ct)
    {
        var created = await _bookService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { bookId = created.BookId }, created);
    }

    [HttpPut("books/{bookId:int}")]
    public async Task<ActionResult<BookResponse>> Update(int bookId, UpdateBookRequest request, CancellationToken ct) =>
        Ok(await _bookService.UpdateAsync(bookId, request, ct));

    [HttpDelete("books/{bookId:int}")]
    public async Task<IActionResult> Delete(int bookId, CancellationToken ct)
    {
        await _bookService.DeleteAsync(bookId, ct);
        return NoContent();
    }

    [HttpGet("authors")]
    public async Task<ActionResult<IReadOnlyList<AuthorResponse>>> GetAllAuthors(CancellationToken ct) =>
        Ok(await _bookService.GetAllAuthorsAsync(ct));
}

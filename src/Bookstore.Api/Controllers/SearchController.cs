using Bookstore.Api.Dtos.Responses;
using Bookstore.Api.Services;
using Bookstore.Shared.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Api.Controllers;

[ApiController]
[Route("api/search")]
[Authorize(Policy = PolicyNames.SearchAccess)]
public class SearchController : ControllerBase
{
    private readonly ISearchService _searchService;

    public SearchController(ISearchService searchService) => _searchService = searchService;

    [HttpGet("books")]
    public async Task<ActionResult<PagedResult<BookResponse>>> SearchBooks(
        [FromQuery] string? title,
        [FromQuery] string? author,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default) =>
        Ok(await _searchService.SearchBooksAsync(title, author, page, pageSize, ct));
}

using System.ComponentModel.DataAnnotations;
using Bookstore.Api.Dtos.Requests;
using Xunit;

namespace Bookstore.Api.Tests.Dtos;

public class CreateBookRequestValidationTests
{
    private static IList<ValidationResult> Validate(CreateBookRequest request)
    {
        var context = new ValidationContext(request);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(request, context, results, validateAllProperties: true);
        return results;
    }

    [Fact]
    public void Title_ShorterThanThreeCharacters_FailsValidation()
    {
        var request = new CreateBookRequest { AuthorId = 1, Title = "Ab" };
        var results = Validate(request);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(CreateBookRequest.Title)));
    }

    [Fact]
    public void ValidRequest_PassesValidation()
    {
        var request = new CreateBookRequest { AuthorId = 1, Title = "Valid Title", SubTitle = null };
        var results = Validate(request);
        Assert.Empty(results);
    }
}

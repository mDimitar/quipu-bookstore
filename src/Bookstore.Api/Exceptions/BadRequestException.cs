namespace Bookstore.Api.Exceptions;

public sealed class BadRequestException : BookstoreException
{
    public BadRequestException(string message) : base(message) { }
}

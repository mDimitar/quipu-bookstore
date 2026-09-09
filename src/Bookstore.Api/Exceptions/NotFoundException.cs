namespace Bookstore.Api.Exceptions;

public sealed class NotFoundException : BookstoreException
{
    public NotFoundException(string message) : base(message) { }
    public NotFoundException(string entityName, object key) : base($"{entityName} with id '{key}' was not found.") { }
}

namespace Bookstore.Api.Exceptions;

public abstract class BookstoreException : Exception
{
    protected BookstoreException(string message) : base(message) { }
}

namespace Demo.Invoices.API.Application;

public class ApplicationException : Exception
{
    public ApplicationException()
    {
    }
    public ApplicationException(string? message) : base(message)
    {
    }
}
namespace Demo.Invoices.API.Application;

/// <summary>
/// Application do not care how user is authenticated. It can expose it's own identity contract for hosting layer to meet.
/// </summary>
public interface IUserContext
{
    public bool IsAuthenticated { get; }

    public string Email { get; }
}

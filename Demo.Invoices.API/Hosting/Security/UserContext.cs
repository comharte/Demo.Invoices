using System.Security.Claims;

namespace Demo.Invoices.API.Hosting.Security;

public class UserContext : IUserContext
{
    public UserContext()
    {
        _authenticated = false;
        _email = string.Empty;
    }

    internal static bool CanBeAuthenticated(ClaimsPrincipal claimsPrincipal)
    {
        var userContext = new UserContext();
        userContext.Authenticate(claimsPrincipal);
        return userContext.IsAuthenticated;

    }

    internal void Authenticate(ClaimsPrincipal claimsPrincipal)
    {
        var email = claimsPrincipal.FindFirst(DemoClaimTypes.UserEmail)?.Value;

        if(!string.IsNullOrEmpty(email))
        {
            _authenticated = true;
            _email = email;
        }
        else
        {
            _authenticated = false;
            _email = string.Empty;
        }
    }

    private bool _authenticated;

    private string _email;

    public bool IsAuthenticated => _authenticated;

    public string Email => _email;
}

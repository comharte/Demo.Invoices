namespace Demo.Invoices.API.Hosting.Security;

/// <summary>
/// Usually within our app we want standardized claim types that authorization layer can rely on.
/// It's up to authenticaion handlers and claims transformations to map incoming claims to these standardized claim types.
/// </summary>
public sealed class DemoClaimTypes
{
    public const string UserId = "id";

    public const string UserName = "name";

    public const string UserEmail = "email";

    public const string Role = "role";

    public const string Permission = "permission";
}
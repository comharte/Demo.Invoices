using Microsoft.AspNetCore.Authorization;
using System.Collections.Immutable;

namespace Demo.Invoices.API.Hosting.Security;

internal class AuthorizePermissionAttribute : AuthorizeAttribute,
    IAuthorizationRequirement, IAuthorizationRequirementData
{
    private readonly ImmutableArray<string> _permissions;

    public AuthorizePermissionAttribute(params string[] permissions)
    {
        _permissions = [.. permissions];
        AuthenticationSchemes = SecurityAuthenticationSchemes.AllConcatenated;
    }

    public IReadOnlyCollection<string> Permissions => _permissions;

    public IEnumerable<IAuthorizationRequirement> GetRequirements()
    {
        yield return this;
    }
}

internal class PermissionAuthorizationHandler(ILogger<PermissionAuthorizationHandler> logger)
    : AuthorizationHandler<AuthorizePermissionAttribute>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AuthorizePermissionAttribute requirement)
    {
        if (context.User.Claims.Any(c => c.Type.Equals(DemoClaimTypes.Permission, StringComparison.OrdinalIgnoreCase)
            && requirement.Permissions.Contains(c.Value)))
        {
            context.Succeed(requirement);
        }
        else
        {
            logger.LogWarning("Authorization failed: user does not have required permissions: {Permissions}",
                string.Join(", ", requirement.Permissions));
        }

        return Task.CompletedTask;
    }
}

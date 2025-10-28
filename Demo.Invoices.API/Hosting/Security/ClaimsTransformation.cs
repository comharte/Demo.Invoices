using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace Demo.Invoices.API.Hosting.Security;

public class ClaimsTransformation : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if(principal.Identity is ClaimsIdentity identity)
        {
            identity.AddClaim(new Claim("Transformed", "true"));
        }

        return Task.FromResult(principal);
    }
}
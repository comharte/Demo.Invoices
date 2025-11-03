using Microsoft.IdentityModel.Tokens;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Demo.Invoices.API.Hosting.Security;

/// <summary>
/// This is fake user store for demonstration purposes only.
/// Do not code your own user stores or permissions stores. If needed use Microsfot's Identity (https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-9.0&tabs=visual-studio)
/// Best practice implemented in modern system architecture is delegating authentication and authorization to dedicated Identity Providers (IdP) using standards like OAuth2 and OpenID Connect.
/// </summary>
public class FakeUserStore
{
    private class FakeUser
    {
        private static byte[] CreateHash(string password)
        {
            using var sha256 = SHA256.Create();
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        private ImmutableHashSet<Claim> _claims;

        private readonly byte[] _passwordHash;

        public FakeUser(string id, string userName, string userEmail, string password, IEnumerable<string> permissions, IEnumerable<string> roles)
        {
            _passwordHash = CreateHash(password);

            var claims = new List<Claim>
            {
                new (DemoClaimTypes.UserId, id),
                new (DemoClaimTypes.UserName, userName),
                new (DemoClaimTypes.UserEmail, userEmail)
            };

            claims.AddRange(permissions.Select(p => new Claim(DemoClaimTypes.Permission, p)));
            claims.AddRange(roles.Select(r => new Claim(DemoClaimTypes.Role, r)));

            _claims = [.. claims];
        }

        public bool VerifyPassword(string password)
            => _passwordHash.SequenceEqual(CreateHash(password));

        public IReadOnlyCollection<Claim> Claims => _claims;
    }

    private static KeyValuePair<string, FakeUser> CreateFakeUser(string id, string userName, string userEmail, string password, IEnumerable<string> permissions, IEnumerable<string> roles)
        => new KeyValuePair<string, FakeUser>(
            id,
            new FakeUser(id, userName, userEmail, password, permissions, roles)
        );

    private static Dictionary<string, FakeUser> _users = new([
        CreateFakeUser("asmith","Alice Smith","alice.smith@example.com", "as123", ["Invoices.Read","Invoices.Modify"], ["Admin"]),
        CreateFakeUser("jdoe","John Doe","john.doe@example.com","jd123", ["Invoices.Read", "Access.Invoice.Dev.Resources"], ["Basic", "Developer"]),
        ]);


    private readonly JwtTokenSymmetricSigningCredentials _signingCredentials;

    public FakeUserStore(JwtTokenSymmetricSigningCredentials signingCredentials)
    {
        _signingCredentials = signingCredentials;
    }

    public bool TryAuthenticate(string username, string password, [MaybeNullWhen(false)] out List<Claim> claims)
    {
        if (_users.TryGetValue(username, out var user) && user.VerifyPassword(password))
        {
            claims = [.. user.Claims];
            return true;
        }

        claims = null;
        return false;
    }

    public string CreateToken(List<Claim> claims)
        => new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
            issuer: _signingCredentials.Issuer,
            audience: _signingCredentials.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_signingCredentials.ExpirationMinutes),
            signingCredentials: _signingCredentials.Credentials));
}
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Demo.Invoices.API.Hosting.Security;

public class UserStore
{
    private readonly JwtTokenSymmetricSigningCredentials _signingCredentials;

    private static KeyValuePair<string, Dictionary<string, string>> CreateClaims(string userId, string userName, string userEmail, int age, params string[] roles)
        => new(userId, new Dictionary<string, string>
        {
            { "userId", userId },
            { "userName", userName },
            { "userEmail", userEmail },
            { "age", age.ToString() },
            { "roles", string.Join(';', roles) }
        });

    private readonly Dictionary<string, Dictionary<string, string>> _userClaims = new([
            CreateClaims("jd","John Doe", "jd@example.com", 61, "Admin", "User"),
            CreateClaims("as", "Alice Smith", "as@example.com", 16, "User")]
        );

    public UserStore(JwtTokenSymmetricSigningCredentials signingCredentials)
    {
        _signingCredentials = signingCredentials;
    }

    public bool TryAuthenticate(string username, string password, [MaybeNullWhen(false)] out Dictionary<string, string> claims)
        => _userClaims.TryGetValue(username, out claims);

    public string CreateToken(Dictionary<string, string> claimsValues)
    {
        // In a real application, you would add claims based on the user's information
        var claims = new List<Claim>
        {
            new("userId", claimsValues["userId"]),
            new("userName", claimsValues["userName"]),
            new("userEmail", claimsValues["userEmail"]),
            new("age", claimsValues["age"])
        };

        var roles = claimsValues["roles"].Split(';', StringSplitOptions.RemoveEmptyEntries);

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
            issuer: _signingCredentials.Issuer,
            audience: _signingCredentials.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_signingCredentials.ExpirationMinutes),
            signingCredentials: _signingCredentials.Credentials));
    }
}
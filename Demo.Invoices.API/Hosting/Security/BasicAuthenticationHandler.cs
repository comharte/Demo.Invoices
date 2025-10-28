using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Demo.Invoices.API.Hosting.Security
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly UserStore _userStore;

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            UserStore userStore)
            : base(options, logger, encoder)
        {
            _userStore = userStore;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                if (!Request.Headers.TryGetValue("Authorization", out var authorizationHeaderValues))
                {
                    return Fail("Missing Authorization Header");
                }

                if (authorizationHeaderValues.Count != 1 || string.IsNullOrEmpty(authorizationHeaderValues.Single()?.ToString()))
                {
                    return Fail("Invalid Authorization Header");
                }

                var authorizationHeader = AuthenticationHeaderValue.Parse(authorizationHeaderValues.Single()!.ToString());

                if (!authorizationHeader.Scheme.Equals("Basic", StringComparison.OrdinalIgnoreCase))
                {
                    return Fail("Invalid Authorization Scheme");
                }

                var credentialBytes = Convert.FromBase64String(authorizationHeader.Parameter ?? string.Empty);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
                if (credentials.Length != 2)
                {
                    return Fail("Invalid Authorization Header Parts");
                }

                if (!_userStore.TryAuthenticate(credentials[0] ?? string.Empty, credentials[1] ?? string.Empty, out var claimsValues))
                {
                    return Fail("Invalid Username or Password");
                }

                return Success(claimsValues!);
            }
            catch (FormatException)
            {
                return Fail("Invalid Authorization Header Format");
            }
        }

        private Task<AuthenticateResult> Fail(string message)
            => Task.FromResult(AuthenticateResult.Fail(message));

        private Task<AuthenticateResult> Success(Dictionary<string, string> claimsValues)
        {
            var claims = claimsValues.Select(kv => new Claim(kv.Key, kv.Value));
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
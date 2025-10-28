using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Demo.Invoices.API.Hosting.Security
{
    public class JwtTokenSymmetricSigningCredentials
    {
        private const string ConfigurationSectionName = "JwtTokenSymmetricKey";

        private readonly SigningCredentials _signingCredentials;

        private readonly SymmetricSecurityKey _symmetricSecurityKey;

        public JwtTokenSymmetricSigningCredentials(IConfiguration configuration)
        {
            configuration.Bind("JwtTokenSymmetricKey", this);

            if (string.IsNullOrEmpty(Issuer) || string.IsNullOrEmpty(Audience))
            {
                throw new InvalidOperationException("JWT Token Symmetric Key configuration is invalid.");
            }

            if (string.IsNullOrEmpty(Key))
            {
                throw new InvalidOperationException("JWT Token Symmetric Key is not configured.");
            }

            if (ExpirationMinutes < 1)
            {
                throw new InvalidOperationException("JWT Token Symmetric Key ExpirationMinutes must be greater than zero.");
            }

            _symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));

            _signingCredentials = new SigningCredentials(_symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
        }

        public string Issuer { get; set; } = "Demo.Invoices.API";

        public string Audience { get; set; } = "Demo.Invoices.API.Clients";

        public string Key { get; set; } = Guid.NewGuid().ToString();

        public int ExpirationMinutes { get; set; } = 60;

        public SigningCredentials Credentials => _signingCredentials;

        public SymmetricSecurityKey SymmetricSecurityKey => _symmetricSecurityKey;
    }
}
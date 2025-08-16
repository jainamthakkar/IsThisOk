using IsThisOk.Application.Interfaces;
using IsThisOk.Domain.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IsThisOk.Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;

        // Constructor - gets JWT settings from appsettings.json
        public TokenService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public string GenerateToken(string userId, string email, string role)
        {
            // Step 1: Create "claims" - think of these as stamps on your ID card
            var claims = new[]
            {
                new Claim("userId", userId),           // Your unique ID
                new Claim("email", email),             // Your email
                new Claim("role", role),               // Are you User or Admin?
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Unique token ID
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64) // When was this created?
            };

            // Step 2: Create the "key" to encrypt the token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Step 3: Create the actual token
            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,                           // Who created this token? (Your app)
                audience: _jwtSettings.Audience,                       // Who is this token for? (Your users)
                claims: claims,                                        // What info does it contain?
                expires: DateTime.UtcNow.AddDays(_jwtSettings.ExpiryInDays), // When does it expire?
                signingCredentials: creds                              // How to verify it's real?
            );

            // Step 4: Convert to string so browser can store it
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

                // Try to decode and validate the token
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,                   // Check if signature is valid
                    IssuerSigningKey = new SymmetricSecurityKey(key),  // Use our secret key
                    ValidateIssuer = true,                             // Check if token came from our app
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = true,                           // Check if token is for our users
                    ValidAudience = _jwtSettings.Audience,
                    ValidateLifetime = true,                           // Check if token hasn't expired
                    ClockSkew = TimeSpan.Zero                          // No grace period for expiration
                }, out SecurityToken validatedToken);

                return true; // Token is valid!
            }
            catch
            {
                return false; // Token is invalid or expired
            }
        }
    }
}
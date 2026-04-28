using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Schefco.TaskFlow.Application.Common.Interfaces;
using Schefco.TaskFlow.Domain.Entities;

namespace Schefco.TaskFlow.Infrastructure.Identity
{
    // Generates JWT tokens for authenticated users and generates temp tokens for first time login
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly IConfiguration _config;

        // Inject configuration so we can read JWT settings (key, issuer, audience, etc.)
        public JwtTokenGenerator(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(AppUser user)
        {
            // JWT signing Key: Loaded from secret key in configuration
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

            // JWT Signing Credentials using HMAC SHA256
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Claims for RBAC
            // Add the user information that will be embedded inside the JWT
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            // JWT Token Creation: issuer, audience, claims, expiration and siging creds
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:ExpiresMinutes"]!)
                ),
                signingCredentials: creds
            );

            // Convert the token object into a string to return
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateTempToken(AppUser user)
        {
            // JWT signing Key: Loaded from secret key in configuration
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

            // JWT Signing Credentials using HMAC SHA256
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Claims for First time login password reset
            // Add the user information that will be embedded inside the JWT
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("temp", "true")
            };

            // Get expiration for temp token from appsettings
            var hours = int.Parse(_config["Jwt:TempTokenHours"]!);

            // JWT Token Creation: issuer, audience, claims, expiration and siging creds
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(hours),
                signingCredentials: creds
            );

            // Convert the token object into a string to return
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal ValidateTempToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            // Parameters to validate temp password reset token
            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _config["Jwt:Issuer"],

                ValidateAudience = true,
                ValidAudience = _config["Jwt:Audience"],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)),

                ValidateLifetime = true,
            };

            // Validate signature, issuer, audience, expiration
            var principal = tokenHandler.ValidateToken(token, parameters, out _);

            // Check if token is temp for password reset
            var isTemp = principal.Claims.Any(c => c.Type == "temp" && c.Value == "true");

            if (!isTemp)
                throw new SecurityTokenException("Token is not a temporary password reset token");

            // Return the validated principal
            return principal;
        }
    }
}

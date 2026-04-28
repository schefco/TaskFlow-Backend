using Microsoft.AspNetCore.Http;
using Schefco.TaskFlow.Application.Common.Interfaces;
using Schefco.TaskFlow.Application.Common.Mediator;
using System.IdentityModel.Tokens.Jwt;

namespace Schefco.TaskFlow.Infrastructure.Services
{
    public class CurrentTokenService : ICurrentTokenService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public string? Token { get; set; }

        public CurrentTokenService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid GetCurrentUserId()
        {
            // load in the current token
            var token = Token;

            // if token is null send empty Guid
            if (string.IsNullOrEmpty(token))
                return Guid.Empty;

            // Read the token if its isn't null
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            // Find the UserId from the token
            var sub = jwt.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

            // If we can find the userId send it
            if (Guid.TryParse(sub, out var userId))
                return userId;

            // Otherwise return Empty guid
            return Guid.Empty;
        }
    }
}

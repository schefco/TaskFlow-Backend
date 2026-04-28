using Schefco.TaskFlow.Application.Common.Interfaces;

namespace Schefco.TaskFlow.API.MIddleware
{
    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ICurrentTokenService tokenService)
        {
            // Read the JWT from the cookie
            var token = context.Request.Cookies["AuthToken"];

            if (!string.IsNullOrEmpty(token))
            {
                // Store it in the service
                tokenService.Token = token;

                // Inject the token into the Authorization header for JwtBearer
                context.Request.Headers["Authorization"] = $"Bearer {token}";
            }

            await _next(context);
        }
    }
}

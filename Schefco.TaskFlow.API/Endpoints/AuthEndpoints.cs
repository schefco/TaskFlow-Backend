using Schefco.TaskFlow.Application.Common.Mediator;
using Schefco.TaskFlow.Application.Common.Interfaces;
using Schefco.TaskFlow.Application.Features.Auth;
using Schefco.TaskFlow.Application.Features.Auth.DTOs;
using Azure.Core;

namespace Schefco.TaskFlow.API.Endpoints
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/auth").WithTags("Auth");

            //
            // Public Registration
            // POST /auth/register
            // Creates a PendingUser request
            //
            group.MapPost("/register", async (SubmitRegistrationRequestCommand command, IMediator mediator) =>
            {
                await mediator.Send(command);
                return Results.Ok();
            });

            //
            // GET pending users /auth/pending
            //
            group.MapGet("/pending", async (IPendingUserRepository repo) =>
            {
                var pendingUsers = await repo.GetAllAsync();

                var result = pendingUsers.Select(u => new PendingUserDto(
                    u.Id,
                    u.Name,
                    u.Email,
                    u.Company,
                    u.Reason
                ));

                return Results.Ok(result);
            });

            //
            // GET approved users /auth/users
            //
            group.MapGet("/users", async (IUserRepository repo) =>
            {
                // Get user repo
                var users = await repo.GetAllAsync();

                // Pack results into DTO
                var result = users.Select(u => new UserDto(
                    u.Id,
                    u.Name,
                    u.Email,
                    u.Company
                ));

                return Results.Ok(result);
            }).RequireAuthorization("Owner");

            //
            // GET /auth/users/{id}
            //
            group.MapGet("/users/{id}", async (Guid id, IMediator mediator) =>
            {
                var result = await mediator.Send(new GetUserByIdQuery(id));
                return Results.Ok(result);
            }).RequireAuthorization("Owner");

            //
            // GET /auth/me
            // Return authoized user
            //
            group.MapGet("/me", async (ICurrentTokenService tokenService, IUserRepository users) =>
            {
                // Get token
                var token = tokenService.Token;

                if (string.IsNullOrEmpty(token))
                    return Results.Unauthorized();

                // Decode the token
                var userId = tokenService.GetCurrentUserId();

                if (userId == Guid.Empty)
                    return Results.Unauthorized();

                // Get the user
                var user = await users.GetByIdAsync(userId);

                if (user == null)
                    return Results.Unauthorized();

                // send back user info
                return Results.Ok(new
                {
                    userId = user.Id,
                    email = user.Email,
                    role = user.Role
                });
            });

            //
            // POST /auth/approve Approve pending users
            //
            group.MapPost("/approve", async (ApprovePendingUserCommand command, IMediator mediator) =>
            {
                var tempPassword = await mediator.Send(command);
                return Results.Ok(new { tempPassword });
            });

            //
            // Login
            // POST /auth/login
            //
            group.MapPost("/login", async (LoginCommand command, HttpContext http, IMediator mediator, IJwtTokenGenerator jwt, IUserRepository users, IPasswordHashingService hasher) =>
            {
                var result = await mediator.Send(command);

                // look up the user
                var user = await users.GetByEmailAsync(command.Email);

                if (user == null)
                    return Results.Unauthorized();

                // Generate the token
                string token = result.RequiresPasswordReset ? jwt.GenerateTempToken(user) : jwt.GenerateToken(user);

                // Write cookie
                http.Response.Cookies.Append("AuthToken", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddDays(7)
                });

                // return only the password reset flag
                return Results.Ok(new { 
                    requiresPasswordReset = result.RequiresPasswordReset 
                });
            });

            //
            // First time password reset
            // POST /auth/first-time-password
            //
            group.MapPost("/first-time-password", async (ResetTempPasswordCommand command,
                ICurrentTokenService tokenService,
                HttpContext http,
                IMediator mediator) =>
            {
                // extract userId from the token
                var userId = tokenService.GetCurrentUserId();

                if (userId == Guid.Empty)
                    return Results.Unauthorized();

                // Execute the password reset
                var result = await mediator.Send(command);

                return Results.Ok(result);
            });

            //
            // POST /reset-password
            //
            group.MapPost("/reset-password", async (ResetUserPasswordCommand command, ICurrentTokenService tokenService, IMediator mediator) =>
            {
                // Get userId from cookie
                var userId = tokenService.GetCurrentUserId();

                if (userId == Guid.Empty)
                    return Results.Unauthorized();

                // Execute the password reset
                var result = await mediator.Send(command);

                return Results.Ok(result);
            }).RequireAuthorization("Owner");

            //
            // POST /auth/logout
            // Clears the HttpOnly cookie
            //
            group.MapPost("/logout", (HttpContext http) =>
            {
                http.Response.Cookies.Append("AuthToken", "", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddDays(-1)
                });

                return Results.Ok();
            });

            //
            // DELETE /users/{id}
            //
            group.MapDelete("/users/{id}", async (Guid id, IUserRepository repo) =>
            {
                // find the user
                var user = await repo.GetByIdAsync(id);

                if (user == null)
                    return Results.NotFound();

                // Delete if found
                await repo.DeleteAsync(user);

                return Results.NoContent();
            }).RequireAuthorization("Owner");

            //
            // PATCH update /users/{id}
            //
            group.MapPatch("/users/{id}", async (UpdateUserCommand command, IMediator mediator) =>
            {
                await mediator.Send(command);

                return Results.NoContent();
            }).RequireAuthorization("Owner");
        }  
    }
}

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
            group.MapPost("/login", async (LoginCommand command, HttpContext http, IMediator mediator) =>
            {
                var result = await mediator.Send(command);

                // Extract token from result
                var token = result.Token;

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
                    token = result.Token,
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
                // Extract token from from Header
                var header = http.Request.Headers["Authorization"].ToString();
                var token = header.Replace("Bearer ", "");

                // Add to show the current token
                tokenService.Token = token;

                // Send to reset temp password
                return await mediator.Send(command);
            });

            //
            // POST /reset-password
            //
            group.MapPost("/reset-password", async (ResetUserPasswordCommand command, IMediator mediator) =>
            {
                var tempPassword = await mediator.Send(command);
                return Results.Ok(new { tempPassword });
            }).RequireAuthorization("Owner");

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

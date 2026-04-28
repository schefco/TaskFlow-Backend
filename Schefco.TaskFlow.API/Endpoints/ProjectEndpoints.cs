using Schefco.TaskFlow.Application.Common.Mediator;
using Schefco.TaskFlow.Application.Features.Projects.Commands.CreateProject;
using Schefco.TaskFlow.Application.Features.Projects.Commands.DeleteProject;
using Schefco.TaskFlow.Application.Features.Projects.Commands.UpdateProject;
using Schefco.TaskFlow.Application.Features.Projects.Queries.GetProjectById;
using Schefco.TaskFlow.Application.Features.Projects.Queries.GetProjects;
using Schefco.TaskFlow.Application.Features.Projects.Queries.GetProjectsByPriority;
using Schefco.TaskFlow.Application.Features.Projects.Queries.GetProjectsByStatus;
using Microsoft.AspNetCore.Mvc;
using Schefco.TaskFlow.Domain.Enums;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Schefco.TaskFlow.API.Endpoints
{
    public static class ProjectEndpoints
    {
        public static void MapProjectEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/projects").RequireAuthorization();

            group.RequireAuthorization();

            //
            // Projects Command
            //

            // Create Project
            group.MapPost("/", async ([FromBody]CreateProjectCommand command, HttpContext http, IMediator mediator) =>
            {
                Console.WriteLine("AUTH HEADER: " + http.Request.Headers["Authorization"]);
                var userId = http.User.FindFirst("sub")?.Value;

                var updatedCommand = command with
                {
                    CreatedByUserId = http.User.GetUserId()
                };

                var result = await mediator.Send(updatedCommand);
                return Results.Ok(result);
            }).RequireAuthorization();

            // Delete Project
            group.MapDelete("/{id:guid}", async (Guid id, HttpContext http, IMediator mediator) =>
            {
                var userId = http.User.GetUserId();
                var role = http.User.FindFirstValue(ClaimTypes.Role);

                await mediator.Send(new DeleteProjectCommand(id, userId, role));
                return Results.NoContent();
            });

            // Update Project
            group.MapPut("/{id:guid}", async (Guid id, [FromBody] UpdateProjectCommand command, IMediator mediator) =>
            {
                // Ensure the route ID override the body ID
                var updatedCommand = command with { Id = id };

                var result = await mediator.Send(updatedCommand);
                return Results.Ok(result);
            });

            //
            // Projects Query
            //

            // GET All
            group.MapGet("/", async (HttpContext http, IMediator mediator) =>
            {
                // Extract user info
                var userId = http.User.GetUserId();
                var role = http.User.FindFirstValue(ClaimTypes.Role);
                
                // Extract Sorting params
                var sortBy = http.Request.Query["sortBy"].ToString();
                var sortDirection = http.Request.Query["sortDirection"].ToString();

                // Extract pagination params with defaults
                var pageNumber = int.TryParse(http.Request.Query["pageNumber"], out var pn) ? pn : 1;
                var pageSize = int.TryParse(http.Request.Query["pageSize"], out var ps) ? ps : 20;

                // Create the query with all params
                var query = new GetProjectQuery(
                    userId,
                    role,
                    string.IsNullOrWhiteSpace(sortBy) ? null : sortBy, // defaults to null/empty string 
                    string.IsNullOrWhiteSpace(sortDirection) ? null : sortDirection,
                    pageNumber,
                    pageSize
                );

                return await mediator.Send(query);
            });

            // GET by Project ID
            group.MapGet("/{id:guid}", async (Guid id, HttpContext http, IMediator mediator) =>
            {
                var userId = http.User.GetUserId();
                var role = http.User.FindFirstValue(ClaimTypes.Role);

                var result = await mediator.Send(new GetProjectByIdQuery(id, userId, role));
                return Results.Ok(result);
            });

            // GET by Project Status
            group.MapGet("/status/{status}", async (ProjectStatus status, HttpContext http, IMediator mediator) =>
            {
                var userId = http.User.GetUserId();
                var role = http.User.FindFirstValue(ClaimTypes.Role);

                var result = await mediator.Send(new GetProjectsByStatusQuery(status, userId, role));
                return Results.Ok(result);
            });

            // GET by Project Priority
            group.MapGet("/priority/{priority}", async (ProjectPriority priority, HttpContext http, IMediator mediator) =>
            {
                var userId = http.User.GetUserId();
                var role = http.User.FindFirstValue(ClaimTypes.Role);

                var result = await mediator.Send(new GetProjectsByPriorityQuery(priority, userId, role));
                return Results.Ok(result);
            });
        }

        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var id = user.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(id))
                throw new Exception("User ID claim missing from token");

            return Guid.Parse(id);
        }

        public static string GetUserRole(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.Role);
        }
    }
}

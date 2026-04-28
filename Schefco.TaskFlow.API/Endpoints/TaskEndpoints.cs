using Schefco.TaskFlow.Domain.Enums;
using Schefco.TaskFlow.Application.Common.Mediator;
using Schefco.TaskFlow.Application.Features.Tasks.Commands.CreateTask;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using Schefco.TaskFlow.Domain.Entities;
using Schefco.TaskFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Schefco.TaskFlow.Application.Features.Tasks.Queries.GetTaskById;
using Schefco.TaskFlow.Application.Features.Tasks.Queries.GetSubtasks;
using Schefco.TaskFlow.Application.Features.Tasks.Queries.GetCommentsForTask;
using Schefco.TaskFlow.Application.Features.Tasks.Commands.AddComment;
using Schefco.TaskFlow.Application.Features.Tasks.Commands.DeleteComment;
using Schefco.TaskFlow.Application.Features.Tasks.Commands.EditComment;
using Schefco.TaskFlow.Application.Features.Tasks.Commands.DeleteTask;
using Schefco.TaskFlow.Application.Features.Tasks.Commands.UpdateTask;
using Schefco.TaskFlow.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Schefco.TaskFlow.Infrastructure.Services;

namespace Schefco.TaskFlow.API.Endpoints
{
    public static class TaskEndpoints
    {
        public static void MapTaskEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/tasks");

            group.RequireAuthorization();

            //
            // Task Commands
            //
            
            // POST create task
            group.MapPost("/", async ([FromBody] CreateTaskCommand command, ICurrentTokenService tokenService, IMediator mediator) =>
            {
                // send the command to the handler
                var result = await mediator.Send(command);

                return Results.Ok(result);
            });

            // POST add comment to task
            group.MapPost("/{taskId}/comments", async (Guid taskId, [FromBody] AddCommentToTaskCommand command, ICurrentTokenService tokenService, IMediator mediator) =>
            {
                // Inject userid from token
                var userId = tokenService.GetCurrentUserId();
                command = command with { UserId = userId };

                // Ensure the route taskId matches the body taskId
                if (taskId != command.TaskId)
                    return Results.BadRequest("TaskId in route and body must match.");

                var result = await mediator.Send(command);
                return Results.Ok(result);
            });

            // POST create subtask
            group.MapPost("/{parentTaskId}/subtasks", async (Guid parentTaskId, [FromBody] CreateTaskCommand command, AppDbContext db, IMediator mediator) =>
            {
                // Validate parent exists
                var parent = await db.Tasks.FindAsync(parentTaskId);
                if (parent == null)
                    return Results.NotFound("Parent task not found.");

                // Override ProjectId and ParentTaskId
                var updated = command with
                {
                    ProjectId = parent.ProjectId,
                    ParentTaskId = parent.Id
                };

                // Create subtask
                var result = await mediator.Send(updated);

                return Results.Ok(result);
            });

            // PUT update task
            group.MapPut("/{taskId}", async (Guid taskId, [FromBody] UpdateTaskCommand command, ICurrentTokenService tokenService, IMediator mediator) =>
            {
                // Make sure we are updating the correct task
                if (taskId != command.TaskId)
                    return Results.BadRequest("TaskId in route and body must match.");

                // Inject current user ID from the JWT
                var userId = tokenService.GetCurrentUserId();
                command = command with { UserId = userId };

                // if match is good update task
                var updated = await mediator.Send(command);

                // Return updated task
                return Results.Ok(updated);
            });

            // PATCH status udpates
            group.MapPatch("/{id:guid}/status", async (Guid id, [FromBody] TaskEntityStatus newStatus, AppDbContext db) =>
            {
                // Only update the status and UpdatedAt
                var task = await db.Tasks.FindAsync(id);

                if (task is null)
                    return Results.NotFound();

                task.Status = newStatus;
                task.UpdatedAt = DateTime.UtcNow;

                // Set completed date when moving to Done
                if (newStatus == TaskEntityStatus.Done)
                {
                    task.CompletedAt = DateTime.UtcNow;
                }
                else
                {
                    task.CompletedAt = null;
                }

                await db.SaveChangesAsync();

                return Results.Ok(new
                {
                    task.Id,
                    task.Status,
                    task.CompletedAt
                });
            });

            // PATCH update comment
            group.MapPatch("/comments/{commentId}", async (Guid commentId, [FromBody] EditCommentCommand command, ICurrentTokenService tokenService, IMediator mediator) =>
            {
                // Inject userId from token attach to command
                var userId = tokenService.GetCurrentUserId();
                command = command with { UserId = userId };

                // check this is correct comment to update
                if (commentId != command.CommentId)
                    return Results.BadRequest("CommentId in route and body must match.");

                // If match is good, update comment
                var result = await mediator.Send(command);
                return Results.Ok(result);
            });

            // DELETE task
            group.MapDelete("/{taskId}", async (Guid taskId, ICurrentTokenService tokenService, IMediator mediator) =>
            {
                // Get the user id from the current token
                var userId = tokenService.GetCurrentUserId();

                // Push taskId and userId to DeleteTaskCommand
                var command = new DeleteTaskCommand(taskId, userId);

                // if match is good delete task
                var result = await mediator.Send(command);

                return result ? Results.NoContent() : Results.BadRequest();
            });

            // DELETE comment
            group.MapDelete("/comments/{commentId}", async (Guid commentId, [FromBody] DeleteCommentCommand command, ICurrentTokenService tokenService, IMediator mediator) =>
            {
                // Inject userId from token attach to command
                var userId = tokenService.GetCurrentUserId();
                command = command with { UserId = userId };

                // Confirm we are deleting the right comment
                if (commentId != command.CommentId)
                    return Results.BadRequest("CommentId in route and body must match.");

                // If match is good delete comment
                var result = await mediator.Send(command);

                return result ? Results.NoContent() : Results.BadRequest();
            });

            //
            // Tasks Query
            //

            // GET get all tasks
            group.MapGet("/", async (AppDbContext db) =>
            {
                var tasks = await db.Tasks.Select(t => new
                {
                    t.Id,
                    t.Title,
                    t.Status,
                    t.Priority,
                    t.DueDate,
                    t.ProjectId,
                    t.ParentTaskId
                }).ToListAsync();

                return Results.Ok(tasks);
            });

            // GET tasks for a project
            group.MapGet("/project/{projectId}", async (Guid projectId, AppDbContext db) =>
            {
                var tasks = await db.Tasks.Where(t => t.ProjectId == projectId)
                .Select(t => new
                {
                    t.Id,
                    t.Title,
                    t.Description,
                    t.Status,
                    t.Priority,
                    t.DueDate,
                    t.ProjectId,
                    t.ParentTaskId,
                    t.CreatedAt,
                    t.UpdatedAt,
                    t.CompletedAt,
                    t.UserId,
                    t.SortOrder
                }).ToListAsync();

                return Results.Ok(tasks);
            });

            // GET tasks by Id
            group.MapGet("/{taskId}", async (Guid taskId, IMediator mediator) =>
            {
                var result = await mediator.Send(new GetTaskByIdQuery(taskId));
                return Results.Ok(result);
            });

            // GET subtasks
            group.MapGet("/{taskId}/subtasks", async (Guid taskId, IMediator mediator) =>
            {
                // This is to lazy-load additional subtasks for subtasks that contain subtasks
                var result = await mediator.Send(new GetSubtasksForTaskQuery(taskId));
                return Results.Ok(result);
            });

            // GET comments for Task
            group.MapGet("/{taskId}/comments", async (Guid taskId, IMediator mediator) =>
            {
                var result = await mediator.Send(new GetCommentsForTaskQuery(taskId));
                return Results.Ok(result);
            });
        }
    }
}


using Schefco.TaskFlow.Application.Common.Interfaces;
using Schefco.TaskFlow.Application.Common.Mediator;
using Schefco.TaskFlow.Application.Features.Tasks.Queries.GetCommentsForTask;

namespace Schefco.TaskFlow.Application.Features.Tasks.Commands.DeleteComment
{
    public class DeleteCommentHandler : ICommandHandler<DeleteCommentCommand, bool>
    {
        private readonly ITaskRepository _taskRepo;
        private readonly IUserRepository _users;

        public DeleteCommentHandler(ITaskRepository taskRepo, IUserRepository users)
        {
            _taskRepo = taskRepo;
            _users = users;
        }

        public async Task<bool> Handle(DeleteCommentCommand command, CancellationToken cancellationToken)
        {
            // Load in the comment
            var comment = await _taskRepo.GetCommentByIdAsync(command.CommentId);

            if (comment is null)
                throw new Exception("Comment not found.");

            // Load the comment creator
            var commentCreator = await _users.GetByIdAsync(comment.UserId);
            if (commentCreator == null)
                throw new UnauthorizedAccessException("Comment creator does not exist");

            // Load the current user
            var currentUser = await _users.GetByIdAsync(command.UserId);
            if (currentUser == null)
                throw new UnauthorizedAccessException("User not found");

            // Authorization check
            // If the comment was created by an Owner
            if (commentCreator.Role == "Owner")
            {
                // Only Owners can delete Owner-created comments
                if (currentUser.Role != "Owner")
                    throw new UnauthorizedAccessException("Only Owners can delete Owner-created comments");
            }
            else
            {
                // Comment was created by a Recruiter
                // Recruiters can only delete their own comments
                if (currentUser.Role != "Owner" && comment.UserId != command.UserId)
                    throw new UnauthorizedAccessException("You do not have permission to delete this comment");
            }

            // Delete if no issues
            await _taskRepo.DeleteCommentAsync(comment);

            return true;
        }
    }
}

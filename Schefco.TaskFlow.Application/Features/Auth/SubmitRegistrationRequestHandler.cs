using Schefco.TaskFlow.Application.Common.Mediator;
using Schefco.TaskFlow.Application.Common.Interfaces;
using Schefco.TaskFlow.Domain.Entities;

namespace Schefco.TaskFlow.Application.Features.Auth
{
    public class SubmitRegistrationRequestHandler : ICommandHandler<SubmitRegistrationRequestCommand, string>
    {
        private readonly IPendingUserRepository _repo;
        private readonly IUserRepository _users;
        private readonly IEmailService _emailService;

        public SubmitRegistrationRequestHandler(IPendingUserRepository repo, IEmailService emailService, IUserRepository users)
        {
            _repo = repo;
            _emailService = emailService;
            _users = users;
        }

        public async Task<string> Handle(SubmitRegistrationRequestCommand command, CancellationToken cancellationToken)
        {
            // Normalize email for comparison
            var email = command.Email.ToLower().Trim();

            // Check is the user has already registered or is already an active user
            // Pending users
            if (await _repo.EmailExistAsync(email))
                throw new Exception("A registration request with this email is already pending.");

            // Check active users
            if (await _users.EmailExistsAsync(email))
                throw new Exception("An account with this email already exists.");
            
            // Building the pending user record from the registration form
            // This is what sits in the queue until an admin approves it
            var pending = new PendingUser(
                command.Name,
                command.Email,
                command.Company,
                command.Reason
                );

            // drop it into the PendingUsers table through the repo
            await _repo.AddAsync(pending);

            // Send pending approval email
            await _emailService.SendPendingApprovalEmailAsync(command.Email, command.Name);

            // Return the id so the frontend can confirm the request was received
            return pending.Id.ToString();
        }
    }
}

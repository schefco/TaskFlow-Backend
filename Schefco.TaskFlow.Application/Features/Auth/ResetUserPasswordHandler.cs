using Schefco.TaskFlow.Application.Common.Interfaces;
using Schefco.TaskFlow.Application.Common.Mediator;
using Schefco.TaskFlow.Application.Common.Utilities;
using Schefco.TaskFlow.Application.Features.Auth.DTOs;

namespace Schefco.TaskFlow.Application.Features.Auth
{
    public class ResetUserPasswordHandler : ICommandHandler<ResetUserPasswordCommand, string>
    {
        private readonly IUserRepository _users;
        private readonly IPasswordHashingService _hasher;
        private readonly IEmailService _email;

        public ResetUserPasswordHandler(IUserRepository users, IPasswordHashingService hasher, IEmailService email)
        {
            _users = users;
            _hasher = hasher;
            _email = email;
        }

        public async Task<string> Handle(ResetUserPasswordCommand command, CancellationToken cancellationToken)
        {
            // Load in user
            var user = await _users.GetByIdAsync(command.UserId);

            if (user == null)
                throw new Exception("User not found");

            // Generate temp password
            var tempPassword = PasswordGenerator.GeneratePassword();

            // Hash and store
            user.PasswordHash = _hasher.HashPassword(user, tempPassword);

            // Force them to change password on next login
            user.MustChangePassword = true;

            await _users.UpdateAsync(user);

            // Send email with temp password
            await _email.SendTempPasswordEmailAsync(user.Email, user.Name, tempPassword);

            // Return temp password to the frontend
            return tempPassword;
        }
    }
}

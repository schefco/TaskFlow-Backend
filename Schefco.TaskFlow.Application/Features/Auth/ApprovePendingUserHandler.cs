using Schefco.TaskFlow.Application.Common.Interfaces;
using Schefco.TaskFlow.Application.Common.Mediator;
using Schefco.TaskFlow.Application.Common.Utilities;
using Schefco.TaskFlow.Domain.Entities;
using System.Runtime.InteropServices;

namespace Schefco.TaskFlow.Application.Features.Auth
{
    public class ApprovePendingUserHandler : ICommandHandler<ApprovePendingUserCommand, string>
    {
        private readonly IPendingUserRepository _pendingRepo;
        private readonly IUserRepository _userRepo;
        private readonly IPasswordHashingService _hasher;
        private readonly IEmailService _emailService;

        public ApprovePendingUserHandler(IPendingUserRepository pendingRepo, IPasswordHashingService hasher, IUserRepository userRepo, IEmailService emailService)
        {
            _pendingRepo = pendingRepo;
            _hasher = hasher;
            _userRepo = userRepo;
            _emailService = emailService;
        }

        public async Task<string> Handle(ApprovePendingUserCommand command, CancellationToken cancellationToken)
        {
            // Find the user to approve from the PendingUsers repo
            var pendingUser = await _pendingRepo.GetByIdAsync(command.pendingUserId);

            // If not user, throw Exception
            if (pendingUser == null)
                throw new Exception("User not found");

            // Generate a structured 10-char temp password for first time login
            var tempPassword = PasswordGenerator.GeneratePassword();

            // Convert to AppUser
            var approvedUser = new AppUser(
                pendingUser.Id,
                pendingUser.Name,
                pendingUser.Email,
                pendingUser.Company,
                ""
            );

            // Hash the temp password before creating the AppUser
            var tempHash = _hasher.HashPassword(approvedUser, tempPassword);

            // Set the password hash after the user is created in User repo
            approvedUser.PasswordHash = tempHash;

            // save to User Repo
            await _userRepo.AddAsync(approvedUser);

            // Send temp password email
            try
            {
                await _emailService.SendTempPasswordEmailAsync(approvedUser.Email, approvedUser.Name, tempPassword);
            } catch
            {
                Console.WriteLine("Email failed to send.");
            }
            
            // Remove user from Pending Repo
            await _pendingRepo.DeleteAsync(pendingUser);

            // Return temp password for user to login and change
            return tempPassword;
        }
    }
}

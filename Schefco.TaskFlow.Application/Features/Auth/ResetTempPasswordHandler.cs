using Microsoft.IdentityModel.JsonWebTokens;
using Schefco.TaskFlow.Application.Common.Interfaces;
using Schefco.TaskFlow.Application.Common.Mediator;
using Schefco.TaskFlow.Application.Features.Auth.DTOs;

namespace Schefco.TaskFlow.Application.Features.Auth
{
    public class ResetTempPasswordHandler : ICommandHandler<ResetTempPasswordCommand, LoginResponseDto>
    {
        private readonly IUserRepository _users;  // Access the stored users
        private readonly IPasswordHashingService _hasher;  // Verifies password is correct
        private readonly IJwtTokenGenerator _jwt;  // Generates JWT on successful login
        private readonly ICurrentTokenService _currentTokenService; // Provides the current token

        // Inject required services
        public ResetTempPasswordHandler(
            IUserRepository users,
            IPasswordHashingService hasher,
            IJwtTokenGenerator jwt,
            ICurrentTokenService currentTokenService)
        {
            _users = users;
            _hasher = hasher;
            _jwt = jwt;
            _currentTokenService = currentTokenService;
        }

        public async Task<LoginResponseDto> Handle(ResetTempPasswordCommand command, CancellationToken cancellationToken)
        {
            // Extracted token from Authorization header
            var token = _currentTokenService.Token;

            if (token == null)
                throw new Exception("Authorization token missing.");

            // Validate our temp token is for a password reset
            var principal = _jwt.ValidateTempToken(token!);

            // Extract user ID from the validate principal
            var userId = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (userId == null)
                throw new Exception("Invalid token payload.");

            // Load and then update the user
            var user = await _users.GetByIdAsync(Guid.Parse(userId));

            if (user == null)
                throw new Exception("User not found.");

            // Update user data
            user.PasswordHash = _hasher.HashPassword(user, command.NewPassword);
            user.MustChangePassword = false;

            await _users.UpdateAsync(user);

            // Generate full JWT
            var fullToken = _jwt.GenerateToken(user);

            // Return Response
            return new LoginResponseDto
            {
                Token = fullToken,
                RequiresPasswordReset = false
            };
        }
    }
}

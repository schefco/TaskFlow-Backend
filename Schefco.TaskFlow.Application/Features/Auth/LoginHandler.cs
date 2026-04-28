using Schefco.TaskFlow.Application.Common.Mediator;
using Schefco.TaskFlow.Application.Common.Interfaces;
using Schefco.TaskFlow.Application.Features.Auth.DTOs;

namespace Schefco.TaskFlow.Application.Features.Auth
{
    // Handles user login requests
    public class LoginHandler : ICommandHandler<LoginCommand, LoginResponseDto>
    {
        private readonly IUserRepository _users;  // Access the stored users
        private readonly IPasswordHashingService _hasher;  // Verifies password is correct
        private readonly IJwtTokenGenerator _jwt;  // Generates JWT on successful login

        // Inject required services
        public LoginHandler(
            IUserRepository users,
            IPasswordHashingService hasher,
            IJwtTokenGenerator jwt)
        {
            _users = users;
            _hasher = hasher;
            _jwt = jwt;
        }

        public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // Look up the user by email
            var user = await _users.GetByEmailAsync(request.Email);

            // If no user found, login fails
            if (user == null)
                throw new Exception("Invalid email or password");

            // If first login, redirect to password reset
            if (user.MustChangePassword)
            {
                var tempToken = _jwt.GenerateTempToken(user);

                return new LoginResponseDto
                {
                    Token = tempToken,
                    RequiresPasswordReset = true
                };
            }

            // Verify the password
            var result = _hasher.VerifyPassword(user, request.Password);

            // If password doesn't match, login fails
            if (!result)
                throw new Exception("Invalid email or password");

            // Generate a JWT for the authenticated user
            var fullToken = _jwt.GenerateToken(user);

            // Return login response
            return new LoginResponseDto
            {
                Token = fullToken,
                RequiresPasswordReset = false
            };
        }
    }
}

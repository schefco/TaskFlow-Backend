
using Schefco.TaskFlow.Application.Common.Interfaces;
using Schefco.TaskFlow.Application.Common.Mediator;
using Schefco.TaskFlow.Application.Features.Auth.DTOs;

namespace Schefco.TaskFlow.Application.Features.Auth
{
    public class GetUserByIdHandler : IQueryHandler<GetUserByIdQuery, UserDetailsDto>
    {
        private readonly IUserRepository _users;

        public GetUserByIdHandler(IUserRepository users)
        {
            _users = users;
        }

        public async Task<UserDetailsDto> Handle(GetUserByIdQuery command, CancellationToken cancellationToken)
        {
            // find the user
            var user = await _users.GetByIdAsync(command.UserId);

            if (user == null)
                throw new Exception("User not found");

            // If found, show the results
            return new UserDetailsDto(
                user.Id,
                user.Name,
                user.Email,
                user.Company,
                user.Role
            );
        }
    }
}

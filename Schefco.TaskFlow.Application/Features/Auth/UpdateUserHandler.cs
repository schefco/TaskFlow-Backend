using Schefco.TaskFlow.Application.Common.Interfaces;
using Schefco.TaskFlow.Application.Common.Mediator;
using Schefco.TaskFlow.Application.Features.Auth.DTOs;

namespace Schefco.TaskFlow.Application.Features.Auth
{
    public class UpdateUserHandler : ICommandHandler<UpdateUserCommand, UpdateUserRequestDto>
    {
        private readonly IUserRepository _users;

        public UpdateUserHandler(IUserRepository users)
        {
            _users = users;
        }

        public async Task<UpdateUserRequestDto> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
        {
            // Find the user
            var user = await _users.GetByIdAsync(command.UserId);

            if (user == null)
                throw new Exception("user not found");

            user.Name = command.Name;
            user.Company = command.Company;

            await _users.UpdateAsync(user);

            return new UpdateUserRequestDto(
                user.Name,
                user.Company
            );
        }
    }
}

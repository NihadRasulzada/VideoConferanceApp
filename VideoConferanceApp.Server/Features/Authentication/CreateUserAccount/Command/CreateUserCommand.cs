using MediatR;
using VideoConferanceApp.Shared.Authentication.Requests;
using VideoConferanceApp.Shared.Authentication.Responces;

namespace VideoConferanceApp.Server.Features.Authentication.CreateUserAccount.Command
{
    public record CreateUserCommand(CreateUserRequest User) : IRequest<CreateUserResponse>;
}   
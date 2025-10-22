using MediatR;
using VideoConferanceApp.Shared.Authentication.Requests;
using VideoConferanceApp.Shared.Authentication.Responces;

namespace VideoConferanceApp.Server.Features.Authentication.LoginUser.Command
{
    public record LoginUserCommand(LoginUserRequest Login) : IRequest<LoginUserResponse>;
}
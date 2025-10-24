using MediatR;
using VideoConferanceApp.Shared.Authentication.Requests;
using VideoConferanceApp.Shared.Authentication.Responces;

namespace VideoConferanceApp.Server.Features.Authentication.GetNewToken.Command;

public record CreateNewTokenCommand(RefreshTokenRequest RefreshToken)
    : IRequest<RefreshTokenResponse>;
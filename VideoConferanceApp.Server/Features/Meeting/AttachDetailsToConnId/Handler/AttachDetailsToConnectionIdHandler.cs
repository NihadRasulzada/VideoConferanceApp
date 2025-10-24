using MediatR;
using Microsoft.EntityFrameworkCore;
using VideoConferanceApp.Server.Features.Meeting.AttachDetailsToConnId.Command;
using VideoConferanceApp.Server.Infrastructure.Data;
using VideoConferanceApp.Shared.Meeting.Responses;

namespace VideoConferanceApp.Server.Features.Meeting.AttachDetailsToConnId.Handler;

public class AttachDetailsToConnectionIdHandler(AppDbContext context)
    : IRequestHandler
        <AttachDetailsToConnectionIdCommand, AttachDetailsToConnectionIdResponse>
{
    public async Task<AttachDetailsToConnectionIdResponse> 
        Handle(AttachDetailsToConnectionIdCommand request, CancellationToken cancellationToken)
    {
        var connectedUser = await context.ConnectedUsers
            .FirstOrDefaultAsync
            (_ => _.ConnectionId == request.DetailsToConnectionId.ConnectionId,
                cancellationToken: cancellationToken);

        if (connectedUser == null)
            return new() { IsSuccess = false, Message = "Connected user not found" };

        if (connectedUser != null && connectedUser.UserId != 
            request.DetailsToConnectionId.UserId)
        {
            connectedUser.UserId = request.DetailsToConnectionId.UserId;
            connectedUser.UserName = request.DetailsToConnectionId.Name!;
            await context.SaveChangesAsync(cancellationToken);
        }
        return new() { IsSuccess = true, Message = "Done" };
    }
}
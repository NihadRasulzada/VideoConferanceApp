using MediatR;
using Microsoft.EntityFrameworkCore;
using VideoConferanceApp.Server.Features.Meeting.GetMeetingMembers.Query;
using VideoConferanceApp.Server.Infrastructure.Data;
using VideoConferanceApp.Shared.Meeting.Responses;

namespace VideoConferanceApp.Server.Features.Meeting.GetMeetingMembers.Handler;

public class GetMeetingMembersHandler(AppDbContext context) :
    IRequestHandler<GetMeetingMembersQuery, GetMeetingMembersResponse>
{
    public async Task<GetMeetingMembersResponse>
        Handle(GetMeetingMembersQuery request, CancellationToken cancellationToken)
    {
        var meeting = await context.ActiveMeetings.AsNoTracking()
            .Where(_ => _.MeetingId == request.MeetingMembers.MeetingId)
            .ToListAsync(cancellationToken: cancellationToken);

        // check if members found.
        if (meeting == null)
            return new() { IsSuccess = false, Message = "No meeting found" };

        // get only the connected users Ids
        IEnumerable<string> userIds = [.. meeting.Select(x => x.UserId)];

        List<string> MembersName = [];
        foreach (var userId in userIds)
        {
            var connectedUser = await context.ConnectedUsers.AsNoTracking()
                .FirstOrDefaultAsync
                    (x => x.UserId == userId, cancellationToken: cancellationToken);
            if (connectedUser != null)
                MembersName.Add(connectedUser.UserName);
        }

        return new()
        {
            IsSuccess = true,
            Message = $"{MembersName.Count} members found",
            Data = MembersName
        };
    }
}
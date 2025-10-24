using Microsoft.EntityFrameworkCore;
using VideoConferanceApp.Server.Infrastructure.Data;

namespace VideoConferanceApp.Server.Helpers;

public interface IGetUsersConnectionIdsByMeetingIdHelper
{
    Task<HashSet<string>> Handle(string meetingId);
}

public class GetUsersConnectionIdsByMeetingIdHelper(AppDbContext context) : 
    IGetUsersConnectionIdsByMeetingIdHelper
{
    public async Task<HashSet<string>> Handle(string meetingId)
    {
        IEnumerable<string> connectedUserIds =
            (await context.ActiveMeetings.Where(_ => _.MeetingId == meetingId)
                .AsNoTracking().ToListAsync()).Select(x => x.UserId);

        if (connectedUserIds == null)
            return [];

        // Store all users signalR connection IDs
        HashSet<string> userConnectionIds = [];
        foreach (var connectedUser in connectedUserIds)
        {
            var user = await context.ConnectedUsers.AsNoTracking()
                .FirstOrDefaultAsync(_ => _.UserId == connectedUser);
            if (user != null)
                userConnectionIds.Add(user.ConnectionId);
        }
        return userConnectionIds;
    }
}
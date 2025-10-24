using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using VideoConferanceApp.Server.Helpers;
using VideoConferanceApp.Server.Infrastructure.Data;
using VideoConferanceApp.Server.Models;

namespace VideoConferanceApp.Server.Hubs;

public class MeetingHub(
    AppDbContext appDbContext,
    IGetUsersConnectionIdsByMeetingIdHelper getUsersConnectionIdsByMeetingId,
    IMeetingHubHelper meetingHubHelper) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var user = new ConnectedUser() { ConnectionId = Context.ConnectionId };
        if (Context.User!.Identity!.IsAuthenticated)
        {
            user.UserId = meetingHubHelper.GetUserIdFromHubContext(Context);
            user.UserName = meetingHubHelper.GetUserNameFromHubContext(Context);
        }
        else
        {
            user.UserId = "Anonymous";
            user.UserName = "Anonymous";
        }

        appDbContext.ConnectedUsers.Add(user);
        await appDbContext.SaveChangesAsync();

        await Clients.Client(Context.ConnectionId).SendAsync("Connected",
            user.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var user = await meetingHubHelper
            .FindConnectedUserByConnectionIdWithTracking(Context.ConnectionId);
        if (user == null) return;

        appDbContext.ConnectedUsers.Remove(user);

        var _user = await meetingHubHelper
            .FindActiveMeetingByUserWithTracking(user.UserId);
        if (user != null)
            appDbContext.ActiveMeetings.Remove(_user!);

        await appDbContext.SaveChangesAsync();
    }

    [Authorize]
    public async Task InitializeMeeting(string meetingId)
    {
        string UserId = meetingHubHelper.GetUserIdFromHubContext(Context);

        var meeting = await meetingHubHelper.FindMeetingByUser(meetingId, UserId) ??
                      throw new HubException("Meeting does not exist");

        var activeMeeting = await meetingHubHelper
            .FindActiveMeetingByUser(meetingId, UserId);
        if (activeMeeting != null)
            throw new HubException("Meeting already started");

        await meetingHubHelper.AddActiveMeeting(new ActiveMeeting
        {
            MeetingId = meeting.MeetingId,
            UserId = UserId
        });
        return;
    }


    [AllowAnonymous]
    public async Task JoinMeeting(string meetingId, string userId, string passcode)
    {
        _ = await meetingHubHelper.FindMeetingByPasscode(meetingId, passcode)
            ?? throw new HubException("Invalid meeting credentials provided");

        var userInMeeting = await meetingHubHelper.FindActiveMeetingByUser(userId);
        if (userInMeeting != null)
            throw new HubException("You have already joined the meeting");

        await meetingHubHelper.AddActiveMeeting(new ActiveMeeting
        {
            MeetingId = meetingId,
            UserId = userId
        });

        var userConnectionIds = await getUsersConnectionIdsByMeetingId.Handle(meetingId);
        IReadOnlyList<string> userIds = [.. userConnectionIds];
        await Clients.Clients(userIds).SendAsync("GetConnectedUsers");
        return;
    }
}
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using VideoConferanceApp.Server.Infrastructure.Data;
using VideoConferanceApp.Server.Models;

namespace VideoConferanceApp.Server.Helpers;

public interface IMeetingHubHelper
    {
        Task<Meeting?> FindMeetingByUser(string meetingId, string userId);
        Task<Meeting?> FindMeetingByPasscode(string meetingId, string passcode);
        Task<ActiveMeeting?> FindActiveMeetingByUserWithTracking(string userId);
        Task<ActiveMeeting?> FindActiveMeetingByUser(string userId);
        Task<ActiveMeeting?> FindActiveMeetingByUser(string userId, string meetingId);
        Task<ConnectedUser?> FindConnectedUserByConnectionIdWithTracking
            (string connectionId);
        Task AddActiveMeeting(ActiveMeeting activeMeeting);
        string GetUserIdFromHubContext(HubCallerContext hubCallerContext);
        string GetUserNameFromHubContext(HubCallerContext hubCallerContext);
    }

    public class MeetingHubHelper(AppDbContext appDbContext) : IMeetingHubHelper
    {
        public async Task AddActiveMeeting(ActiveMeeting activeMeeting)
        {
            appDbContext.ActiveMeetings.Add(new ActiveMeeting
            {
                MeetingId = activeMeeting.MeetingId,
                UserId = activeMeeting.UserId
            });
            await appDbContext.SaveChangesAsync();
        }

        public async Task<ActiveMeeting?> FindActiveMeetingByUser(string userId)
       => await appDbContext.ActiveMeetings.AsNoTracking()
            .FirstOrDefaultAsync(_ => _.UserId == userId);

        public async Task<ActiveMeeting?> FindActiveMeetingByUser(string userId, string meetingId) =>
        await appDbContext.ActiveMeetings.AsNoTracking()
                    .FirstOrDefaultAsync(_ => _.UserId == userId && _.MeetingId == meetingId);

        public async Task<ActiveMeeting?> FindActiveMeetingByUserWithTracking(string userId) =>
         await appDbContext.ActiveMeetings.AsNoTracking()
            .FirstOrDefaultAsync(_ => _.UserId == userId);

        public async Task<ConnectedUser?> FindConnectedUserByConnectionIdWithTracking(string connectionId)
        => await appDbContext.ConnectedUsers
            .FirstOrDefaultAsync(_ => _.ConnectionId == connectionId);

        public async Task<Meeting?> FindMeetingByPasscode(string meetingId, string passcode) =>
       await appDbContext.Meetings.AsNoTracking()
                .FirstOrDefaultAsync(_ => _.MeetingId == meetingId && _.Passcode == passcode);

        public async  Task<Meeting?> FindMeetingByUser(string meetingId, string userId) =>
         await appDbContext.Meetings.AsNoTracking()
               .FirstOrDefaultAsync(_ => _.MeetingId == meetingId && _.HostId == userId);

        public string GetUserIdFromHubContext(HubCallerContext hubCallerContext) =>
        hubCallerContext.User!.Claims
            .FirstOrDefault(_ => _.Type == ClaimTypes.NameIdentifier)!.Value;

        public string GetUserNameFromHubContext(HubCallerContext hubCallerContext) =>
        hubCallerContext.User!.Claims
               .FirstOrDefault(_ => _.Type == ClaimTypes.Name)!.Value;
    }
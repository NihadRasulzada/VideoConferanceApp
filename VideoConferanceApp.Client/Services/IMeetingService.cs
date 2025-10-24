using System.Net.Http.Json;
using VideoConferanceApp.Client.Extensions;
using VideoConferanceApp.Shared.Meeting.Requests;
using VideoConferanceApp.Shared.Meeting.Responses;

namespace VideoConferanceApp.Client.Services
{
    public interface IMeetingService
    {
        Task<CreateMeetingResponse?> CreateMeeting(CreateMeetingRequest meeting);
        Task<GetMeetingsResponse?> GetMeetings(string hostId);
        Task<GetRecentMeetingsResponse?> GetRecentMeetings(string hostId);

        Task<AttachDetailsToConnectionIdResponse?> AttachDetailsToConnectionId
            (AttachDetailsToConnectionIdRequest attachNameToConnection);

        Task<GetMeetingMembersResponse?> GetConnectedMembers(string meetingId);
        Task<ClearMeetingResponse?> ClearMeeting(string meetingId);
    }

    public class MeetingService(IHttpExtension httpExtension) : IMeetingService
    {
        public async Task<AttachDetailsToConnectionIdResponse?> AttachDetailsToConnectionId
            (AttachDetailsToConnectionIdRequest attachNameToConnection)
        {
            var result = await httpExtension.GetPublicClient()
                .PostAsJsonAsync($"meeting/attachDetailsToConnId", attachNameToConnection);
            return await result.Content
                .ReadFromJsonAsync<AttachDetailsToConnectionIdResponse>();
        }

        public async Task<ClearMeetingResponse?> ClearMeeting(string meetingId)
        {
            var result = await (await httpExtension.GetPrivateClient())
                .GetAsync($"meeting/clear/{meetingId}");
            return await result.Content.ReadFromJsonAsync<ClearMeetingResponse>();
        }

        public async Task<CreateMeetingResponse?> CreateMeeting(CreateMeetingRequest meeting)
        {
            try
            {
                var result = await (await httpExtension.GetPrivateClient())
                    .PostAsJsonAsync("meeting/create", meeting);
                return await result.Content.ReadFromJsonAsync<CreateMeetingResponse>();
            }
            catch
            {
                return new CreateMeetingResponse { IsSuccess = false, Message = "Error connecting to server" };
            }
        }

        public async Task<GetMeetingMembersResponse?> GetConnectedMembers(string meetingId)
        {
            var result = await httpExtension.GetPublicClient()
                .GetAsync($"meeting/members/{meetingId}");
            return await result.Content.ReadFromJsonAsync<GetMeetingMembersResponse>();
        }

        public async Task<GetMeetingsResponse?> GetMeetings(string hostId)
        {
            try
            {
                var result = await (await httpExtension
                    .GetPrivateClient()).GetAsync($"meeting/{hostId}");
                return await result.Content.ReadFromJsonAsync<GetMeetingsResponse>();
            }
            catch
            {
                return new GetMeetingsResponse { IsSuccess = false, Message = "Error connecting to server" };
            }
        }

        public async Task<GetRecentMeetingsResponse?> GetRecentMeetings(string hostId)
        {
            try
            {
                var result = await (await httpExtension.GetPrivateClient())
                    .GetAsync($"meeting/recent/{hostId}");
                return await result.Content.ReadFromJsonAsync<GetRecentMeetingsResponse>();
            }
            catch
            {
                return new GetRecentMeetingsResponse { IsSuccess = false, Message = "Error connecting to server" };
            }
        }
    }
}
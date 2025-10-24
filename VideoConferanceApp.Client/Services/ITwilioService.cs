using System.Net.Http.Json;
using Microsoft.JSInterop;
using VideoConferanceApp.Client.Extensions;
using VideoConferanceApp.Shared.Meeting.Responses;

namespace VideoConferanceApp.Client.Services
{
    public interface ITwilioService
    {
        Task<TwilioServiceResponse?> GenerateMeetingToken(string username, string meetingId);
        Task JoinMeeting(string token, string roomName, string containerId);

        Task<TwilioServiceResponse?> CloseMeeting(string meetingId);
        Task LeaveMeeting(string containerId);
        Task ToggleMic();
    }

    public class TwilioService(IJSRuntime _js, IHttpExtension httpExtension) : ITwilioService
    {
        public async Task<TwilioServiceResponse?> CloseMeeting(string meetingId)
        {
            var result = await (await httpExtension.GetPrivateClient())
                .GetAsync($"twilio/end-meeting/{meetingId}");
            return await result.Content.ReadFromJsonAsync<TwilioServiceResponse>();
        }

        public async Task<TwilioServiceResponse?> GenerateMeetingToken(string username, string meetingId)
        {
            var result = await httpExtension.GetPublicClient()
                .GetAsync($"twilio/token/{username}/{meetingId}");
            return await result.Content.ReadFromJsonAsync<TwilioServiceResponse>();
        }

        public async Task JoinMeeting(string token, string roomName, string containerId) =>
            await _js
                .InvokeVoidAsync("window.twilioVideo.connectToRoom", token, roomName, containerId);

        public async Task LeaveMeeting(string containerId)
        {
            await _js.InvokeVoidAsync("window.twilioVideo.leaveRoom", containerId);
        }


        public async Task ToggleMic()
        {
            await _js.InvokeVoidAsync("window.twilioVideo.toggleAudio");
        }
    }
}
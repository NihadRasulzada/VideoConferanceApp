using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using VideoConferanceApp.Server.Helpers;
using VideoConferanceApp.Server.Hubs;
using VideoConferanceApp.Server.Infrastructure.Services;
using VideoConferanceApp.Shared.Meeting.Responses;

namespace VideoConferanceApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TwilioController(
        IVideoService videoService,
        IHubContext<MeetingHub> hubContext,
        IGetUsersConnectionIdsByMeetingIdHelper getUsersConnectionIdsByMeetingId) : ControllerBase
    {
        [HttpGet("token/{username}/{meetingId}")]
        [AllowAnonymous]
        public ActionResult<VideoServiceResponse>
            GetMeetingToken(string username, string meetingId)
        {
            VideoServiceResponse response = videoService
                .GenerateMeetingToken(username, meetingId);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpGet("end-meeting/{meetingId}")]
        [Authorize]
        public async Task<ActionResult<VideoServiceResponse>>
            EndMeeting(string meetingId)
        {
            // first check if meeting room exist at all
            VideoServiceResponse getRoomNameResponse =
                await videoService.GetRoomByName(meetingId);

            if (!getRoomNameResponse.IsSuccess)
                return NotFound(getRoomNameResponse);

            // end meeting
            VideoServiceResponse endMeetingResponse =
                videoService.EndMeeting(getRoomNameResponse.Data!);

            if (!endMeetingResponse.IsSuccess)
                return BadRequest(endMeetingResponse);

            HashSet<string> userConnectionIds =
                await getUsersConnectionIdsByMeetingId.Handle(meetingId);

            if (userConnectionIds.Count == 0)
                return Ok(new VideoServiceResponse
                    { IsSuccess = true, Message = "No attendees to notify" });

            await hubContext.Clients.Clients(userConnectionIds).SendAsync("MeetingClosed");
            return Ok(new VideoServiceResponse
            {
                IsSuccess = true,
                Message = $"{userConnectionIds.Count} attendees notified"
            });
        }
    }
}
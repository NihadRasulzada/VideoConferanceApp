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
        ITwilioService twilioService,
        IHubContext<MeetingHub> hubContext,
        IGetUsersConnectionIdsByMeetingIdHelper getUsersConnectionIdsByMeetingId) : ControllerBase
    {
        [HttpGet("token/{username}/{meetingId}")]
        [AllowAnonymous]
        public ActionResult<TwilioServiceResponse>
            GetMeetingToken(string username, string meetingId)
        {
            TwilioServiceResponse response = twilioService
                .GenerateMeetingToken(username, meetingId);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpGet("end-meeting/{meetingId}")]
        [Authorize]
        public async Task<ActionResult<TwilioServiceResponse>>
            EndMeeting(string meetingId)
        {
            // first check if meeting room exist at all
            TwilioServiceResponse getRoomNameResponse =
                twilioService.GetRoomByName(meetingId);

            if (!getRoomNameResponse.IsSuccess)
                return NotFound(getRoomNameResponse);

            // end meeting
            TwilioServiceResponse endMeetingResponse =
                twilioService.EndMeeting(getRoomNameResponse.Data!);

            if (!endMeetingResponse.IsSuccess)
                return BadRequest(endMeetingResponse);

            HashSet<string> userConnectionIds =
                await getUsersConnectionIdsByMeetingId.Handle(meetingId);

            if (userConnectionIds.Count == 0)
                return NotFound(new TwilioServiceResponse
                    { IsSuccess = false, Message = "No Attendee connected" });

            await hubContext.Clients.Clients(userConnectionIds).SendAsync("MeetingClosed");
            return Ok(new TwilioServiceResponse
            {
                IsSuccess = true,
                Message = $"{userConnectionIds.Count} attendees notified"
            });
        }
    }
}
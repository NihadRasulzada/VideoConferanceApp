namespace VideoConferanceApp.Server.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using VideoConferanceApp.Shared.Meeting.Responses;

    [ApiController]
    [Route("api/[controller]")]
    public class MeetingController(ISender sender) : ControllerBase
    {
        [HttpPost("create")]
        [Authorize]
        public async Task<ActionResult<CreateMeetingResponse>> CreateMeeting(CreateMeetingRequest meeting)
        {
            CreateMeetingResponse response = await sender.Send(new CreateMeetingCommand(meeting));
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{hostId}")]
        [Authorize]
        public async Task<ActionResult<GetMeetingsResponse>> GetMeetings(string hostId)
        {
            GetMeetingsResponse response = await sender.Send(new GetMeetingsQuery(new(hostId)));
            return response.IsSuccess ? Ok(response) : NotFound(response);
        }
    }
}
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoConferanceApp.Shared.Meeting.Requests;
using VideoConferanceApp.Shared.Meeting.Responses;

namespace VideoConferanceApp.Server.Controllers
{
    // [ApiController]
    // [Route("api/[controller]")]
    // public class MeetingController(ISender sender) : ControllerBase
    // {
    //     [HttpPost("create")]
    //     [Authorize]
    //     public async Task<ActionResult<CreateMeetingResponse>> CreateMeeting(CreateMeetingRequest meeting)
    //     {
    //         CreateMeetingResponse response = await sender.Send(new CreateMeetingCommand(meeting));
    //         return response.IsSuccess ? Ok(response) : BadRequest(response);
    //     }
    //
    //     [HttpGet("{hostId}")]
    //     [Authorize]
    //     public async Task<ActionResult<GetMeetingsResponse>> GetMeetings(string hostId)
    //     {
    //         GetMeetingsResponse response = await sender.Send(new GetMeetingsQuery(new(hostId)));
    //         return response.IsSuccess ? Ok(response) : NotFound(response);
    //     }
    //
    //     [HttpGet("recent/{hostId}")]
    //     [Authorize]
    //     public async Task<ActionResult<GetRecentMeetingsResponse>> GetRecentMeetings(string hostId)
    //     {
    //         GetRecentMeetingsResponse response = await sender.Send(new GetRecentMeetingsQuery(new(hostId)));
    //         return response.IsSuccess ? Ok(response) : NotFound(response);
    //     }
    // }
}
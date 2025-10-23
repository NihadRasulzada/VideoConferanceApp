using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoConferanceApp.Shared.Meeting.Responses;


namespace VideoConferanceApp.Server.Controllers
{
    // [ApiController]
    // [Route("api/[controller]")]
    // public class TwilioController(ITwilioService twilioService) : ControllerBase
    // {
    //     [HttpGet("token/{username}/{meetingId}")]
    //     [Authorize]
    //     public async Task<ActionResult<GetMeetingsResponse>> GetMeetings(string username, string meetingId)
    //     {
    //         TwilioServiceResponse response = await twilioService.GetMeetingToken(username, meetingId);
    //         return response.IsSuccess ? Ok(response) : NotFound(response);
    //     }
    // }
}
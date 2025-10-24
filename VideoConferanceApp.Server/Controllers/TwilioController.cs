using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoConferanceApp.Server.Infrastructure.Services;
using VideoConferanceApp.Shared.Meeting.Responses;

namespace VideoConferanceApp.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TwilioController(ITwilioService twilioService) : ControllerBase
    {
        [HttpGet("token/{username}/{meetingId}")]
        [AllowAnonymous]
        public ActionResult<TwilioServiceResponse> GetMeetingToken(string username, string meetingId)
        {
            var response = twilioService.GenerateMeetingToken(username, meetingId);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
    }
}
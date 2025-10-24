using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoConferanceApp.Server.Features.Meeting.AttachDetailsToConnId.Command;
using VideoConferanceApp.Server.Features.Meeting.ClearMeeting.Command;
using VideoConferanceApp.Server.Features.Meeting.CreateMeeting.Command;
using VideoConferanceApp.Server.Features.Meeting.GetMeetingMembers.Query;
using VideoConferanceApp.Server.Features.Meeting.GetMeetings.Query;
using VideoConferanceApp.Server.Features.Meeting.GetRecentMeetings.Query;
using VideoConferanceApp.Shared.Meeting.Requests;
using VideoConferanceApp.Shared.Meeting.Responses;

namespace VideoConferanceApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetingController(ISender sender) : ControllerBase
    {
        [HttpPost("create")]
        [Authorize]
        public async Task<ActionResult<CreateMeetingResponse>>
            CreateMeeting(CreateMeetingRequest meeting)
        {
            CreateMeetingResponse Response = await sender
                .Send(new CreateMeetingCommand(meeting));
            return Response.IsSuccess ? Ok(Response) : BadRequest(Response);
        }

        [HttpGet("{hostId}")]
        [Authorize]
        public async Task<ActionResult<GetMeetingsResponse>> GetMeetings(string hostId)
        {
            GetMeetingsResponse Response = await sender.Send(new GetMeetingsQuery(hostId));
            return Response.IsSuccess ? Ok(Response) : NotFound(Response);
        }

        [HttpGet("recent/{hostId}")]
        [Authorize]
        public async Task<ActionResult<GetRecentMeetingsResponse>>
            GetRecentMeetings(string hostId)
        {
            GetRecentMeetingsResponse Response = await sender.Send(new GetRecentMeetingsQuery(hostId));
            return Response.IsSuccess ? Ok(Response) : NotFound(Response);
        }

        [HttpPost("attachDetailsToConnId")]
        [AllowAnonymous]
        public async Task<ActionResult<AttachDetailsToConnectionIdResponse>>
            AttachDetailsToConnId(AttachDetailsToConnectionIdRequest
                attachDetailsToConnectionId)
        {
            AttachDetailsToConnectionIdResponse Response =
                await sender.Send(new AttachDetailsToConnectionIdCommand(attachDetailsToConnectionId));
            return Response.IsSuccess ? Ok(Response) : NotFound(Response);
        }

        [HttpGet("clear/{meetingId}")]
        [Authorize]
        public async Task<ActionResult<ClearMeetingResponse>>
            ClearMeeting(string meetingId)
        {
            ClearMeetingResponse Response = await sender.Send(new ClearMeetingCommand(new(meetingId)));
            return Response.IsSuccess ? Ok(Response) : NotFound(Response);
        }

        [HttpGet("members/{meetingId}")]
        public async Task<ActionResult<GetMeetingMembersResponse>>
            GetMeetingMembers(string meetingId)
        {
            GetMeetingMembersResponse Response =
                await sender.Send(new GetMeetingMembersQuery(new(meetingId)));
            return Response.IsSuccess ? Ok(Response) : NotFound(Response);
        }
    }
}
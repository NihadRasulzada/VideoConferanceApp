using MediatR;
using VideoConferanceApp.Shared.Meeting.Requests;
using VideoConferanceApp.Shared.Meeting.Responses;

namespace VideoConferanceApp.Server.Features.Meeting.ClearMeeting.Command;

public record ClearMeetingCommand(ClearMeetingRequest ClearMeeting) :
    IRequest<ClearMeetingResponse>;
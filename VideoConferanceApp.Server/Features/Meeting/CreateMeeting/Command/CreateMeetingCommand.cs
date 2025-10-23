using MediatR;
using VideoConferanceApp.Shared.Meeting.Requests;
using VideoConferanceApp.Shared.Meeting.Responses;

namespace VideoConferanceApp.Server.Features.Meeting.CreateMeeting.Command;

public record CreateMeetingCommand(CreateMeetingRequest CreateMeeting) : IRequest<CreateMeetingResponse>;
using MediatR;
using VideoConferanceApp.Shared.Meeting.Responses;

namespace VideoConferanceApp.Server.Features.Meeting.GetMeetings.Query;

public record GetMeetingsQuery(string HostId) : IRequest<GetMeetingsResponse>;
using MediatR;
using VideoConferanceApp.Shared.Meeting.Responses;

namespace VideoConferanceApp.Server.Features.Meeting.GetRecentMeetings.Query;

public record GetRecentMeetingsQuery(string HostId) : IRequest<GetRecentMeetingsResponse>;
using MediatR;
using VideoConferanceApp.Shared.Meeting.Requests;
using VideoConferanceApp.Shared.Meeting.Responses;

namespace VideoConferanceApp.Server.Features.Meeting.GetMeetingMembers.Query;

public record GetMeetingMembersQuery(GetMeetingMembersRequest MeetingMembers) :
    IRequest<GetMeetingMembersResponse>;
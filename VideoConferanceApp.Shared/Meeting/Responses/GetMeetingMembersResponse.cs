namespace VideoConferanceApp.Shared.Meeting.Responses;

public record GetMeetingMembersResponse : ServiceResponse<IEnumerable<string>>;
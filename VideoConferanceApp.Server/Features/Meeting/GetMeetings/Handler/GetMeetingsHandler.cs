using MediatR;
using Microsoft.EntityFrameworkCore;
using VideoConferanceApp.Server.Features.Meeting.GetMeetings.Query;
using VideoConferanceApp.Server.Infrastructure.Data;
using VideoConferanceApp.Shared.Meeting.Responses;

namespace VideoConferanceApp.Server.Features.Meeting.GetMeetings.Handler;

public class GetMeetingsHandler(AppDbContext context) : IRequestHandler<GetMeetingsQuery, GetMeetingsResponse>
{
    public async Task<GetMeetingsResponse> Handle(GetMeetingsQuery request, CancellationToken cancellationToken)
    {
        var meetings = await context.Meetings.AsNoTracking()
            .Where(_ => _.HostId == request.HostId && _.IsComleted == false)
            .ToListAsync(cancellationToken);

        if (meetings.Count == 0)
            return new GetMeetingsResponse
            {
                IsSuccess = false,
                Message = "No meetings found"
            };

        var _meetings = meetings.Select(x => new GetMeeting
        {
            HostId = request.HostId,
            Title = x.Title,
            Description = x.Description,
            StartTimeOnly = x.StartTimeOnly,
            EndTimeOnly = x.EndTimeOnly,
            StartDateOnly = x.StartDateOnly,
            EndDateOnly = x.EndDateOnly,
            Link = x.Link,
            MeetingId = x.MeetingId,
            Passcode = x.Passcode
        }).ToList();

        return new GetMeetingsResponse
        {
            Data = _meetings,
            IsSuccess = true,
            Message = $"{_meetings.Count} meetings found"
        };
    }
}
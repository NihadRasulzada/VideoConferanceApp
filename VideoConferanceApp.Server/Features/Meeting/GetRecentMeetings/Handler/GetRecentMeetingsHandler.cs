using MediatR;
using Microsoft.EntityFrameworkCore;
using VideoConferanceApp.Server.Features.Meeting.GetRecentMeetings.Query;
using VideoConferanceApp.Server.Infrastructure.Data;
using VideoConferanceApp.Shared.Meeting.Responses;

namespace VideoConferanceApp.Server.Features.Meeting.GetRecentMeetings.Handler;

public class GetRecentMeetingsHandler(AppDbContext context)
    : IRequestHandler<GetRecentMeetingsQuery, GetRecentMeetingsResponse>
{
    public async Task<GetRecentMeetingsResponse> Handle(GetRecentMeetingsQuery request,
        CancellationToken cancellationToken)
    {
        var meetings = await context.Meetings.AsNoTracking().Where(_ => _.HostId == request.HostId && _.IsCompleted)
            .ToListAsync(cancellationToken);

        if (meetings.Count == 0)
            return new GetRecentMeetingsResponse
            {
                IsSuccess = false,
                Message = "No recent meetings found"
            };

        var recentMeetings = meetings.Select(x => new GetMeeting
        {
            Id = x.Id,
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

        return new GetRecentMeetingsResponse
        {
            Data = recentMeetings,
            IsSuccess = true,
            Message = $"{recentMeetings.Count} recent meetings found"
        };
    }
}
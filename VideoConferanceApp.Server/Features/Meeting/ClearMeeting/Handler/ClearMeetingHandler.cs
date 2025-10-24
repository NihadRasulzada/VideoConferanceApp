using MediatR;
using Microsoft.EntityFrameworkCore;
using VideoConferanceApp.Server.Features.Meeting.ClearMeeting.Command;
using VideoConferanceApp.Server.Infrastructure.Data;
using VideoConferanceApp.Shared.Meeting.Responses;

namespace VideoConferanceApp.Server.Features.Meeting.ClearMeeting.Handler;

public class ClearMeetingHandler(AppDbContext context) :
    IRequestHandler<ClearMeetingCommand, ClearMeetingResponse>
{
    public async Task<ClearMeetingResponse> Handle
        (ClearMeetingCommand request, CancellationToken cancellationToken)
    {
        // remove all attendees in Active Room
        var attendees = await context.ActiveMeetings
            .Where(_ => _.MeetingId == request.ClearMeeting.MeetingId)
            .ToListAsync(cancellationToken: cancellationToken);
        if (attendees.Count != 0)
            context.ActiveMeetings.RemoveRange(attendees);

        // remove all attendees in connected User table
        foreach (var attendee in attendees)
        {
            var user = await context.ConnectedUsers
                .FirstOrDefaultAsync(_ => _.UserId == attendee.UserId, cancellationToken: cancellationToken);
            if (user != null)
                context.ConnectedUsers.Remove(user!);
        }

        // update the meeting / set status to completed
        Models.Meeting completeMeeting = await context.Meetings
            .FirstOrDefaultAsync
                (_ => _.MeetingId == request.ClearMeeting.MeetingId, cancellationToken: cancellationToken);


        if (completeMeeting != null)
            completeMeeting.IsCompleted = true;

        await context.SaveChangesAsync(cancellationToken);

        return new() { IsSuccess = true, Message = $"{attendees.Count} users cleared" };
    }
}
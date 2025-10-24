using System.Security.Cryptography;
using MediatR;
using VideoConferanceApp.Server.Features.Meeting.CreateMeeting.Command;
using VideoConferanceApp.Server.Infrastructure.Data;
using VideoConferanceApp.Server.Infrastructure.Services;
using VideoConferanceApp.Shared.Meeting.Responses;

namespace VideoConferanceApp.Server.Features.Meeting.CreateMeeting.Handler;

public class CreateMeetingHandler(AppDbContext context, IConfiguration config, ITwilioService twilioService)
    : IRequestHandler<CreateMeetingCommand, CreateMeetingResponse>
{
    public async Task<CreateMeetingResponse> Handle(CreateMeetingCommand request, CancellationToken cancellationToken)
    {
        if (request.CreateMeeting == null)
            return new CreateMeetingResponse { IsSuccess = false, Message = "Object send is null" };

        if (DateTimeOffset.Parse(request.CreateMeeting.StartDateOnly) < DateTimeOffset.UtcNow)
            return new CreateMeetingResponse
                { IsSuccess = false, Message = "Start date cannot be lesser than today's date" };

        if (DateTimeOffset.Parse(request.CreateMeeting.EndDateOnly) <
            DateTimeOffset.Parse(request.CreateMeeting.StartDateOnly))
            return new CreateMeetingResponse
                { IsSuccess = false, Message = "End date cannot be lesser than start date" };

        var meetingId = GenerateMeetingId();

        var twilioResponse = twilioService.CreateRoom(meetingId);

        if (!twilioResponse.IsSuccess)
            return new CreateMeetingResponse { IsSuccess = twilioResponse.IsSuccess, Message = twilioResponse.Message };

        var meeting = new Models.Meeting
        {
            MeetingId = meetingId,
            HostId = request.CreateMeeting.HostId,
            Title = request.CreateMeeting.Title,
            Description = request.CreateMeeting.Description,
            StartTimeOnly = request.CreateMeeting.StartTimeOnly,
            EndTimeOnly = request.CreateMeeting.EndTimeOnly,
            StartDateOnly = request.CreateMeeting.StartDateOnly,
            EndDateOnly = request.CreateMeeting.EndDateOnly,
            Passcode = GenerateMeetingPasscode()
        };

        meeting.Link = GenerateLink(meeting.MeetingId, meeting.Passcode);
        await context.Meetings.AddAsync(meeting, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return new CreateMeetingResponse { IsSuccess = true, Message = "Meeting created" };
    }

    private static string GenerateMeetingId()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace("/", "").Replace("+", "").TrimEnd('=');
    }

    private string GenerateLink(string meetingId, string passcode)
    {
        return $"{config["Client:BaseAddress"]}/join-meeting/{meetingId}/{passcode}";
    }

    public static string GenerateMeetingPasscode()
    {
        const string chars = "ABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[4];
        rng.GetBytes(bytes);

        var randomPart = new char[4];
        for (var i = 0; i < 4; i++)
            randomPart[i] = chars[bytes[i] % chars.Length];

        var timestamp = DateTime.UtcNow.ToString("HHmmss");
        return $"{new string(randomPart)}{timestamp}";
    }
}
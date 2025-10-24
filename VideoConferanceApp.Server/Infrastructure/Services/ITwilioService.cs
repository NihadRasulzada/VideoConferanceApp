using Twilio;
using Twilio.Jwt.AccessToken;
using Twilio.Rest.Video.V1;
using VideoConferanceApp.Shared.Meeting.Responses;

namespace VideoConferanceApp.Server.Infrastructure.Services;

public interface ITwilioService
{
    TwilioServiceResponse GenerateMeetingToken(string identity, string meetingId);
    TwilioServiceResponse CreateRoom(string roomName);
}

public class TwilioService : ITwilioService
{
    private readonly string _accountSid;
    private readonly string _apiKey;
    private readonly string _apiSecret;
    private readonly string _authToken;

    public TwilioService(IConfiguration configuration)
    {
        _accountSid = configuration["Twilio:AccountSid"];
        _authToken = configuration["Twilio:AuthToken"];
        _apiKey = configuration["Twilio:ApiKey"];
        _apiSecret = configuration["Twilio:ApiSecret"];

        TwilioClient.Init(_accountSid, _authToken);
    }

    public TwilioServiceResponse GenerateMeetingToken(string identity, string meetingId)
    {
        var grants = new HashSet<IGrant> { new VideoGrant { Room = meetingId } };

        var token = new Token(_accountSid, _apiKey, _apiSecret, identity, grants: grants);

        return new TwilioServiceResponse
            { IsSuccess = true, Message = "Token senerated successfully", Data = token.ToJwt() };
    }

    public TwilioServiceResponse CreateRoom(string roomName)
    {
        var response = RoomResource.Create(type: RoomResource.RoomTypeEnum.Group, uniqueName: roomName,
            maxParticipants: 10, recordParticipantsOnConnect: false);
        if (response.Sid != null)
            return new TwilioServiceResponse { IsSuccess = true, Message = $"Room status: {response.Status}" };
        else
            return new TwilioServiceResponse { IsSuccess = false, Message = $"Room could not be created." };
    }
}
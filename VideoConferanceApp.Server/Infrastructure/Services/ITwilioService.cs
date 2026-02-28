using VideoConferanceApp.Shared.Meeting.Responses;
using Twilio;
using Twilio.Exceptions;
using Twilio.Jwt.AccessToken;
using Twilio.Rest.Video.V1;
using static Twilio.Rest.Insights.V1.Room.ParticipantResource;

namespace VideoConferanceApp.Server.Infrastructure.Services;

public interface ITwilioService
{
    TwilioServiceResponse GenerateMeetingToken(string identity, string meetingId);
    TwilioServiceResponse CreateRoom(string roomName);
    TwilioServiceResponse GetRoomByName(string roomName);
    TwilioServiceResponse EndMeeting(string roomSid);
}

public class TwilioService : ITwilioService
{
    private readonly string _accountSid;
    private readonly string _apiKey;
    private readonly string _apiSecret;
    private readonly string _authToken;

    public TwilioService(IConfiguration configuration)
    {
        _accountSid = configuration["Twilio:AccountSid"]!;
        _apiKey = configuration["Twilio:ApiKey"]!;
        _apiSecret = configuration["Twilio:ApiSecret"]!;
        _authToken = configuration["Twilio:AuthToken"]!;

        TwilioClient.Init(_accountSid, _authToken);
    }
    public TwilioServiceResponse CreateRoom(string roomName)
    {
        var response = RoomResource.Create(
                 type: RoomResource.RoomTypeEnum.Group,
                 uniqueName: roomName,
                 maxParticipants: 10,
                 recordParticipantsOnConnect: false);
        if (response.Sid != null)
            return new() { IsSuccess = true, Message = $"Room status: {response.Status}" };
        else
            return new() { IsSuccess = false, Message = $"Room could not be created" };
    }

    public TwilioServiceResponse EndMeeting(string roomSid)
    {
        try
        {
            var response = RoomResource.Update(
                 pathSid: roomSid,
                 status: RoomResource.RoomStatusEnum.Completed
             );
            if (response.Status == RoomStatusEnum.Completed)
                return new()
                { IsSuccess = true, Message = $"Room status{response.Status}" };
            else
                return new()
                { IsSuccess = false, Message = $"Room status{response.Status}" };
        }
        catch (ApiException ex)
        {
            return new() { IsSuccess = false, Message = ex.Message };
        }
    }

    public TwilioServiceResponse GenerateMeetingToken(string identity, string meetingId)
    {
        var grants = new HashSet<IGrant>
            { new VideoGrant { Room = meetingId } };

        var token =
            new Token(_accountSid, _apiKey, _apiSecret, identity, grants: grants);
        return new()
        {
            IsSuccess = true,
            Message = "Token generated successfully",
            Data = token.ToJwt()
        };
    }

    public TwilioServiceResponse GetRoomByName(string roomName)
    {
        var rooms = RoomResource.Read(uniqueName: roomName);
        var room = rooms.FirstOrDefault();
        if (room == null)
            return new()
            { IsSuccess = false, Message = "No room found", Data = roomName };
        else
            return new()
            { IsSuccess = true, Message = "Room found", Data = room.Sid };
    }
}
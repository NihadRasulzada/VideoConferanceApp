using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using VideoConferanceApp.Server.Infrastructure.Data;
using VideoConferanceApp.Shared.Meeting.Responses;

namespace VideoConferanceApp.Server.Infrastructure.Services;

public interface IVideoService
{
    TwilioServiceResponse GenerateMeetingToken(string identity, string meetingId);
    TwilioServiceResponse CreateRoom(string roomName);
    Task<TwilioServiceResponse> GetRoomByName(string roomName);
    TwilioServiceResponse EndMeeting(string roomSid);
}

public class VideoService(IConfiguration configuration, AppDbContext db) : IVideoService
{
    public TwilioServiceResponse CreateRoom(string roomName) =>
        new() { IsSuccess = true, Message = "Room ready" };

    public TwilioServiceResponse EndMeeting(string roomSid) =>
        new() { IsSuccess = true, Message = "Room ended" };

    public TwilioServiceResponse GenerateMeetingToken(string identity, string meetingId)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));

        var claims = new[]
        {
            new Claim("identity", identity),
            new Claim("room", meetingId)
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(4),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new()
        {
            IsSuccess = true,
            Message = "Token generated successfully",
            Data = new JwtSecurityTokenHandler().WriteToken(token)
        };
    }

    public async Task<TwilioServiceResponse> GetRoomByName(string roomName)
    {
        var exists = await db.Meetings.AnyAsync(m => m.MeetingId == roomName);
        return exists
            ? new() { IsSuccess = true, Message = "Room found", Data = roomName }
            : new() { IsSuccess = false, Message = "No room found" };
    }
}

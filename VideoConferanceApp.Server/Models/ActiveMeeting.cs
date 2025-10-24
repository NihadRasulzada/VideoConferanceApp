namespace VideoConferanceApp.Server.Models;

public class ActiveMeeting
{
    public int Id { get; set; }
    public string MeetingId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
}
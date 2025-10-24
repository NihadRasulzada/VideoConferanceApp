namespace VideoConferanceApp.Server.Models;

public class Meeting
{
    public int Id { get; set; }
    public string MeetingId { get; set; } = string.Empty;
    public string Passcode { get; set; } = string.Empty;
    public string HostId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string StartTimeOnly { get; set; } = string.Empty;
    public string EndTimeOnly { get; set; } = string.Empty;
    public string StartDateOnly { get; set; } = string.Empty;
    public string EndDateOnly { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public bool IsCompleted { get; set; } = false;
}
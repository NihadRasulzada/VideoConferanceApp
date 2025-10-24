namespace VideoConferanceApp.Server.Models;

public class ConnectedUser
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string ConnectionId { get; set; } = string.Empty;
}
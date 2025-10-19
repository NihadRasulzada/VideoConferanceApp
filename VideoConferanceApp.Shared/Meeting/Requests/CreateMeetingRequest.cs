namespace VideoConferanceApp.Shared.Meeting.Requests
{
    public class CreateMeetingRequest
    {
        public string HostId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string StartTimeOnly { get; set; } = string.Empty;
        public string EndTimeOnly { get; set; } = string.Empty;
        public string StartDateOnly { get; set; } = string.Empty;
        public string EndDateOnly { get; set; } = string.Empty;
    }
}

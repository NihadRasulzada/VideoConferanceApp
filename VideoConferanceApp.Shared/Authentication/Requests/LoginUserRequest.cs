using System.ComponentModel.DataAnnotations;

namespace VideoConferanceApp.Shared.Authentication.Requests
{
    public class LoginUserRequest
    {
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}

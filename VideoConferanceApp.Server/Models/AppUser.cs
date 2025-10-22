using Microsoft.AspNetCore.Identity;

namespace VideoConferanceApp.Server.Models
{
    public class AppUser : IdentityUser
    {
        public string Name { get; set; } = string.Empty;
    }
}
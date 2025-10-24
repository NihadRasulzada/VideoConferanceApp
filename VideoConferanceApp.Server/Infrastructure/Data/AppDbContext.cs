using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VideoConferanceApp.Server.Models;

namespace VideoConferanceApp.Server.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Meeting> Meetings { get; set; } = default!;
        public DbSet<ConnectedUser> ConnectedUsers { get; set; } = default!;
        public DbSet<ActiveMeeting> ActiveMeetings { get; set; } = default!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = default!;
    }
}
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VideoConferanceApp.Server.Models;

namespace VideoConferanceApp.Server.Infrastructure.Data
{
    public  class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        //public DbSet<Meeting> Meetings { get; set; } = default!;
    }
}
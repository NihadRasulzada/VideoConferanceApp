using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VideoConferanceApp.Server.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Init2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Meetings",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    MeetingId = table.Column<string>(type: "TEXT", nullable: false),
                    Passcode = table.Column<string>(type: "TEXT", nullable: false),
                    HostId = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    StartTimeOnly = table.Column<string>(type: "TEXT", nullable: false),
                    EndTimeOnly = table.Column<string>(type: "TEXT", nullable: false),
                    StartDateOnly = table.Column<string>(type: "TEXT", nullable: false),
                    EndDateOnly = table.Column<string>(type: "TEXT", nullable: false),
                    Link = table.Column<string>(type: "TEXT", nullable: false),
                    IsComleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meetings", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Meetings");
        }
    }
}

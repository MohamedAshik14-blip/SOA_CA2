using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollegeEventsApi.Migrations
{
    /// <inheritdoc />
    public partial class AddRegistrationCountToEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RegistrationCount",
                table: "Events",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegistrationCount",
                table: "Events");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollegeEventsApi.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleToStudent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Students",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Students");
        }
    }
}

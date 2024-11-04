using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagementBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddTaggedUsersJsonToTaskItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TaggedUsersJson",
                table: "TaskItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaggedUsersJson",
                table: "TaskItems");
        }
    }
}

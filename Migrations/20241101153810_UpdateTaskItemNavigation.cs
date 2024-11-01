using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagementBackend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTaskItemNavigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskSubItem");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TaskSubItem",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    TaskItemTaskId = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskSubItem", x => x.ItemId);
                    table.ForeignKey(
                        name: "FK_TaskSubItem_TaskItems_TaskItemTaskId",
                        column: x => x.TaskItemTaskId,
                        principalTable: "TaskItems",
                        principalColumn: "TaskId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskSubItem_TaskItemTaskId",
                table: "TaskSubItem",
                column: "TaskItemTaskId");
        }
    }
}

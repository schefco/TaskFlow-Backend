using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Schefco.TaskFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPendingUserFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Compamny",
                table: "Users",
                newName: "Company");

            migrationBuilder.RenameColumn(
                name: "ReasonForJoining",
                table: "PendingUsers",
                newName: "Reason");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Company",
                table: "Users",
                newName: "Compamny");

            migrationBuilder.RenameColumn(
                name: "Reason",
                table: "PendingUsers",
                newName: "ReasonForJoining");
        }
    }
}

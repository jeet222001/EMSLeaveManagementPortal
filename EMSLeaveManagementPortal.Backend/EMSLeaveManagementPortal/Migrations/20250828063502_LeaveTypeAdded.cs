using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMSLeaveManagementPortal.Migrations
{
    /// <inheritdoc />
    public partial class LeaveTypeAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Leaves",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Leaves");
        }
    }
}

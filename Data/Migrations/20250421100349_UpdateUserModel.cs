using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NextUp.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastNotifiedDiscount",
                table: "Games",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastNotifiedExpansion",
                table: "Games",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastNotifiedDiscount",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "LastNotifiedExpansion",
                table: "Games");
        }
    }
}

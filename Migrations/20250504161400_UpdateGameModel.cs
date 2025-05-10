using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NextUp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGameModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SteamDiscountInfo",
                table: "Games",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpcomingExpansionInfo",
                table: "Games",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SteamDiscountInfo",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "UpcomingExpansionInfo",
                table: "Games");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaonTrackerPro.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryIconAndIsIncome : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CategoryIcon",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsIncome",
                table: "Transactions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryIcon",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "IsIncome",
                table: "Transactions");
        }
    }
}

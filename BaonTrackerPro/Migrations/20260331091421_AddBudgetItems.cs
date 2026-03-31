using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaonTrackerPro.Migrations
{
    /// <inheritdoc />
    public partial class AddBudgetItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MonthlyAmount",
                table: "BudgetItems",
                newName: "AmountLimit");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "BudgetItems",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Period",
                table: "BudgetItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "BudgetItems");

            migrationBuilder.DropColumn(
                name: "Period",
                table: "BudgetItems");

            migrationBuilder.RenameColumn(
                name: "AmountLimit",
                table: "BudgetItems",
                newName: "MonthlyAmount");
        }
    }
}

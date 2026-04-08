using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaonTrackerPro.Migrations
{
    /// <inheritdoc />
    public partial class AddUserOwnership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppUserId",
                table: "Transactions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AppUserId",
                table: "SavingsGoals",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AppUserId",
                table: "BudgetItems",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AppUserId",
                table: "Transactions",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SavingsGoals_AppUserId",
                table: "SavingsGoals",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetItems_AppUserId",
                table: "BudgetItems",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetItems_AppUsers_AppUserId",
                table: "BudgetItems",
                column: "AppUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SavingsGoals_AppUsers_AppUserId",
                table: "SavingsGoals",
                column: "AppUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_AppUsers_AppUserId",
                table: "Transactions",
                column: "AppUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetItems_AppUsers_AppUserId",
                table: "BudgetItems");

            migrationBuilder.DropForeignKey(
                name: "FK_SavingsGoals_AppUsers_AppUserId",
                table: "SavingsGoals");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_AppUsers_AppUserId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_AppUserId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_SavingsGoals_AppUserId",
                table: "SavingsGoals");

            migrationBuilder.DropIndex(
                name: "IX_BudgetItems_AppUserId",
                table: "BudgetItems");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "SavingsGoals");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "BudgetItems");
        }
    }
}

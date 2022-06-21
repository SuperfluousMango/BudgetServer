using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetServer.Migrations
{
    public partial class AddSoftDeleteAndUniqueIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Expense",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseCategoryGroup_Name",
                table: "ExpenseCategoryGroup",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseCategory_Name_ExpenseCategoryGroupId",
                table: "ExpenseCategory",
                columns: new[] { "Name", "ExpenseCategoryGroupId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ExpenseCategoryGroup_Name",
                table: "ExpenseCategoryGroup");

            migrationBuilder.DropIndex(
                name: "IX_ExpenseCategory_Name_ExpenseCategoryGroupId",
                table: "ExpenseCategory");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Expense");
        }
    }
}

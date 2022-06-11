using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetServer.Migrations
{
    public partial class RenameTransactionToExpense : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Geez, it sucks to try to rename tables instead of dropping/recreating them.
            // This is what happens when you forget what you'd planned to call the tables in the first place.

            // Drop FKs to disconnect tables from each other
            migrationBuilder.DropForeignKey("FK_TransactionEntry_TransactionCategory_TransactionCategoryId", "TransactionEntry");
            migrationBuilder.DropForeignKey("FK_TransactionCategory_TransactionCategoryGroup_TransactionCategoryGroupId", "TransactionCategory");

            // Drop PKs before renaming tables, then recreate them afterward.
            migrationBuilder.DropPrimaryKey("PK_TransactionEntry", table: "TransactionEntry");
            migrationBuilder.RenameTable("TransactionEntry", newName: "Expense");
            migrationBuilder.RenameColumn("TransactionCategoryId", table: "Expense", newName: "ExpenseCategoryId");
            migrationBuilder.AddPrimaryKey("PK_Expense", table: "Expense", column: "Id");

            migrationBuilder.DropPrimaryKey("PK_TransactionCategory", table: "TransactionCategory");
            migrationBuilder.RenameTable("TransactionCategory", newName: "ExpenseCategory");
            migrationBuilder.RenameColumn("TransactionCategoryGroupId", table: "ExpenseCategory", newName: "ExpenseCategoryGroupId");
            migrationBuilder.AddPrimaryKey("PK_ExpenseCategory", table: "ExpenseCategory", column: "Id");

            migrationBuilder.DropPrimaryKey("PK_TransactionCategoryGroup", table: "TransactionCategoryGroup");
            migrationBuilder.RenameTable("TransactionCategoryGroup", newName: "ExpenseCategoryGroup");
            migrationBuilder.AddPrimaryKey("PK_ExpenseCategoryGroup", table: "ExpenseCategoryGroup", column: "Id");

            // Recreate foreign keys
            migrationBuilder.AddForeignKey(
                name: "FK_Expense_ExpenseCategory_ExpenseCategoryId",
                table: "Expense",
                column: "ExpenseCategoryId",
                principalTable: "ExpenseCategory",
                principalColumn: "Id"
            );
            migrationBuilder.AddForeignKey(
                name: "FK_ExpenseCategory_ExpenseCategoryGroup_ExpenseCategoryGroupId",
                table: "ExpenseCategory",
                column: "ExpenseCategoryGroupId",
                principalTable: "ExpenseCategoryGroup",
                principalColumn: "Id"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop FKs to disconnect tables from each other
            migrationBuilder.DropForeignKey("FK_Expense_ExpenseCategory_ExpenseCategoryId", "Expense");
            migrationBuilder.DropForeignKey("FK_ExpenseCategory_ExpenseCategoryGroup_ExpenseCategoryGroupId", "ExpenseCategory");

            // Drop PKs before renaming tables, then recreate them afterward.
            migrationBuilder.DropPrimaryKey("PK_Expense", table: "Expense");
            migrationBuilder.RenameTable("Expense", newName: "TransactionEntry");
            migrationBuilder.RenameColumn("ExpenseCategoryId", table: "TransactionEntry", newName: "TransactionCategoryId");
            migrationBuilder.AddPrimaryKey("PK_TransactionEntry", table: "TransactionEntry", column: "Id");

            migrationBuilder.DropPrimaryKey("PK_ExpenseCategory", table: "ExpenseCategory");
            migrationBuilder.RenameTable("ExpenseCategory", newName: "TransactionCategory");
            migrationBuilder.RenameColumn("ExpenseCategoryGroupId", table: "TransactionCategory", newName: "TransactionCategoryGroupId");
            migrationBuilder.AddPrimaryKey("PK_TransactionCategory", table: "TransactionCategory", column: "Id");

            migrationBuilder.DropPrimaryKey("PK_ExpenseCategoryGroup", table: "ExpenseCategoryGroup");
            migrationBuilder.RenameTable("ExpenseCategoryGroup", newName: "TransactionCategoryGroup");
            migrationBuilder.AddPrimaryKey("PK_TransactionCategoryGroup", table: "TransactionCategoryGroup", column: "Id");

            // Recreate foreign keys
            migrationBuilder.AddForeignKey(
                name: "FK_TransactionEntry_TransactionCategory_TransactionCategoryId",
                table: "TransactionEntry",
                column: "TransactionCategoryId",
                principalTable: "TransactionCategory",
                principalColumn: "Id"
            );
            migrationBuilder.AddForeignKey(
                name: "FK_TransactionCategory_TransactionCategoryGroup_TransactionCategoryGroupId",
                table: "TransactionCategory",
                column: "TransactionCategoryGroupId",
                principalTable: "TransactionCategoryGroup",
                principalColumn: "Id"
            );
        }
    }
}

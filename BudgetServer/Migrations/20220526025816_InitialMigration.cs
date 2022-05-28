using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetServer.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TransactionCategoryGroup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionCategoryGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransactionCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TransactionCategoryGroupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransactionCategory_TransactionCategoryGroup_TransactionCategoryGroupId",
                        column: x => x.TransactionCategoryGroupId,
                        principalTable: "TransactionCategoryGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransactionEntry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<decimal>(type: "decimal(7,2)", nullable: false),
                    Memo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TransactionDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    TransactionCategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransactionEntry_TransactionCategory_TransactionCategoryId",
                        column: x => x.TransactionCategoryId,
                        principalTable: "TransactionCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "TransactionCategoryGroup",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "Home" });

            migrationBuilder.InsertData(
                table: "TransactionCategory",
                columns: new[] { "Id", "Name", "TransactionCategoryGroupId" },
                values: new object[,]
                {
                    { 1, "Mortgage", 1 },
                    { 2, "Water bill", 1 },
                    { 3, "Electric bill", 1 },
                    { 4, "Gas bill", 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransactionCategory_TransactionCategoryGroupId",
                table: "TransactionCategory",
                column: "TransactionCategoryGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionEntry_TransactionCategoryId",
                table: "TransactionEntry",
                column: "TransactionCategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransactionEntry");

            migrationBuilder.DropTable(
                name: "TransactionCategory");

            migrationBuilder.DropTable(
                name: "TransactionCategoryGroup");
        }
    }
}

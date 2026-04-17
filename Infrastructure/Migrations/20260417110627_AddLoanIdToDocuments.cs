using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLoanIdToDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LoanId",
                table: "Documents",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_LoanId",
                table: "Documents",
                column: "LoanId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Loans_LoanId",
                table: "Documents",
                column: "LoanId",
                principalTable: "Loans",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Loans_LoanId",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_LoanId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "LoanId",
                table: "Documents");
        }
    }
}

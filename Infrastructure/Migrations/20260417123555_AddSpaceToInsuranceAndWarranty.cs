using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSpaceToInsuranceAndWarranty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SpaceId",
                table: "Warranties",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SpaceId",
                table: "Insurances",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Warranties_SpaceId",
                table: "Warranties",
                column: "SpaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Insurances_SpaceId",
                table: "Insurances",
                column: "SpaceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Insurances_Spaces_SpaceId",
                table: "Insurances",
                column: "SpaceId",
                principalTable: "Spaces",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Warranties_Spaces_SpaceId",
                table: "Warranties",
                column: "SpaceId",
                principalTable: "Spaces",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Insurances_Spaces_SpaceId",
                table: "Insurances");

            migrationBuilder.DropForeignKey(
                name: "FK_Warranties_Spaces_SpaceId",
                table: "Warranties");

            migrationBuilder.DropIndex(
                name: "IX_Warranties_SpaceId",
                table: "Warranties");

            migrationBuilder.DropIndex(
                name: "IX_Insurances_SpaceId",
                table: "Insurances");

            migrationBuilder.DropColumn(
                name: "SpaceId",
                table: "Warranties");

            migrationBuilder.DropColumn(
                name: "SpaceId",
                table: "Insurances");
        }
    }
}

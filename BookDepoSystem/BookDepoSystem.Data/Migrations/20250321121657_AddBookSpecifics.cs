using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookDepoSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBookSpecifics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AgeRange",
                table: "Books",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CoverType",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Isbn",
                table: "Books",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Pages",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Sku",
                table: "Books",
                type: "nvarchar(12)",
                maxLength: 12,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgeRange",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "CoverType",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "Isbn",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "Pages",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "Sku",
                table: "Books");
        }
    }
}

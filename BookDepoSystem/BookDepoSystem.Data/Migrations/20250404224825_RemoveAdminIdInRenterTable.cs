using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookDepoSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAdminIdInRenterTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Renters_AspNetUsers_AdminId",
                table: "Renters");

            migrationBuilder.DropIndex(
                name: "IX_Renters_AdminId",
                table: "Renters");

            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "Renters");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AdminId",
                table: "Renters",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Renters_AdminId",
                table: "Renters",
                column: "AdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_Renters_AspNetUsers_AdminId",
                table: "Renters",
                column: "AdminId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}

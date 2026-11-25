using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrightMinds.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSettlement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RequiresSettlement",
                table: "Recipes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "InitialTotalWeight",
                table: "Products",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SettledByUserId",
                table: "Products",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SettledWeight",
                table: "Products",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SettlementDate",
                table: "Products",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SettlementLoss",
                table: "Products",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WithdrawHistoryId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_SettledByUserId",
                table: "Products",
                column: "SettledByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_WithdrawHistoryId",
                table: "Products",
                column: "WithdrawHistoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_AspNetUsers_SettledByUserId",
                table: "Products",
                column: "SettledByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_WithdrawHistory_WithdrawHistoryId",
                table: "Products",
                column: "WithdrawHistoryId",
                principalTable: "WithdrawHistory",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_AspNetUsers_SettledByUserId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_WithdrawHistory_WithdrawHistoryId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_SettledByUserId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_WithdrawHistoryId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "RequiresSettlement",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "InitialTotalWeight",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SettledByUserId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SettledWeight",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SettlementDate",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SettlementLoss",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "WithdrawHistoryId",
                table: "Products");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrightMinds.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddingSupplyReturnTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "RefundAmount",
                table: "Supplies",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "SupplyReturns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ReturnerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SupplyId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplyReturns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplyReturns_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SupplyReturns_Supplies_SupplyId",
                        column: x => x.SupplyId,
                        principalTable: "Supplies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SupplyReturnItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplyReturnId = table.Column<int>(type: "int", nullable: false),
                    SupplyItemId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitType = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplyReturnItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplyReturnItem_SupplyItems_SupplyReturnId",
                        column: x => x.SupplyReturnId,
                        principalTable: "SupplyItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SupplyReturnItem_SupplyReturns_SupplyReturnId",
                        column: x => x.SupplyReturnId,
                        principalTable: "SupplyReturns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SupplyReturnItem_SupplyReturnId",
                table: "SupplyReturnItem",
                column: "SupplyReturnId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyReturns_SupplyId",
                table: "SupplyReturns",
                column: "SupplyId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyReturns_UserId",
                table: "SupplyReturns",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupplyReturnItem");

            migrationBuilder.DropTable(
                name: "SupplyReturns");

            migrationBuilder.DropColumn(
                name: "RefundAmount",
                table: "Supplies");
        }
    }
}

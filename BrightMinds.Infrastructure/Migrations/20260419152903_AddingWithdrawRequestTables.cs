using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrightMinds.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddingWithdrawRequestTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SalesWarehouseWithdrawRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SenderId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ReceiverId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesWarehouseWithdrawRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesWarehouseWithdrawRequests_AspNetUsers_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SalesWarehouseWithdrawRequests_AspNetUsers_SenderId",
                        column: x => x.SenderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RequestedSalesWarehouseInventoryItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductPortionId = table.Column<int>(type: "int", nullable: false),
                    InventoryItemId = table.Column<int>(type: "int", nullable: true),
                    WithdrawRequestId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestedSalesWarehouseInventoryItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestedSalesWarehouseInventoryItems_ProductPortions_ProductPortionId",
                        column: x => x.ProductPortionId,
                        principalTable: "ProductPortions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequestedSalesWarehouseInventoryItems_SalesWarehouseWithdrawRequests_WithdrawRequestId",
                        column: x => x.WithdrawRequestId,
                        principalTable: "SalesWarehouseWithdrawRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequestedSalesWarehouseInventoryItems_WarehouseProductsInventorys_InventoryItemId",
                        column: x => x.InventoryItemId,
                        principalTable: "WarehouseProductsInventorys",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequestedSalesWarehouseInventoryItems_InventoryItemId",
                table: "RequestedSalesWarehouseInventoryItems",
                column: "InventoryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestedSalesWarehouseInventoryItems_ProductPortionId",
                table: "RequestedSalesWarehouseInventoryItems",
                column: "ProductPortionId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestedSalesWarehouseInventoryItems_WithdrawRequestId",
                table: "RequestedSalesWarehouseInventoryItems",
                column: "WithdrawRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesWarehouseWithdrawRequests_ReceiverId",
                table: "SalesWarehouseWithdrawRequests",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesWarehouseWithdrawRequests_SenderId",
                table: "SalesWarehouseWithdrawRequests",
                column: "SenderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestedSalesWarehouseInventoryItems");

            migrationBuilder.DropTable(
                name: "SalesWarehouseWithdrawRequests");
        }
    }
}

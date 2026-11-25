using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrightMinds.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddingRequiredShiftColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChefShiftId",
                table: "WithdrawHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ShiftId",
                table: "SalesWarehouseWithdrawRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_WithdrawHistory_ChefShiftId",
                table: "WithdrawHistory",
                column: "ChefShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesWarehouseWithdrawRequests_ShiftId",
                table: "SalesWarehouseWithdrawRequests",
                column: "ShiftId");

            migrationBuilder.AddForeignKey(
                name: "FK_SalesWarehouseWithdrawRequests_Shifts_ShiftId",
                table: "SalesWarehouseWithdrawRequests",
                column: "ShiftId",
                principalTable: "Shifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WithdrawHistory_Shifts_ChefShiftId",
                table: "WithdrawHistory",
                column: "ChefShiftId",
                principalTable: "Shifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalesWarehouseWithdrawRequests_Shifts_ShiftId",
                table: "SalesWarehouseWithdrawRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_WithdrawHistory_Shifts_ChefShiftId",
                table: "WithdrawHistory");

            migrationBuilder.DropIndex(
                name: "IX_WithdrawHistory_ChefShiftId",
                table: "WithdrawHistory");

            migrationBuilder.DropIndex(
                name: "IX_SalesWarehouseWithdrawRequests_ShiftId",
                table: "SalesWarehouseWithdrawRequests");

            migrationBuilder.DropColumn(
                name: "ChefShiftId",
                table: "WithdrawHistory");

            migrationBuilder.DropColumn(
                name: "ShiftId",
                table: "SalesWarehouseWithdrawRequests");
        }
    }
}

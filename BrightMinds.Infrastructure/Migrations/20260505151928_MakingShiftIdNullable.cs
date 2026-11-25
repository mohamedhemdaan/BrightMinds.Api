using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrightMinds.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakingShiftIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WithdrawHistory_Shifts_ChefShiftId",
                table: "WithdrawHistory");

            migrationBuilder.AlterColumn<int>(
                name: "ChefShiftId",
                table: "WithdrawHistory",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_WithdrawHistory_Shifts_ChefShiftId",
                table: "WithdrawHistory",
                column: "ChefShiftId",
                principalTable: "Shifts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WithdrawHistory_Shifts_ChefShiftId",
                table: "WithdrawHistory");

            migrationBuilder.AlterColumn<int>(
                name: "ChefShiftId",
                table: "WithdrawHistory",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WithdrawHistory_Shifts_ChefShiftId",
                table: "WithdrawHistory",
                column: "ChefShiftId",
                principalTable: "Shifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

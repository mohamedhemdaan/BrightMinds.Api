using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrightMinds.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddConsumedForShifts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ConsumedQuantity",
                table: "WithdrawItems",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SourceWithdrawItemId",
                table: "WithdrawItems",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConsumedQuantity",
                table: "WithdrawItems");

            migrationBuilder.DropColumn(
                name: "SourceWithdrawItemId",
                table: "WithdrawItems");
        }
    }
}

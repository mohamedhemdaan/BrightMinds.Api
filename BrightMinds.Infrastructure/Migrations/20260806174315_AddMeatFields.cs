using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrightMinds.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMeatFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ExchangePrice",
                table: "SupplyItems",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceMultiplier",
                table: "SupplyItems",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ReceivedWeightKg",
                table: "SupplyItems",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsMultiplierLocked",
                table: "RawMaterials",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExchangePrice",
                table: "SupplyItems");

            migrationBuilder.DropColumn(
                name: "PriceMultiplier",
                table: "SupplyItems");

            migrationBuilder.DropColumn(
                name: "ReceivedWeightKg",
                table: "SupplyItems");

            migrationBuilder.DropColumn(
                name: "IsMultiplierLocked",
                table: "RawMaterials");
        }
    }
}

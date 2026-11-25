using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrightMinds.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangesOfMaterials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ContainerQuantity",
                table: "WithdrawItems",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisplayDetails",
                table: "WithdrawItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "ContainerQuantity",
                table: "WarehouseSuppliesInventory",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisplayDetails",
                table: "WarehouseSuppliesInventory",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ContainerQuantity",
                table: "SupplyItems",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisplayDetails",
                table: "SupplyItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ContainerSize",
                table: "RawMaterials",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContainerUnitName",
                table: "RawMaterials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitWeightInKg",
                table: "RawMaterials",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContainerQuantity",
                table: "WithdrawItems");

            migrationBuilder.DropColumn(
                name: "DisplayDetails",
                table: "WithdrawItems");

            migrationBuilder.DropColumn(
                name: "ContainerQuantity",
                table: "WarehouseSuppliesInventory");

            migrationBuilder.DropColumn(
                name: "DisplayDetails",
                table: "WarehouseSuppliesInventory");

            migrationBuilder.DropColumn(
                name: "ContainerQuantity",
                table: "SupplyItems");

            migrationBuilder.DropColumn(
                name: "DisplayDetails",
                table: "SupplyItems");

            migrationBuilder.DropColumn(
                name: "ContainerSize",
                table: "RawMaterials");

            migrationBuilder.DropColumn(
                name: "ContainerUnitName",
                table: "RawMaterials");

            migrationBuilder.DropColumn(
                name: "UnitWeightInKg",
                table: "RawMaterials");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrightMinds.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInnerUnitLevelToRawMaterial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InnerUnitName",
                table: "RawMaterials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InnerUnitWeightInKg",
                table: "RawMaterials",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InnerUnitsPerSubUnit",
                table: "RawMaterials",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 1m);

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerInnerUnit",
                table: "RawMaterials",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InnerUnitName",
                table: "RawMaterials");

            migrationBuilder.DropColumn(
                name: "InnerUnitWeightInKg",
                table: "RawMaterials");

            migrationBuilder.DropColumn(
                name: "InnerUnitsPerSubUnit",
                table: "RawMaterials");

            migrationBuilder.DropColumn(
                name: "PricePerInnerUnit",
                table: "RawMaterials");
        }
    }
}

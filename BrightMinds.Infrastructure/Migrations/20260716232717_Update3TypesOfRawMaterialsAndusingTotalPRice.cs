using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrightMinds.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Update3TypesOfRawMaterialsAndusingTotalPRice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PricePerUnit",
                table: "RawMaterials",
                newName: "TotalContainerPrice");

            migrationBuilder.AlterColumn<decimal>(
                name: "ContainerSize",
                table: "RawMaterials",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HandlingType",
                table: "RawMaterials",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SubUnitName",
                table: "RawMaterials",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HandlingType",
                table: "RawMaterials");

            migrationBuilder.DropColumn(
                name: "SubUnitName",
                table: "RawMaterials");

            migrationBuilder.RenameColumn(
                name: "TotalContainerPrice",
                table: "RawMaterials",
                newName: "PricePerUnit");

            migrationBuilder.AlterColumn<decimal>(
                name: "ContainerSize",
                table: "RawMaterials",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}

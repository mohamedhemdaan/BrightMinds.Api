using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrightMinds.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EditRestuarantSupply : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "RestaurantSupplyWithdraws",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "IsWithdrawnByWeight",
                table: "RestaurantSupplyWithdraws",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "WithdrawnWeight",
                table: "RestaurantSupplyWithdraws",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsWithdrawnByWeight",
                table: "RestaurantSupplyWithdraws");

            migrationBuilder.DropColumn(
                name: "WithdrawnWeight",
                table: "RestaurantSupplyWithdraws");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "RestaurantSupplyWithdraws",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}

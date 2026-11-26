using BrightMinds.Core.Models;
using Blendd.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrightMinds.Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20261126143000_AddExplicitMaterialUnitPricingAndRecipeUnitLevel")]
    public partial class AddExplicitMaterialUnitPricingAndRecipeUnitLevel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(name: "MiddleUnitName", table: "RawMaterials", type: "nvarchar(max)", nullable: true);
            migrationBuilder.AddColumn<string>(name: "SmallestUnitName", table: "RawMaterials", type: "nvarchar(max)", nullable: true);
            migrationBuilder.AddColumn<decimal>(name: "SmallestUnitsPerMiddleUnit", table: "RawMaterials", type: "decimal(18,4)", nullable: false, defaultValue: 1m);
            migrationBuilder.AddColumn<decimal>(name: "PricePerMiddleUnit", table: "RawMaterials", type: "decimal(18,4)", nullable: true);
            migrationBuilder.AddColumn<decimal>(name: "PricePerSmallestUnit", table: "RawMaterials", type: "decimal(18,4)", nullable: true);
            migrationBuilder.AddColumn<int>(name: "RawMaterialUnitLevel", table: "RecipeIngrediants", type: "int", nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "MiddleUnitName", table: "RawMaterials");
            migrationBuilder.DropColumn(name: "SmallestUnitName", table: "RawMaterials");
            migrationBuilder.DropColumn(name: "SmallestUnitsPerMiddleUnit", table: "RawMaterials");
            migrationBuilder.DropColumn(name: "PricePerMiddleUnit", table: "RawMaterials");
            migrationBuilder.DropColumn(name: "PricePerSmallestUnit", table: "RawMaterials");
            migrationBuilder.DropColumn(name: "RawMaterialUnitLevel", table: "RecipeIngrediants");
        }
    }
}

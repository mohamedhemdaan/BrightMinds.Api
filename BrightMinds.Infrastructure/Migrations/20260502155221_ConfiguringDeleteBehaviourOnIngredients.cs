using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrightMinds.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConfiguringDeleteBehaviourOnIngredients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PortionIngredients_RecipePortions_ProductPortionId",
                table: "PortionIngredients");

            migrationBuilder.DropForeignKey(
                name: "FK_RawMaterialIngredients_RawMaterials_RawMaterialId",
                table: "RawMaterialIngredients");

            migrationBuilder.AddForeignKey(
                name: "FK_PortionIngredients_RecipePortions_ProductPortionId",
                table: "PortionIngredients",
                column: "ProductPortionId",
                principalTable: "RecipePortions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RawMaterialIngredients_RawMaterials_RawMaterialId",
                table: "RawMaterialIngredients",
                column: "RawMaterialId",
                principalTable: "RawMaterials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PortionIngredients_RecipePortions_ProductPortionId",
                table: "PortionIngredients");

            migrationBuilder.DropForeignKey(
                name: "FK_RawMaterialIngredients_RawMaterials_RawMaterialId",
                table: "RawMaterialIngredients");

            migrationBuilder.AddForeignKey(
                name: "FK_PortionIngredients_RecipePortions_ProductPortionId",
                table: "PortionIngredients",
                column: "ProductPortionId",
                principalTable: "RecipePortions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RawMaterialIngredients_RawMaterials_RawMaterialId",
                table: "RawMaterialIngredients",
                column: "RawMaterialId",
                principalTable: "RawMaterials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

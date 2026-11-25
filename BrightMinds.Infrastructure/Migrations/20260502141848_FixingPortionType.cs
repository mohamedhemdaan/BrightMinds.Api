using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrightMinds.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixingPortionType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PortionIngredients_ProductPortions_ProductPortionId",
                table: "PortionIngredients");

            migrationBuilder.AddForeignKey(
                name: "FK_PortionIngredients_RecipePortions_ProductPortionId",
                table: "PortionIngredients",
                column: "ProductPortionId",
                principalTable: "RecipePortions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PortionIngredients_RecipePortions_ProductPortionId",
                table: "PortionIngredients");

            migrationBuilder.AddForeignKey(
                name: "FK_PortionIngredients_ProductPortions_ProductPortionId",
                table: "PortionIngredients",
                column: "ProductPortionId",
                principalTable: "ProductPortions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

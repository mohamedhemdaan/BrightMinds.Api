using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrightMinds.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPortionCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PortionCategoryId",
                table: "Recipes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "RecipePortions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "ProductPortions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PortionCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortionCategories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_PortionCategoryId",
                table: "Recipes",
                column: "PortionCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipePortions_CategoryId",
                table: "RecipePortions",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPortions_CategoryId",
                table: "ProductPortions",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductPortions_PortionCategories_CategoryId",
                table: "ProductPortions",
                column: "CategoryId",
                principalTable: "PortionCategories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RecipePortions_PortionCategories_CategoryId",
                table: "RecipePortions",
                column: "CategoryId",
                principalTable: "PortionCategories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_PortionCategories_PortionCategoryId",
                table: "Recipes",
                column: "PortionCategoryId",
                principalTable: "PortionCategories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductPortions_PortionCategories_CategoryId",
                table: "ProductPortions");

            migrationBuilder.DropForeignKey(
                name: "FK_RecipePortions_PortionCategories_CategoryId",
                table: "RecipePortions");

            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_PortionCategories_PortionCategoryId",
                table: "Recipes");

            migrationBuilder.DropTable(
                name: "PortionCategories");

            migrationBuilder.DropIndex(
                name: "IX_Recipes_PortionCategoryId",
                table: "Recipes");

            migrationBuilder.DropIndex(
                name: "IX_RecipePortions_CategoryId",
                table: "RecipePortions");

            migrationBuilder.DropIndex(
                name: "IX_ProductPortions_CategoryId",
                table: "ProductPortions");

            migrationBuilder.DropColumn(
                name: "PortionCategoryId",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "RecipePortions");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "ProductPortions");
        }
    }
}

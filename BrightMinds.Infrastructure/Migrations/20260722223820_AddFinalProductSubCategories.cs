using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrightMinds.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFinalProductSubCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubCategoryId",
                table: "FinalProducts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FinalProductSubCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinalProductSubCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinalProductSubCategories_FinalProductCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "FinalProductCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FinalProducts_SubCategoryId",
                table: "FinalProducts",
                column: "SubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_FinalProductSubCategories_CategoryId",
                table: "FinalProductSubCategories",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_FinalProducts_FinalProductSubCategories_SubCategoryId",
                table: "FinalProducts",
                column: "SubCategoryId",
                principalTable: "FinalProductSubCategories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FinalProducts_FinalProductSubCategories_SubCategoryId",
                table: "FinalProducts");

            migrationBuilder.DropTable(
                name: "FinalProductSubCategories");

            migrationBuilder.DropIndex(
                name: "IX_FinalProducts_SubCategoryId",
                table: "FinalProducts");

            migrationBuilder.DropColumn(
                name: "SubCategoryId",
                table: "FinalProducts");
        }
    }
}

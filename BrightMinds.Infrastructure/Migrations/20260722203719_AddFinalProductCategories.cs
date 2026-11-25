using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrightMinds.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFinalProductCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "FinalProducts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FinalProductCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinalProductCategories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FinalProducts_CategoryId",
                table: "FinalProducts",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_FinalProducts_FinalProductCategories_CategoryId",
                table: "FinalProducts",
                column: "CategoryId",
                principalTable: "FinalProductCategories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FinalProducts_FinalProductCategories_CategoryId",
                table: "FinalProducts");

            migrationBuilder.DropTable(
                name: "FinalProductCategories");

            migrationBuilder.DropIndex(
                name: "IX_FinalProducts_CategoryId",
                table: "FinalProducts");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "FinalProducts");
        }
    }
}

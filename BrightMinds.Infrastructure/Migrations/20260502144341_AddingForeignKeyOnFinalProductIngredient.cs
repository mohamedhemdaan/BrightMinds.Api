using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrightMinds.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddingForeignKeyOnFinalProductIngredient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FinalProductIngredients_FinalProducts_FinalProductId",
                table: "FinalProductIngredients");

            migrationBuilder.AlterColumn<int>(
                name: "FinalProductId",
                table: "FinalProductIngredients",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FinalProductIngredients_FinalProducts_FinalProductId",
                table: "FinalProductIngredients",
                column: "FinalProductId",
                principalTable: "FinalProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FinalProductIngredients_FinalProducts_FinalProductId",
                table: "FinalProductIngredients");

            migrationBuilder.AlterColumn<int>(
                name: "FinalProductId",
                table: "FinalProductIngredients",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_FinalProductIngredients_FinalProducts_FinalProductId",
                table: "FinalProductIngredients",
                column: "FinalProductId",
                principalTable: "FinalProducts",
                principalColumn: "Id");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrightMinds.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddingFinalProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FinalProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinalProducts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FinalProductIngredients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitType = table.Column<int>(type: "int", nullable: false),
                    FinalProductId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinalProductIngredients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinalProductIngredients_FinalProducts_FinalProductId",
                        column: x => x.FinalProductId,
                        principalTable: "FinalProducts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PortionIngredients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    ProductPortionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortionIngredients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PortionIngredients_FinalProductIngredients_Id",
                        column: x => x.Id,
                        principalTable: "FinalProductIngredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PortionIngredients_ProductPortions_ProductPortionId",
                        column: x => x.ProductPortionId,
                        principalTable: "ProductPortions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RawMaterialIngredients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    RawMaterialId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RawMaterialIngredients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RawMaterialIngredients_FinalProductIngredients_Id",
                        column: x => x.Id,
                        principalTable: "FinalProductIngredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RawMaterialIngredients_RawMaterials_RawMaterialId",
                        column: x => x.RawMaterialId,
                        principalTable: "RawMaterials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FinalProductIngredients_FinalProductId",
                table: "FinalProductIngredients",
                column: "FinalProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PortionIngredients_ProductPortionId",
                table: "PortionIngredients",
                column: "ProductPortionId");

            migrationBuilder.CreateIndex(
                name: "IX_RawMaterialIngredients_RawMaterialId",
                table: "RawMaterialIngredients",
                column: "RawMaterialId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PortionIngredients");

            migrationBuilder.DropTable(
                name: "RawMaterialIngredients");

            migrationBuilder.DropTable(
                name: "FinalProductIngredients");

            migrationBuilder.DropTable(
                name: "FinalProducts");
        }
    }
}

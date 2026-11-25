using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrightMinds.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPricePerKgAndPricePerPiece : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. إضافة الأعمدة الجديدة
            migrationBuilder.AddColumn<decimal>(
                name: "PricePerKg",
                table: "RawMaterials",
                type: "decimal(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerPiece",
                table: "RawMaterials",
                type: "decimal(18,4)",
                nullable: true);

            // ============================================================
            //  ✅ 2. تحديث البيانات القديمة
            // ============================================================

            // 2.1 تحديث PricePerKg للمنتجات اللي ليها وزن
            migrationBuilder.Sql(@"
                UPDATE RawMaterials
                SET PricePerKg = 
                    CASE 
                        WHEN UnitWeightInKg IS NOT NULL AND UnitWeightInKg > 0 AND ContainerSize > 0
                        THEN TotalContainerPrice / (ContainerSize * UnitWeightInKg)
                        ELSE NULL
                    END
                WHERE UnitWeightInKg IS NOT NULL AND UnitWeightInKg > 0 AND ContainerSize > 0;
            ");

            // 2.2 تحديث PricePerPiece لكل المنتجات اللي ليها ContainerSize
            migrationBuilder.Sql(@"
                UPDATE RawMaterials
                SET PricePerPiece = 
                    CASE 
                        WHEN ContainerSize > 0
                        THEN TotalContainerPrice / ContainerSize
                        ELSE NULL
                    END
                WHERE ContainerSize > 0;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PricePerKg",
                table: "RawMaterials");

            migrationBuilder.DropColumn(
                name: "PricePerPiece",
                table: "RawMaterials");
        }
    }
}
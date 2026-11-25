#pragma warning disable  CS0618

using BrightMinds.Core.Enums;
using BrightMinds.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BrightMinds.Infrastructure.DataAccess.Config
{
    public class RecipeIngredientConfig : IEntityTypeConfiguration<RecipeIngrediant>
    {
        public void Configure(EntityTypeBuilder<RecipeIngrediant> builder)
        {
            builder.HasOne(ri => ri.Recipe)
                   .WithMany(r => r.Ingrediants)
                   .HasForeignKey(r => r.RecipeId);

            builder.HasOne(ri => ri.SubRecipe)
                   .WithMany()
                   .HasForeignKey(ri => ri.SubRecipeId);

            builder.Property(r => r.IngredientType)
                   .HasConversion<string>();

            builder.Property(r => r.IngredientType)
                   .HasDefaultValue(IngredientType.RawMaterial);

            // Check constraint to ensure the ingredient is either raw Material or sub recipe
            builder.HasCheckConstraint(
                name: "CHK_RecipeIngredient_OneIngredientType",
                sql: "RawMaterialId IS NOT NULL AND SubRecipeId IS NULL" +
                     " OR " +
                     "SubRecipeId IS NOT NULL AND RawMaterialId IS NULL"
            );
        }
    }
}

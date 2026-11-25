using BrightMinds.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BrightMinds.Infrastructure.DataAccess.Config
{
    public class RawMaterialIngredientConfig : IEntityTypeConfiguration<RawMaterialIngredient>
    {
        public void Configure(EntityTypeBuilder<RawMaterialIngredient> builder)
        {
            builder.HasOne(x=>x.RawMaterial)
                   .WithMany()
                   .HasForeignKey(x => x.RawMaterialId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

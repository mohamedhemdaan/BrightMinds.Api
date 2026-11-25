using BrightMinds.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BrightMinds.Infrastructure.DataAccess.Config
{
    public class FinalProductIngredientConfig : IEntityTypeConfiguration<FinalProductIngredient>
    {
        public void Configure(EntityTypeBuilder<FinalProductIngredient> builder)
        {
            builder.UseTptMappingStrategy();

            builder.HasOne(i => i.FinalProduct)
                   .WithMany(fp => fp.Ingredients)
                   .HasForeignKey(i => i.FinalProductId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

using BrightMinds.Core.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace BrightMinds.Infrastructure.DataAccess.Config
{
    public class PortionIngredientConfig : IEntityTypeConfiguration<PortionIngredient>
    {
        public void Configure(EntityTypeBuilder<PortionIngredient> builder)
        {
            builder.HasOne(x => x.ProductPortion)
                   .WithMany()
                   .HasForeignKey(x => x.ProductPortionId)
                   .OnDelete(DeleteBehavior.Restrict);

        }
    }
}

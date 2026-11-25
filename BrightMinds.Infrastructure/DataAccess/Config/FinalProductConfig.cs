using BrightMinds.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BrightMinds.Infrastructure.DataAccess.Config
{
    public class FinalProductConfig : IEntityTypeConfiguration<FinalProduct>
    {
        public void Configure(EntityTypeBuilder<FinalProduct> builder)
        {
            builder.HasMany(x=>x.Ingredients)
                   .WithOne(x=>x.FinalProduct)
                   .HasForeignKey(x => x.FinalProductId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

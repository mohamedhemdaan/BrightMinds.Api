using BrightMinds.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BrightMinds.Infrastructure.DataAccess.Config
{
    public class ProductPortionsConfig : IEntityTypeConfiguration<ProductPortions>
    {
        public void Configure(EntityTypeBuilder<ProductPortions> builder)
        {
            builder.Property(x => x.CreationDate)
                   .HasDefaultValueSql("SYSUTCDATETIME()");
        }
    }
}

using BrightMinds.Core.Enums;
using BrightMinds.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace BrightMinds.Infrastructure.DataAccess.Config
{
    public class ProductConfig : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(x => x.CreationDate)
                   .HasDefaultValueSql("SYSUTCDATETIME()")
                   .ValueGeneratedOnAdd();

            builder.Property(x => x.ProductionStatus)
                   .HasConversion<string>();

            builder.Property(x => x.ProductionStatus)
                .HasDefaultValue(ProductionStatus.Waiting);
        }
    }
}

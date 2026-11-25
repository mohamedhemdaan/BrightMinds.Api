using BrightMinds.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BrightMinds.Infrastructure.DataAccess.Config
{
    public class SupplyReturnConfig : IEntityTypeConfiguration<SupplyReturn>
    {
        public void Configure(EntityTypeBuilder<SupplyReturn> builder)
        {
            builder.Property(x => x.CreationDate)
                   .HasDefaultValueSql("SYSUTCDATETIME()");
        }
    }
}

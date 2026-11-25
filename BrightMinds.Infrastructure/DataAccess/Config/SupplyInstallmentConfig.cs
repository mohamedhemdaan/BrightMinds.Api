using BrightMinds.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BrightMinds.Infrastructure.DataAccess.Config
{
    public class SupplyInstallmentConfig : IEntityTypeConfiguration<SupplyInstallment>
    {
        public void Configure(EntityTypeBuilder<SupplyInstallment> builder)
        {
            builder.Property(x => x.CreationDate)
                   .HasDefaultValueSql("SYSUTCDATETIME()");
        }
    }
}

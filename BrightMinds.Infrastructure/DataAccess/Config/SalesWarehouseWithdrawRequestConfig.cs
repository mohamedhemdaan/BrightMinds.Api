using BrightMinds.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BrightMinds.Infrastructure.DataAccess.Config
{
    public class SalesWarehouseWithdrawRequestConfig : IEntityTypeConfiguration<SalesWarehouseWithdrawRequest>
    {
        public void Configure(EntityTypeBuilder<SalesWarehouseWithdrawRequest> builder)
        {
            builder.Property(x => x.CreationDate)
                   .HasDefaultValueSql("SYSUTCDATETIME()");
        }
    }
}

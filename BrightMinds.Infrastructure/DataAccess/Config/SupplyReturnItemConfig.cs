using BrightMinds.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BrightMinds.Infrastructure.DataAccess.Config
{
    public class SupplyReturnItemConfig : IEntityTypeConfiguration<SupplyReturnItem>
    {
        public void Configure(EntityTypeBuilder<SupplyReturnItem> builder)
        {
            builder.HasOne(x=>x.SupplyItem)
                   .WithMany()
                   .HasForeignKey(x=>x.SupplyReturnId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

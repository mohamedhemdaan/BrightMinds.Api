using BrightMinds.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BrightMinds.Infrastructure.DataAccess.Config
{
    public class RequestedSalesWarehouseInventoryItemConfig : IEntityTypeConfiguration<RequestedSalesWarehouseInventoryItem>
    {
        public void Configure(EntityTypeBuilder<RequestedSalesWarehouseInventoryItem> builder)
        {
            builder.HasOne(x => x.InventoryItem)
                   .WithMany()
                   .HasForeignKey(x => x.InventoryItemId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

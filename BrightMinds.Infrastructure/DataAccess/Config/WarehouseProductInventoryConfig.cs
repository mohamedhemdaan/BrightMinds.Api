using BrightMinds.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BrightMinds.Infrastructure.DataAccess.Config
{
    public class WarehouseProductInventoryConfig : IEntityTypeConfiguration<WarehouseProductsInventory>
    {
        public void Configure(EntityTypeBuilder<WarehouseProductsInventory> builder)
        {

        }
    }
}

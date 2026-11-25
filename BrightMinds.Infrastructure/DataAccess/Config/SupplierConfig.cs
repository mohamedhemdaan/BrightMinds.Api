using BrightMinds.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BrightMinds.Infrastructure.DataAccess.Config
{
    public class SupplierConfig : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            builder.HasMany(s=>s.Supplies)
                   .WithOne(s => s.Supplier)
                   .HasForeignKey(s => s.SupplierId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}

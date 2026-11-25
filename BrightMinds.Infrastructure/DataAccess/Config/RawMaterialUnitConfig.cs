using BrightMinds.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BrightMinds.Infrastructure.DataAccess.Config
{
    public class RawMaterialUnitConfig : IEntityTypeConfiguration<RawMaterialUnit>
    {
        public void Configure(EntityTypeBuilder<RawMaterialUnit> builder)
        {
            builder.HasOne(x=>x.RawMaterial)
                   .WithMany(x=>x.Units)
                   .HasForeignKey(x=>x.RawMaterialId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x=>x.Unit)
                   .WithMany()
                   .HasForeignKey(x=>x.UnitId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

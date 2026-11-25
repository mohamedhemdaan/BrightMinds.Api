using BrightMinds.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BrightMinds.Infrastructure.DataAccess.Config
{
    public class RawMaterialConfig : IEntityTypeConfiguration<RawMaterial>
    {
        public void Configure(EntityTypeBuilder<RawMaterial> builder)
        {
            builder.Property(p => p.UnitType).HasConversion<string>();

            builder.Property(p => p.UnitWeightInKg)
                   .HasPrecision(18, 6);

            builder.Property(p => p.InnerUnitWeightInKg)
                   .HasPrecision(18, 6); 

            builder.Property(p => p.InnerUnitsPerSubUnit)
                   .HasPrecision(18, 4);

            builder.Property(p => p.PricePerInnerUnit)
                   .HasPrecision(18, 4);

        }
    }
}

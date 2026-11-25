using BrightMinds.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace BrightMinds.Infrastructure.DataAccess.Config
{
    public class SupplyConfig : IEntityTypeConfiguration<Supply>
    {
        public void Configure(EntityTypeBuilder<Supply> builder)
        {
            builder.Property(s => s.CreationDate)
                   .HasDefaultValueSql("SYSUTCDATETIME()")
                   .ValueGeneratedOnAdd();

            builder.HasMany(s=>s.SupplyItems)
                   .WithOne(sI=>sI.Supply)
                   .HasForeignKey(s => s.SupplyId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

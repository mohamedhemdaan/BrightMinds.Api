using BrightMinds.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BrightMinds.Infrastructure.DataAccess.Config
{
    public class WithdrawItemConfig : IEntityTypeConfiguration<WithdrawItem>
    {
        public void Configure(EntityTypeBuilder<WithdrawItem> builder)
        {
            builder.Property(p => p.UnitType).HasConversion<string>();
        }
    }
}

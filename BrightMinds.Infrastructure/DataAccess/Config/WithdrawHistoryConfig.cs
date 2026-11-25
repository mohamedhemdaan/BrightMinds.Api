using BrightMinds.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace BrightMinds.Infrastructure.DataAccess.Config
{
    public class WithdrawHistoryConfig : IEntityTypeConfiguration<WithdrawHistory>
    {
        public void Configure(EntityTypeBuilder<WithdrawHistory> builder)
        {
            builder.Property(s => s.Date)
                   .HasDefaultValueSql("SYSUTCDATETIME()")
                   .ValueGeneratedOnAdd();
        }
    }
}

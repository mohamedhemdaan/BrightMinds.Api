using Blendd.Infrastructure.DataAccess;
using BrightMinds.Core.Interfaces;
using BrightMinds.Core.Models;

namespace BrightMinds.Infrastructure.Repositories
{
    public class WithdrawHistoryRepository : GenericRepository<WithdrawHistory>, IWithdrawHistoryRepository
    {
        public WithdrawHistoryRepository(AppDbContext context) : base(context)
        {
        }
    }
}

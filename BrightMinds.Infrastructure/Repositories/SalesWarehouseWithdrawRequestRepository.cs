using Blendd.Infrastructure.DataAccess;
using BrightMinds.Core.Interfaces;
using BrightMinds.Core.Models;

namespace BrightMinds.Infrastructure.Repositories
{
    public class SalesWarehouseWithdrawRequestRepository : GenericRepository<SalesWarehouseWithdrawRequest>, ISalesWarehouseWithdrawRequestRepository
    {
        public SalesWarehouseWithdrawRequestRepository(AppDbContext context) : base(context)
        {
        }
    }
}

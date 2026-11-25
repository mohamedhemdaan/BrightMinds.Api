using Blendd.Infrastructure.DataAccess;
using BrightMinds.Core.Interfaces;
using BrightMinds.Core.Models;

namespace BrightMinds.Infrastructure.Repositories
{
    public class RequestedSalesWarehouseInventoryItemRepository : GenericRepository<RequestedSalesWarehouseInventoryItem>, IRequestedSalesWarehouseInventoryItemRepository
    {
        public RequestedSalesWarehouseInventoryItemRepository(AppDbContext context) : base(context)
        {
        }
    }
}
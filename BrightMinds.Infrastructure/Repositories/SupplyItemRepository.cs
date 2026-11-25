using Blendd.Infrastructure.DataAccess;
using BrightMinds.Core.Interfaces;
using BrightMinds.Core.Models;

namespace BrightMinds.Infrastructure.Repositories
{
    public class SupplyItemRepository : GenericRepository<SupplyItem>, ISupplyItemRepository
    {
        public SupplyItemRepository(AppDbContext context) : base(context) { }
    }
}

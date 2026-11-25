using Blendd.Infrastructure.DataAccess;
using BrightMinds.Core.Interfaces;
using BrightMinds.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace BrightMinds.Infrastructure.Repositories
{
    public class SupplyReturnRepository : GenericRepository<SupplyReturn>, ISupplyReturnRepository
    {
        private readonly AppDbContext _context;
        public SupplyReturnRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<SupplyReturn>> GetAllSupplyReturnsWithSupplyId(int supplyId)
        {
            var supplyReturns = await _context.SupplyReturns
                                              .Include(x => x.Items)
                                              .Where(x => x.SupplyId == supplyId)
                                              .ToListAsync();
            return supplyReturns;
        }
    }
}

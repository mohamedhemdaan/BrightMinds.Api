using Blendd.Infrastructure.DataAccess;
using BrightMinds.Core.Enums;
using BrightMinds.Core.Interfaces;
using BrightMinds.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace BrightMinds.Infrastructure.Repositories
{
    public class WarehouseRepository : GenericRepository<Warehouse>, IWarehouseRepository
    {
        private readonly AppDbContext _context;
        public WarehouseRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Warehouse?> GetByTypeAsync(WarehouseType warehouseType)
        {
            var warehouse = await _context.Warehouses.FirstOrDefaultAsync(w => w.Type == warehouseType);
            return warehouse;
        }
    }
}

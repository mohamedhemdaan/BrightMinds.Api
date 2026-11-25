using Blendd.Infrastructure.DataAccess;
using BrightMinds.Core.Enums;
using BrightMinds.Core.Interfaces;
using BrightMinds.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace BrightMinds.Infrastructure.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly AppDbContext _context;

        public ReportRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<decimal> GetRawMaterialQuantityInWarehouse(int warehouseId, int rawMaterialId)
        {
            var quantity = await _context.WarehouseSuppliesInventory.Include(x=>x.SupplyItem)
                                   .Where(x=>x.WarehouseId == warehouseId && x.SupplyItem.RawMaterialId == rawMaterialId)
                                   .SumAsync(x=>x.UnitType == UnitType.g? x.Quantity/1000 : x.Quantity);

            return quantity;
        }

        public async Task<List<Supply>> GetEverySupplyContainsRawMaterial(int rawMaterialId)
        {
            var supplies = await _context.Supplies.Include(x=>x.SupplyItems)
                                                  .Include(x=>x.Supplier)
                                                  .Where(s => s.SupplyItems.Any(si=>si.RawMaterialId == rawMaterialId))
                                                  .ToListAsync();
            return supplies;
        }
    }
}

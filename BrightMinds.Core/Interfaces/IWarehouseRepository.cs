using BrightMinds.Core.Enums;
using BrightMinds.Core.Models;

namespace BrightMinds.Core.Interfaces
{
    public interface IWarehouseRepository : IGenericRepository<Warehouse>
    {
        public Task<Warehouse?> GetByTypeAsync(WarehouseType warehouseType);
    }
}

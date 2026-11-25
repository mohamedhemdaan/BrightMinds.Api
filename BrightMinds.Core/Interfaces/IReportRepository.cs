
using BrightMinds.Core.Models;

namespace BrightMinds.Core.Interfaces
{
    public interface IReportRepository
    {
        public Task<decimal> GetRawMaterialQuantityInWarehouse(int warehouseId, int rawMaterialId);
        public Task<List<Supply>> GetEverySupplyContainsRawMaterial(int rawMaterialId); 
    }
}

using BrightMinds.Core.Models;

namespace BrightMinds.Core.Interfaces
{
    public interface IWarehouseSuppliesInventoryRepository : IGenericRepository<WarehouseSuppliesInventory>
    {
        public Task DeleteInventoryItemsWithSupplyId(int id);
        public Task AddInventoryItem(WarehouseSuppliesInventory item);
        public Task AddInventoryItems(List<WarehouseSuppliesInventory> items);
        public Task TransfereToSubWarehouse(WarehouseSuppliesInventory item);
        public Task<WarehouseSuppliesInventory?> GetFromSubWarehouseAsync(int rawMaterialId);
        public Task<WarehouseSuppliesInventory?> GetFromMainWarehouseWithSupplyItemId(int supplyItemId);
    }
}

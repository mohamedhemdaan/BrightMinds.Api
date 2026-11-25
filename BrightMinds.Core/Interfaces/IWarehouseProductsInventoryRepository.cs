using BrightMinds.Core.Models;

namespace BrightMinds.Core.Interfaces
{
    public interface IWarehouseProductsInventoryRepository : IGenericRepository<WarehouseProductsInventory>
    {
        public Task AddProduct(ProductPortions productPortion);
    }
}

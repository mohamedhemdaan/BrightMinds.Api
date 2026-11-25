using Blendd.Infrastructure.DataAccess;
using BrightMinds.Core.Enums;
using BrightMinds.Core.Interfaces;
using BrightMinds.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace BrightMinds.Infrastructure.Repositories
{
    public class WarehouseProductsInventoryRepository : GenericRepository<WarehouseProductsInventory>, IWarehouseProductsInventoryRepository
    {
        private readonly AppDbContext _context;
        public WarehouseProductsInventoryRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task AddProduct(ProductPortions productPortion)
        {
            if (productPortion == null)
                throw new ArgumentNullException(nameof(productPortion), "productPortion is null.");

            if (productPortion.Product == null)
            {
                productPortion.Product = await _context.Products.FindAsync(productPortion.ProductId);
                if (productPortion.Product == null)
                    throw new Exception($"Product not found for ProductPortionId: {productPortion.Id}");
            }

            var recipeId = await _context.Products
                .Where(x => x.Id == productPortion.Product.Id)
                .Select(x => x.RecipeId)
                .FirstOrDefaultAsync();

            if (recipeId == 0)
                throw new Exception($"Product (Id: {productPortion.Product.Id}) is not linked to any Recipe.");

            var inventoryProduct = await _context.WarehouseProductsInventorys
                .Include(x => x.ProductPortion.Product)
                .Where(x => x.ProductPortion.Product.RecipeId == recipeId && x.Size == productPortion.SizeInGrams)
                .FirstOrDefaultAsync();

            var salesWarehouse = await _context.Warehouses
                .FirstOrDefaultAsync(w => w.Type == WarehouseType.SalesWarehouse);

            if (salesWarehouse == null)
                throw new Exception("SalesWarehouse not found. Please insert it into the database.");

            if (inventoryProduct == null)
            {
                var inventoryItem = new WarehouseProductsInventory()
                {
                    ProductPortion = productPortion,
                    Quantity = productPortion.Quantity,
                    Size = productPortion.SizeInGrams,
                    Warehouse = salesWarehouse,
                    Price = productPortion.Price
                };

                await _context.WarehouseProductsInventorys.AddAsync(inventoryItem);
                return;
            }

            inventoryProduct.Quantity += productPortion.Quantity;
            inventoryProduct.Price = productPortion.Price;
        }
      
    }
}
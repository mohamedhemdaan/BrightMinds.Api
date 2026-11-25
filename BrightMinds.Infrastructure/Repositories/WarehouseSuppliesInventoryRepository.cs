using Blendd.Infrastructure.DataAccess;
using BrightMinds.Core.Enums;
using BrightMinds.Core.Interfaces;
using BrightMinds.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace BrightMinds.Infrastructure.Repositories
{
    public class WarehouseSuppliesInventoryRepository : GenericRepository<WarehouseSuppliesInventory>, IWarehouseSuppliesInventoryRepository
    {
        private readonly AppDbContext _context;
        public WarehouseSuppliesInventoryRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task AddInventoryItem(WarehouseSuppliesInventory item)
        {
            var rawMaterialId = await _context.SupplyItems
                .Where(x => x.Id == item.SupplyItemId)
                .Select(x => x.RawMaterialId)
                .FirstOrDefaultAsync();

            var existingItem = await _context.WarehouseSuppliesInventory
                .Include(x => x.SupplyItem)
                .ThenInclude(x => x.RawMaterial)
                .Where(x => x.WarehouseId == item.WarehouseId &&
                            x.SupplyItem.RawMaterialId == rawMaterialId)
                .FirstOrDefaultAsync();

            if (existingItem == null)
            {
                await _context.WarehouseSuppliesInventory.AddAsync(item);
                return;
            }

            existingItem.ContainerQuantity += item.ContainerQuantity;
            existingItem.Quantity += item.Quantity;

            existingItem.Price = item.Price;      

            var rawMaterial = existingItem.SupplyItem?.RawMaterial;
            if (rawMaterial != null)
            {
                if (rawMaterial.UnitWeightInKg.HasValue)
                {
                    var totalWeight = existingItem.Quantity * rawMaterial.UnitWeightInKg.Value;
                    existingItem.DisplayDetails = rawMaterial.IsWeightBased ?
                        $"{existingItem.ContainerQuantity:F2} {rawMaterial.ContainerUnitName} ({existingItem.Quantity:F3} {rawMaterial.UnitType})" :
                        $"{existingItem.ContainerQuantity:F0} {rawMaterial.ContainerUnitName} = {existingItem.Quantity:F0} {rawMaterial.SubUnitName ?? rawMaterial.UnitType.ToString()} ({totalWeight:F3} كجم)";
                }
                else
                {
                    existingItem.DisplayDetails = rawMaterial.IsWeightBased ?
                        $"{existingItem.ContainerQuantity:F2} {rawMaterial.ContainerUnitName} ({existingItem.Quantity:F3} {rawMaterial.UnitType})" :
                        $"{existingItem.ContainerQuantity:F0} {rawMaterial.ContainerUnitName} = {existingItem.Quantity:F0} {rawMaterial.SubUnitName ?? rawMaterial.UnitType.ToString()}";
                }
            }

            _context.WarehouseSuppliesInventory.Update(existingItem);
        }

        public async Task AddInventoryItems(List<WarehouseSuppliesInventory> items)
        {
            foreach (var item in items)
            {
                await AddInventoryItem(item);
            }
        }

        public async Task DeleteInventoryItemsWithSupplyId(int id)
        {
            var inventoryItems = await _context.WarehouseSuppliesInventory.Where(i=>i.SupplyItem.SupplyId == id).ToListAsync();
            if (inventoryItems.Any()) return;
            _context.WarehouseSuppliesInventory.RemoveRange(inventoryItems);
        }

        public async Task<WarehouseSuppliesInventory?> GetFromMainWarehouseWithSupplyItemId(int supplyItemId)
        {
            var mainWarehouse = await _context.Warehouses.Where(x => x.Type == WarehouseType.MainWarehouse).FirstAsync();
            var inventoryItem = await _context.WarehouseSuppliesInventory
                                              .Where(x=>x.WarehouseId == mainWarehouse.Id && x.SupplyItemId == supplyItemId)
                                              .FirstOrDefaultAsync();

            return inventoryItem;
        }

        public async Task<WarehouseSuppliesInventory?> GetFromSubWarehouseAsync(int rawMaterialId)
        {
            int subWareId = _context.Warehouses.Where(x => x.Type == WarehouseType.SubWarehouse).First().Id;
            var inventoryItem = await _context.WarehouseSuppliesInventory
                                        .Include(x => x.SupplyItem)
                                        .Where(x => x.WarehouseId == subWareId && x.SupplyItem.RawMaterialId == rawMaterialId)
                                        .FirstOrDefaultAsync();

            return inventoryItem;
        }

        public async Task TransfereToSubWarehouse(WarehouseSuppliesInventory item)
        {
            var existingItem = await _context.WarehouseSuppliesInventory
                    .Where(x => x.WarehouseId == item.WarehouseId &&
                                x.SupplyItemId == item.SupplyItemId &&  
                                x.Price == item.Price)            
                    .FirstOrDefaultAsync();

            if (existingItem == null)
            {
                _context.WarehouseSuppliesInventory.Add(item);
                return;
            }

            if (item.UnitType != existingItem.UnitType)
            {
                if (existingItem.UnitType == UnitType.kg)
                    item.Quantity = item.Quantity * 1000;
                else
                    item.Quantity = item.Quantity / 1000;
            }

            existingItem.Quantity += item.Quantity;
        }
    }
}

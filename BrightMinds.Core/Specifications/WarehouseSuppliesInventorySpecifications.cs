using BrightMinds.Core.ISpecificationParams;
using BrightMinds.Core.Models;
using System.Linq.Expressions;

namespace BrightMinds.Core.Specifications
{
    public class WarehouseSuppliesInventorySpecifications : BaseSpecification<WarehouseSuppliesInventory>
    {
        public WarehouseSuppliesInventorySpecifications(IWarehouseInventorySpecParams? specParams, int? warehouseId,  int? inventoryItemId, bool isCount):
            base(BuildCriteria(specParams, warehouseId, inventoryItemId))
        {
            AddInclude(WI => WI.SupplyItem);
            AddIncludeString("SupplyItem.RawMaterial");
            AddIncludeString("SupplyItem.RawMaterial.Category");

            if (!isCount && specParams != null)
            {
                ApplyPagination(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);
                if (string.IsNullOrEmpty(specParams.OrderBy) || specParams.OrderBy == "new")
                    AddOrderByDesc(x => x.Id);
                else AddOrderBy(x => x.Id);
            }
        }

        private static Expression<Func<WarehouseSuppliesInventory, bool>> BuildCriteria(IWarehouseInventorySpecParams? specParams, int? warehouseId, int? inventoryItemId)
        {
            return x => (warehouseId == null || x.WarehouseId == warehouseId) &&
                        (inventoryItemId == null || x.Id == inventoryItemId)&&
                        (specParams == null || specParams.CategoryId == null || x.SupplyItem.RawMaterial.CategoryId == specParams.CategoryId)&&
                        (specParams == null ||
                         string.IsNullOrEmpty(specParams.SearchName) ||
                         x.SupplyItem.RawMaterial.Name.ToLower().Contains(specParams.SearchName.ToLower()));
        }
    }
}

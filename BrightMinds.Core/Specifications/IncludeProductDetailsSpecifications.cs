
using BrightMinds.Core.Models;
using System.Linq.Expressions;

namespace BrightMinds.Core.Specifications
{
    public class IncludeProductDetailsSpecifications : BaseSpecification<WarehouseProductsInventory>
    {
        public IncludeProductDetailsSpecifications(int? id = null) : base(BuildCriteria(id))
        {
            AddInclude(x => x.ProductPortion);
            AddIncludeString("ProductPortion.Product");
        }

        public static Expression<Func<WarehouseProductsInventory, bool>> BuildCriteria(int? Id = null)
        {
            return x => (Id == null || x.Id == Id);
        }
    }
}

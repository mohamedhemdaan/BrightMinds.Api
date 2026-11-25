using BrightMinds.Core.ISpecificationParams;
using BrightMinds.Core.Models;
using System.Linq.Expressions;

namespace BrightMinds.Core.Specifications
{
    public class WarehouseProductsSpecifications : BaseSpecification<WarehouseProductsInventory>
    {
        public WarehouseProductsSpecifications(IWarehouseProductsSpecParams specParams, bool isCount) : base(BuildeCriteria(specParams))
        {
            AddInclude(x => x.ProductPortion);
            AddIncludeString("ProductPortion.Product");

            if (!isCount)
            {
                ApplyPagination(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);
                if (string.IsNullOrEmpty(specParams.OrderBy) || specParams.OrderBy == "new")
                    AddOrderByDesc(x => x.Id);
                else AddOrderBy(x => x.Id);
            }
        }

        private static Expression<Func<WarehouseProductsInventory, bool>> BuildeCriteria(IWarehouseProductsSpecParams specParams)
        {
            return x => (
            string.IsNullOrEmpty(specParams.SearchName) ||
                   x.ProductPortion.Product.Name.ToLower().Contains(specParams.SearchName.ToLower()))

              &&
                (specParams.CategoryId == null ||
                 x.ProductPortion.CategoryId == specParams.CategoryId);
        }
    }
}

using BrightMinds.Core.ISpecificationParams;
using BrightMinds.Core.Models;
using System.Linq.Expressions;

namespace BrightMinds.Core.Specifications
{
    public class ProductSpecifications : BaseSpecification<Product>
    {
        public ProductSpecifications(IProductionSpecParams specParams, bool isCount) : base(BuildCriteria(specParams))
        {
            AddInclude(x => x.ProductPortions);
            if (!isCount)
            {
                ApplyPagination(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);
                if (string.IsNullOrEmpty(specParams.OrderBy) || specParams.OrderBy == "new")
                    AddOrderByDesc(x => x.Id);
                else AddOrderBy(x => x.Id);
            }
        }

        private static Expression<Func<Product, bool>> BuildCriteria(IProductionSpecParams specParams)
        {
            return x => ((string.IsNullOrEmpty(specParams.SearchName) || x.Name.ToLower().Contains(specParams.SearchName.ToLower())) &&
                       (specParams.ProductionDate == null || specParams.ProductionDate.Value.Date == x.CreationDate.Date) &&
                       ( specParams.ProductionStatus == null || specParams.ProductionStatus == x.ProductionStatus));
        }
    }
}


using BrightMinds.Core.ISpecificationParams;
using BrightMinds.Core.Models;
using System.Linq.Expressions;

namespace BrightMinds.Core.Specifications
{
    public class SupplierSpecifications : BaseSpecification<Supplier>
    {
        public SupplierSpecifications(ISupplierSpecParams specParams, bool isCount) : base(BuildCriteria(specParams))
        {
            if (!isCount)
            {
                ApplyPagination(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);
                if (string.IsNullOrEmpty(specParams.OrderBy) || specParams.OrderBy == "new")
                    AddOrderByDesc(x => x.Id);
                else AddOrderBy(x => x.Id);
            }
        }

        private static Expression<Func<Supplier, bool>> BuildCriteria(ISupplierSpecParams specParams)
        {
            return s => (string.IsNullOrEmpty(specParams.SearchName) || s.Name.ToLower().Contains(specParams.SearchName));
        }
    }
}

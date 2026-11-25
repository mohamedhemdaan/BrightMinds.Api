using BrightMinds.Core.ISpecificationParams;
using BrightMinds.Core.Models;
using System.Linq.Expressions;

namespace BrightMinds.Core.Specifications
{
    public class SupplyReturnSpecifications : BaseSpecification<SupplyReturn>
    {
        public SupplyReturnSpecifications(int id) : base(x=>x.Id == id)
        {
            AddInclude(x => x.Items);
        }

        public SupplyReturnSpecifications(ISupplyReturnSpecParams specParams, bool isCount) : base(BuildCriteria(specParams))
        {
            AddInclude(x=>x.Items);

            if (!isCount)
            {
                ApplyPagination(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);
                if (string.IsNullOrEmpty(specParams.OrderBy) || specParams.OrderBy == "new")
                    AddOrderByDesc(x => x.Id);
                else AddOrderBy(x => x.Id);

            }
        }

        private static Expression<Func<SupplyReturn, bool>> BuildCriteria(ISupplyReturnSpecParams specParams)
        {
            return x => (specParams.Status == null || x.Status == specParams.Status);
        }
    }
}

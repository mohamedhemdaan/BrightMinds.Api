using BrightMinds.Core.ISpecificationParams;
using BrightMinds.Core.Models;
using System.Linq.Expressions;

namespace BrightMinds.Core.Specifications
{
    public class BaseUnitSpecifications : BaseSpecification<BaseUnit>
    {
        public BaseUnitSpecifications(int id) : base(x=>x.Id == id)
        {
            AddInclude(x => x.Category);
        }

        public BaseUnitSpecifications(IUnitSpecParams specParams, bool isCount) : base(BuildCriteria(specParams))
        {
            AddInclude(x => x.Category);

            if (!isCount)
            {
                ApplyPagination(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);
                if (string.IsNullOrEmpty(specParams.OrderBy) || specParams.OrderBy == "new")
                    AddOrderByDesc(x => x.Id);
                else AddOrderBy(x => x.Id);
            }

        }

        private static Expression<Func<BaseUnit, bool>> BuildCriteria(IUnitSpecParams specParams)
        {
            return x => (string.IsNullOrEmpty(specParams.SearchName) || x.Name.ToLower().Contains(specParams.SearchName.ToLower()));
        }
    }
}

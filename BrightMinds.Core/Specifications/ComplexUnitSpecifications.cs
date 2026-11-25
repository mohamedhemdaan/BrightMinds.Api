using BrightMinds.Core.ISpecificationParams;
using BrightMinds.Core.Models;
using System.Linq.Expressions;

namespace BrightMinds.Core.Specifications
{
    public class ComplexUnitSpecifications : BaseSpecification<ComplexUnit>
    {
        public ComplexUnitSpecifications(int id) : base(x => x.Id == id) { }

        public ComplexUnitSpecifications(IUnitSpecParams specParams, bool isCount) : base(BuildCriteria(specParams))
        {
            if (!isCount)
            {
                ApplyPagination(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);
                if (string.IsNullOrEmpty(specParams.OrderBy) || specParams.OrderBy == "new")
                    AddOrderByDesc(x => x.Id);
                else AddOrderBy(x => x.Id);
            }

        }

        private static Expression<Func<ComplexUnit, bool>> BuildCriteria(IUnitSpecParams specParams)
        {
            return x => (string.IsNullOrEmpty(specParams.SearchName) || x.Name.ToLower().Contains(specParams.SearchName.ToLower()));
        }
    }
}

using BrightMinds.Core.ISpecificationParams;
using BrightMinds.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BrightMinds.Core.Specifications
{
    public class CategorySpecifications : BaseSpecification<Category>
    {
        public CategorySpecifications(ICategorySpecParams specParams, bool isCount)
            : base(BuildCriteria(specParams))
        { 
            if (!isCount)
            {
                ApplyPagination(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);
                if (string.IsNullOrEmpty(specParams.OrderBy) || specParams.OrderBy == "new")
                    AddOrderByDesc(x => x.Id);
                else AddOrderBy(x => x.Id);
            }
        }

        private static Expression<Func<Category, bool>> BuildCriteria(ICategorySpecParams specParams)
        {
            return x => (string.IsNullOrEmpty(specParams.SearchName) || x.Name.ToLower().Contains(specParams.SearchName));
        }
    }
}
